using BaobabEngine.Graphics;
using Foster.Framework;
using System.Numerics;

namespace UntitledGame;

public class Entity
{
    protected Sprite Sprite;

    public Vector2 Position;

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

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    public bool IsDead() => CurrentHealth <= 0;
}