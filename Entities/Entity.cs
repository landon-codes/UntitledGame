using BaobabEngine.Graphics;
using Foster.Framework;
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
}