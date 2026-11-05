using Foster.Framework;

Game game = new();
game.Run();

public class Game : App
{
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
        
    }

    protected override void Startup() { }
    
    protected override void Shutdown() { }

    protected override void Update() { }

    protected override void Render() { }
}