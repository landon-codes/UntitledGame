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

	public new float Width => BackSprite.Width;
	public new float Height => BackSprite.Height;

	public new Sprite Texture => BackSprite;
	
	public Sprite FrontSprite;
	public Sprite BackSprite;

	public BundledSprite(Subtexture frontTexture,
		Subtexture backTexture,
		float spriteScale = 1f,
		bool rotate90Degrees = false) : base()
	{
		FrontSprite = new Sprite(frontTexture, spriteScale, (rotate90Degrees) ?  0.5f * System.MathF.PI : 0f);
		BackSprite = new Sprite(backTexture, spriteScale);
	}

	public void Draw(in Batcher batcher, DrawArguments frontSpriteArguments, DrawArguments backSpriteArguments)
	{
		BackSprite.Draw(batcher,
			backSpriteArguments.Position,
			backSpriteArguments.Origin,
			backSpriteArguments.MirrorX,
			backSpriteArguments.MirrorY);

		FrontSprite.Draw(batcher,
			frontSpriteArguments.Position,
			frontSpriteArguments.Origin,
			frontSpriteArguments.MirrorX,
			frontSpriteArguments.MirrorY);
	}
}
