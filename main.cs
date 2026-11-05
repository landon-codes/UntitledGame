using Foster.Framework;
using BaobabEngine.Graphics;
using UntitledGame;
using System.Numerics;
using System;
using BaobabEngine.Collisions;

Game game = new();
game.Run();

public sealed class Game : App
{
    private readonly Grid _grid;

    private readonly Entity _player;

    private readonly AtlasGenerator _atlas;

    private readonly Batcher _batcher;

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

        _batcher = new Batcher(GraphicsDevice);

        _grid = new Grid(new IntVector2(3, 5), Window);

        Sprite playerSprite = new Sprite(_atlas.GetTexture("tempAsset"), 0.5f);
        _player = new Entity(
            playerSprite,
            new Vector2(windowSize.X * 0.5f, windowSize.Y - (playerSprite.Height * 0.5f)),
            3
        );
    }

    protected override void Startup()
    {
        _grid.SubscribeEntity(_player, new IntVector2(1, 3));
    }
    
    protected override void Shutdown()
    {
        Console.WriteLine("The game has closed successfully.");
    }

    protected override void Update()
    {
        if (Input.Keyboard.Pressed(Keys.Escape))
            Exit();
        
        _player.Update(_grid, Input);
    }

    protected override void Render()
    {
        Window.Clear(Color.White);

        _player.Draw(_batcher);

        _batcher.Render(Window);
        _batcher.Clear();
    }
}