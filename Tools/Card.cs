using BaobabEngine.Graphics;

namespace UntitledGame;

public interface ICard
{
	// The action the card does when used.
	public void Action(Entity player, Entity[] enemies);
}
