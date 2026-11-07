using BaobabEngine.Graphics;
using Foster.Framework;
using System;
using System.Numerics;

namespace UntitledGame;

public interface ICard
{
	// The action the card does when used.
	public void Action();

	public void Draw(Batcher batcher);
}

public class StrategicRetreatCard : ICard
{
	private Sprite _sprite;
	private Vector2 _position;

	private Grid _grid;
	private Entity _player;
	
	// Stores the direction the player will be moved.
	public IntVector2? MovementDirection;

	public readonly struct Config (
		Sprite sprite,
		Vector2 position,
		Grid grid,
		Entity player,
		IntVector2? movementDirection = null
	)
	{
		public readonly Sprite CardSprite = sprite;
		public readonly Vector2 CardPosition = position;
		public readonly Grid Grid = grid;
		public readonly Entity Player = player;
		public readonly IntVector2? MovementDirection = movementDirection;
	}

	public StrategicRetreatCard(Config cardConfig)
	{
		_sprite = cardConfig.CardSprite;
		_position = cardConfig.CardPosition;		 

		_grid = cardConfig.Grid;
		_player = cardConfig.Player;

		MovementDirection = cardConfig.MovementDirection;
	}

	public void Draw(Batcher batcher)
	{
		_sprite.Draw(batcher, _position);
	}

	public void Action()
	{
		if (MovementDirection is null)
		{
			Console.Error.WriteLine("The value `public IntVector2 MovmentDirection` has not been assigned yet.\n Please assign it before using a `StrategicRetreatCard`.");
			return;
		}

		try
		{
			var playerPosition = _grid.GetGridPosition(_player);
			IntVector2 newPosition = playerPosition + (IntVector2)MovementDirection;

			if (_grid.CellOccupied(newPosition))
			{
				Console.Error.WriteLine("Moving the player was skipped because the cell they wanted to move to was occupied.");
				return;
			}
			if (!_grid.IsCellInGrid(newPosition))
			{
				Console.Error.WriteLine("The player was not moved because they requested to move off the grid.");
				return;
			}

			_grid.MoveEntity(_player, (IntVector2)MovementDirection);
		}
		catch (System.Collections.Generic.KeyNotFoundException)
		{
			Console.Error.WriteLine("The player appears to have not been subscribed before running a `StrategicRetreatCard`.");
		}
	}	
}
