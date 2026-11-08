using BaobabEngine.Collisions;
using Foster.Framework;
using System;
using System.Numerics;

namespace UntitledGame;

public interface ICard
{
	// The action the card does when used.
	public void Action();

	public void Draw(Batcher batcher);	

	public bool IsClicked();
}

public class StrategicRetreatCard : ICard
{
	private readonly BundledSprite _sprite;
	private readonly Vector2 _position;

	private readonly Grid _grid;
	private readonly Entity _player;

	private readonly Input _gameInput;
	
	// Stores the direction the player will be moved.
	public readonly IntVector2? MovementDirection;

	public readonly struct Config (
		BundledSprite sprite,
		Vector2 position,
		Grid grid,
		Entity player,
		Input gameInput,
		IntVector2? movementDirection = null		
	)
	{
		public readonly BundledSprite CardSprite = sprite;
		public readonly Vector2 CardPosition = position;
		public readonly Grid Grid = grid;
		public readonly Entity Player = player;
		public readonly IntVector2? MovementDirection = movementDirection;
		public readonly Input GameInput = gameInput;
	}

	public StrategicRetreatCard(Config cardConfig)
	{
		_sprite = cardConfig.CardSprite;
		_position = cardConfig.CardPosition;		 

		_grid = cardConfig.Grid;
		_player = cardConfig.Player;

		MovementDirection = cardConfig.MovementDirection;

		_gameInput = cardConfig.GameInput;
	}

	public void Draw(Batcher batcher)
	{
		bool mirrorX = false;
		
		// Makes sure the sprite matches the direction
		if (!(MovementDirection is null))
		{
			IntVector2 notNullMovementDirection = (IntVector2)MovementDirection;

			if (notNullMovementDirection.X == 1)
				mirrorX = true;
		}

		_sprite.Draw(batcher,
			frontSpriteArguments: new BundledSprite.DrawArguments(
				_position
			),
			backSpriteArguments: new BundledSprite.DrawArguments(
				_position,
				BaobabEngine.Graphics.Sprite.DrawPosition.Centered,
				mirrorX
			)
		);
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

			if (!_grid.IsCellInGrid(newPosition))
			{
				Console.Error.WriteLine("The player was not moved because they requested to move off the grid.");
				return;
			}
			if (_grid.CellOccupied(newPosition))
			{
				Console.Error.WriteLine("Moving the player was skipped because the cell they wanted to move to was occupied.");
				return;
			}
			

			_grid.MoveEntity(_player, (IntVector2)MovementDirection);
		}
		catch (System.Collections.Generic.KeyNotFoundException)
		{
			Console.Error.WriteLine("The player appears to have not been subscribed before running a `StrategicRetreatCard`.");
		}
	}	

	public bool IsClicked()
	{
		BoundingBox cardBounds = new BoundingBox(_sprite.BackSprite, _position);

		Vector2 mousePosition = new(_gameInput.Mouse.X, _gameInput.Mouse.Y);
		BoundingBox mouseBounds = new(
			mousePosition,
			width: 1,
			height: 1
		);

		return (mouseBounds.Intersects(cardBounds) && _gameInput.Mouse.Pressed(MouseButtons.Left));
	}
}

public class SurpriseAttackCard : ICard
{
	public readonly struct Config(BundledSprite cardSprite, Vector2 cardPosition, Grid grid, Entity player, Game gameInstance)
	{
		public readonly BundledSprite Sprite = cardSprite;
		public readonly Vector2 Position = cardPosition;
		public readonly Grid Grid = grid;
		public readonly Entity Player = player;

		public readonly Game GameInstance = gameInstance;
	}

	private readonly Grid _grid;
	private readonly Entity _player;

	private readonly BundledSprite _sprite;
	private readonly Vector2 _position;

	private readonly Game _game;

	public SurpriseAttackCard(Config cardConfig)
	{
		_sprite = cardConfig.Sprite;
		_position = cardConfig.Position;

		_grid = cardConfig.Grid;
		_player = cardConfig.Player;

		_game = cardConfig.GameInstance;
	}

	public void Draw(Batcher batcher)
	{
		_sprite.Draw(batcher,
			frontSpriteArguments: new BundledSprite.DrawArguments(
				_position
			),
			backSpriteArguments: new BundledSprite.DrawArguments(
				_position
			)
		);
	}		

	// Removes the entity above the player if it exists.
	public void Action()
	{
		try
		{
			var playerPosition = _grid.GetGridPosition(_player);
			var positionAbovePlayer = playerPosition + IntVector2.Up;

			if (!_grid.IsCellInGrid(positionAbovePlayer))
			{
				Console.Error.WriteLine("The position above the player was not on the grid.\nAborting `SurpriseAttackCard` action.");
				return;
			}				
	
			var entity = _grid.GetEntity(positionAbovePlayer);
			if (entity is null) return;

			_game.RemoveEntity(entity);					
		}
		catch (System.Collections.Generic.KeyNotFoundException)
		{
			Console.Error.WriteLine("The player appears to not have been subscribed before running a `SurpriseAttackCard`.\nThe action is being aborted.");
			return;
		}
	}

	public bool IsClicked()
	{
		BoundingBox cardBounds = new(_sprite.BackSprite, _position);

		Vector2 mousePosition = new(_game.Input.Mouse.X, _game.Input.Mouse.Y);
		BoundingBox mouseBounds = new(
			mousePosition,
			width: 1,
			height: 1
		);

		return (mouseBounds.Intersects(cardBounds) && _game.Input.Mouse.Pressed(MouseButtons.Left));
	}
}
