using Foster.Framework;
using BaobabEngine.Graphics;
using UntitledGame;
using System.Numerics;

Game game = new();
game.Run();

public sealed class Game : App
{
    private Grid _grid;

    private Entity _player;

    private AtlasGenerator _atlas;

    public Game() : base(new AppConfig
    {
        ApplicationName = "AccEngII Final Project",
        WindowTitle = "Untitled Game",
        Width = 720,
        Height = 854,
        Fullscreen = false,
        Resizable = false,
        UpdateMode = UpdateMode.FixedStep(60)
    })
    {
        var windowSize = Window.SizeInPixels;

        _atlas = new AtlasGenerator("Assets", GraphicsDevice, [
            "tempAsset.png"
        ]);

        _grid = new Grid(new IntVector2(3, 5));

        _player = new Entity(
            new Sprite(
                _atlas.GetTexture("tempAsset"),
                2
            ),
            new Vector2(windowSize.X * 0.5f, windowSize.Y * (2f/3f)),
            3
        );
    }

    protected override void Startup() { }
    
    protected override void Shutdown() { }

    protected override void Update() { }

    protected override void Render() { }
}