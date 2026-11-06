using Foster.Framework;
using BaobabEngine.Graphics;
using UntitledGame;
using System.Numerics;
using System;
using System.Collections.Generic;

Game game = new();
game.Run();

public sealed class Game : App
{
    private readonly Grid _grid;

    private readonly Entity _player;

    private readonly AtlasGenerator _atlas;

    private readonly Batcher _batcher;

    private List<Entity> _enemies;

    private string _turn = "player";

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

        _enemies = new List<Entity>();

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

        GenerateEnemy();
        GenerateEnemy();
    }
    
    protected override void Shutdown()
    {
        Console.WriteLine("The game has closed successfully.");
    }

    protected override void Update()
    {
        if (Input.Keyboard.Pressed(Keys.Escape))
            Exit();

        if (_turn == "player")
        {
            _player.Update(_grid);

            if (Input.Keyboard.Pressed(Keys.Space)) _turn = "enemies";
        }
        else if (_turn == "enemies")
        {
            UpdateEnemies();
            _turn = "player";
        }
    }

    protected override void Render()
    {
        Window.Clear(Color.White);

        _player.Draw(_batcher);

        foreach (var enemy in _enemies) enemy.Draw(_batcher);

        _batcher.Render(Window);
        _batcher.Clear();
    }

    private void UpdateEnemies()
    {
        int entitiesRemoved = 0;

        // Moves the entities down
        List<Entity> newEnemies = [.. _enemies];
        foreach (var enemy in _enemies)
        {
            var enemyPosition = _grid.GetGridPosition(enemy);
            var playerPosition = _grid.GetGridPosition(_player);

            if (enemyPosition.Y >= 4 || (enemyPosition + IntVector2.Down) == playerPosition)
            {
                _grid.UnsubscribeEntity(enemy);

                newEnemies.Remove(enemy);
                _player.TakeDamage(1);

                entitiesRemoved++;
                
                continue;
            }

            if (_grid.CellOccupied(enemyPosition + IntVector2.Down)) continue;

            _grid.MoveEntity(enemy, IntVector2.Down);
        }
        _enemies = newEnemies;

        for (int i = 0; i < entitiesRemoved; i++) GenerateEnemy();

        foreach (var enemy in _enemies) enemy.Update(_grid);
    }

    private void GenerateEnemy()
    {
        Random random = new Random();

        while (true)
        {
            var x = random.Next(0, 2);
            var y = 0;
            IntVector2 entityPosition = new(x,y);

            if (!_grid.CellOccupied(entityPosition))
            {
                Entity enemy = new(
                    new Sprite(
                        _atlas.GetTexture("tempAsset"),
                        0.5f
                    ),
                    _grid.GetGlobalPositionFromGrid(entityPosition),
                    1
                );

                _enemies.Add(enemy);
                _grid.SubscribeEntity(enemy, entityPosition);

                break;
            }
        }
    }
}