using BaobabEngine.Graphics;
using Foster.Framework;
using System.Numerics;

public class BundledSprite : Sprite
{
	public readonly struct DrawArguments(Vector2 position,
		Sprite.DrawPosition origin = DrawPosition.Centered,
		bool mirrorX = false,
		bool mirrorY = false)
	{
		public readonly Vector2 Position = position;
		public readonly Sprite.DrawPosition Origin = origin;
		public readonly bool MirrorX = mirrorX;
		public readonly bool MirrorY = mirrorY;
	}
	
	private Sprite _frontSprite;
	private Sprite _backSprite;

	public BundledSprite(Subtexture frontTexture,
		Subtexture backTexture,
		float spriteScale = 1f) : base()
	{
		_frontSprite = new Sprite(frontTexture, spriteScale);
		_backSprite = new Sprite(backTexture, spriteScale);
	}

	public void Draw(in Batcher batcher, DrawArguments frontSpriteArguments, DrawArguments backSpriteArguments)
	{
		_backSprite.Draw(batcher,
			backSpriteArguments.Position,
			backSpriteArguments.Origin,
			backSpriteArguments.MirrorX,
			backSpriteArguments.MirrorY);

		_frontSprite.Draw(batcher,
			frontSpriteArguments.Position,
			frontSpriteArguments.Origin,
			frontSpriteArguments.MirrorX,
			frontSpriteArguments.MirrorY);
	}
}
