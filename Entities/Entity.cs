using BaobabEngine.Graphics;
using Foster.Framework;
using System.Numerics;

namespace UntitledGame;

public class Entity
{
    protected Sprite Sprite;

    protected Vector2 Position;

    public Entity(Sprite entitySprite, Vector2 entityPosition)
    {
        Sprite = entitySprite;
        Position = entityPosition;
    }

    public void Draw(Batcher batcher) => Sprite.Draw(batcher, Position);
}