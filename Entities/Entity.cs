using BaobabEngine.Graphics;
using Foster.Framework;
using System;
using System.Numerics;

namespace UntitledGame;

public class Entity
{
    protected Sprite Sprite;

    protected Vector2 Position;

    protected int MaxHealth;
    protected int CurrentHealth;

    public Entity(Sprite entitySprite, Vector2 entityPosition, int maxHealth)
    {
        Sprite = entitySprite;
        Position = entityPosition;

        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void Draw(Batcher batcher) => Sprite.Draw(batcher, Position);

    public void Update(Grid grid, Input input)
    {
        Position = grid.GetGlobalPositionFromGrid(this);

        if (input.Keyboard.Pressed(Keys.P))
        {
            Console.WriteLine($"Global X: {Position.X}\nGlobal Y: {Position.Y}\n");
            
            var gridPosition = grid.GetGridPosition(this);
            Console.WriteLine($"Local X: {gridPosition.X}\nLocal Y: {gridPosition.Y}\n");
        }

        var currentGridPosition = grid.GetGridPosition(this);

        if (input.Keyboard.Pressed(Keys.Up) && grid.IsCellInGrid(currentGridPosition + IntVector2.Up))
        {
            grid.MoveEntity(this, IntVector2.Up);
        }
        if (input.Keyboard.Pressed(Keys.Down) && grid.IsCellInGrid(currentGridPosition + IntVector2.Down))
        {
            grid.MoveEntity(this, new IntVector2(0,1));
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    public bool IsDead() => CurrentHealth <= 0;
}