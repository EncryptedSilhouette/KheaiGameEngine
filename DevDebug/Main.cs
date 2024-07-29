#if DEBUG

using KheaiGameEngine;
using KheaiUtils;

public class GameApp : IKApplication
{
    public string AppName { get; init; }
    public KEngine Engine { get; init; }

    private GameApp() 
    {
        AppName = "KDebugGame";
        //Engine = new(this, 30, new BatchRenderer());
    }

    public static void Main() 
    {
        GameApp app = new GameApp();
        app.Start();
    }

    public void Init()
    {
        Load();
    }

    public void Load()
    {
        
    }

    public void Start()
    {
        Init();
        Engine.Start();
    }
}

#endif