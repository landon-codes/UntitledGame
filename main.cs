using Foster.Framework;
using BaobabEngine.Graphics;
using UntitledGame;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.IO;

Game game = new();
game.Run();

struct GameEndData
{
    public bool GameOver = false;
    public string Message = "";

    public GameEndData() {}

    public GameEndData(bool gameOver, string message)
    {
        GameOver = gameOver;
        Message = message;
    }
}

public sealed class Game : App
{
    private readonly Grid _grid;

    private readonly Entity _player;

    private readonly AtlasGenerator _atlas;

    private readonly Batcher _batcher;

    private List<Entity> _enemies;

    private string _turn = "player";

    private GameEndData _gameOver = new();
    
    private SpriteFont _font;

    private const string GameOverMessage = "Odysseus died...";

    private readonly float _gameOverMessageWidth;

    private ICard[]? _cards;

    private int _enemiesToAdd = 0;

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
            "CardBack.aseprite",
            "StrategicRetreatSprite.aseprite",
            "SurpriseAttack.aseprite",
            "tempAsset.png",
            "Arrow.png",
            "Sword.png",
            "Odysseus.aseprite"
        ]);

        _batcher = new Batcher(GraphicsDevice);

        _enemies = new List<Entity>();

        _grid = new Grid(new IntVector2(3, 5), Window);

        _font = new SpriteFont(GraphicsDevice, Path.Combine("Assets", "Lorem-Demo.ttf"), 20);
        _gameOverMessageWidth = _font.WidthOf(GameOverMessage);

        Sprite playerSprite = new Sprite(_atlas.GetTexture("Odysseus"), 4f);
        _player = new Entity(
            playerSprite,
            new Vector2(windowSize.X * 0.5f, windowSize.Y - (playerSprite.Height * 0.5f)),
            maxHealth: 4
        );

        _cards = GenerateCards();
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

        if (_gameOver.GameOver)
        {
            // TODO: Make a restort button

            return;
        }

        if (_turn == "player")
        {
            if (_cards is null)
            {
                Console.Error.WriteLine("The cards were null when the player's turn arrived.");
            }
            else
            {
                foreach (var card in _cards)
                {
                    if (!card.IsClicked()) continue;

                    card.Action();
                    _turn = "enemies";
                    _cards = null;
                }
            }

            _player.Update(_grid);          
        }
        else if (_turn == "enemies")
        {
            UpdateEnemies();

            _turn = "player";
            _cards = GenerateCards();
        }

        if (_player.IsDead())
        {
            _gameOver.GameOver = true;
            _gameOver.Message = GameOverMessage;
        }
    }

    protected override void Render()
    {
        Window.Clear(Color.White);

        _player.Draw(_batcher);

        foreach (var enemy in _enemies) enemy.Draw(_batcher);

        if (_gameOver.GameOver)
        {
            _font.Draw(
                _batcher, 
                GameOverMessage, 
                new Vector2 (
                    (Window.WidthInPixels * 0.5f) - (0.5f * _gameOverMessageWidth),
                    Window.HeightInPixels * 0.5f
                ),
                Color.Black
            );
        }

        if (!(_cards is null))
        {
            foreach (var card in _cards)
                card.Draw(_batcher);
        }

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

            bool collidingWithPlayer = (enemyPosition + IntVector2.Down) == playerPosition;

            if (collidingWithPlayer)
                _player.TakeDamage(1);

            if (enemyPosition.Y >= 4 || collidingWithPlayer)
            {
                _grid.UnsubscribeEntity(enemy);

                newEnemies.Remove(enemy);

                entitiesRemoved++;
                
                continue;
            }

            if (_grid.CellOccupied(enemyPosition + IntVector2.Down)) continue;

            _grid.MoveEntity(enemy, IntVector2.Down);
        }
        _enemies = newEnemies;

        for (int i = 0; i < entitiesRemoved + _enemiesToAdd; i++) GenerateEnemy();
        _enemiesToAdd = 0;

        foreach (var enemy in _enemies) enemy.Update(_grid);
    }

    private void GenerateEnemy()
    {
        Random random = new Random();

        while (true)
        {
            var x = random.Next(0, 3);
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

    public void RemoveEntity(Entity entity)
    {        
        try
        {            
            _grid.UnsubscribeEntity(entity);
            _enemies.Remove(entity);

            if (entity != _player)
                _enemiesToAdd++;
        }
        catch (KeyNotFoundException)
        {
            Console.Error.WriteLine("An entity was trying to be removed from the grid when they were not subscribed.");
        }

    }

    private ICard[] GenerateCards()
    {
        ICard[] cards = new ICard[3];

        string[] cardTypes = {
            "surprise attack",
            "strategic retreat"
        };
        
        for (int i = 0; i < cards.Length; i++)
        {
            Random random = new();
            var cardType = cardTypes[random.Next(0, cardTypes.Length)];

            Vector2 GetCardPosition(float spriteWidth, float spriteHeight)
            {
                Vector2 cardPosition = new(
                    (Window.WidthInPixels / cards.Length) * i + 2 * (spriteWidth * 0.5f),
                    (spriteHeight * 0.5f)
                );

                return cardPosition; 
            }

            switch(cardType)
            {
                case "surprise attack":
                    BundledSprite surpriseAttackSprite = new(
                        frontTexture: _atlas.GetTexture("Sword"),
                        backTexture: _atlas.GetTexture("CardBack"),
                        spriteScale: 3
                    );
                
                    SurpriseAttackCard.Config surpriseCardConfig = new(
                        cardSprite: surpriseAttackSprite,
                        cardPosition: GetCardPosition(surpriseAttackSprite.Width, surpriseAttackSprite.Height),
                        grid: _grid,
                        player: _player,
                        gameInstance: this
                    );
                    cards[i] = new SurpriseAttackCard(surpriseCardConfig);

                    break;

                case "strategic retreat":
                    BundledSprite strategicRetreatSprite = new(
                        frontTexture: _atlas.GetTexture("Arrow"),
                        backTexture: _atlas.GetTexture("CardBack"),
                        spriteScale: 3,
                        rotate90Degrees: true
                    );
                    
                    IntVector2[] movementDirections = {
                        IntVector2.Left,
                        IntVector2.Right
                    };
                    var movementDirection = movementDirections[random.Next(0, movementDirections.Length)];
                
                    StrategicRetreatCard.Config retreatCardConfig = new(
                        sprite: strategicRetreatSprite,
                        position: GetCardPosition(strategicRetreatSprite.Width, strategicRetreatSprite.Height),
                        grid: _grid,
                        player: _player,
                        gameInstance: this,
                        movementDirection: movementDirection
                    );
                    cards[i] = new StrategicRetreatCard(retreatCardConfig);

                    break;
            }
        }

        return cards;
    }
}
