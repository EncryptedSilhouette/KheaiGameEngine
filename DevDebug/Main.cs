#if DEBUG

using KheaiGameEngine;
using KheaiUtils;
using SFML.Graphics;

public class GameApp : IKApplication
{
    public string AppName { get; init; }
    public KEngine Engine { get; init; }

    private GameApp() 
    {
        RenderStates renderStates = new();
        KBatchRenderer batchRenderer = new(32, renderStates, PrimitiveType.Quads);
        
        AppName = "KDebugGame";
        Engine = new(this, batchRenderer);
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
        string directory = "res\\assets";
        KBatchRenderer renderer = (KBatchRenderer) KEngine.ActiveRenderer;
        KTextureAtlas atlas = renderer.TextureAtlas = new();
        atlas.LoadTexture($"{directory}\\a.png");
        atlas.LoadTexture($"{directory}\\b.png");
        atlas.LoadTexture($"{directory}\\c.png");
        atlas.LoadTexture($"{directory}\\d.png");
        atlas.LoadTexture($"{directory}\\e.png");
        atlas.LoadTexture($"{directory}\\f.png");
        atlas.LoadTexture($"{directory}\\g.png");
        atlas.LoadTexture($"{directory}\\h.png");
        atlas.LoadTexture($"{directory}\\i.png");
        atlas.LoadTexture($"{directory}\\j.png");
        atlas.LoadTexture($"{directory}\\k.png");
        atlas.CreateAtlas();
    }

    public void Start()
    {
        Init();
        Engine.Start();
    }
}

public class KGameManager : IKEngineComponent
{
    public KEngine Engine { get; set; }
    public short Order { get; init; }
    public string ID { get; init; }

    public List<KGameObject> gameObjects = new(16);

    public KGameManager() 
    {
        Order = 0;
        ID = GetType().Name;
    }

    public void Init()
    {
        KGameObject gameObject = new("test");
        KEntity entity = new KEntity();
        

        gameObject.AddComponent(entity);
    }

    public void Start()
    {
        foreach (var gameObject in gameObjects) gameObject.Start();
    }

    public void End()
    {
        foreach (var gameObject in gameObjects) gameObject.End();
    }

    public void Update(uint currentUpdate)
    {
        foreach (var gameObject in gameObjects) gameObject.Update(currentUpdate);
    }

    public void FrameUpdate(uint currentUpdate)
    {
        foreach (var gameObject in gameObjects) gameObject.FrameUpdate(currentUpdate);
    }
}

public class KEntity : KObjectComponent
{
    public short Order { get; init; }
    public string ID { get; init; }

    public bool Enabled { get; set; }
    public KGameObject Owner { get; set; }

    public KEntity() 
    {
        Order = 0;
        ID = GetType().Name;

        Enabled = false;
    }

    public void Init()
    {
        
    }

    public void Start()
    {
        
    }

    public void Update(uint currentTick)
    {
        
    }

    public void End()
    {
        
    }

    public void FrameUpdate(uint currentFrame)
    {
        
    }
}

#endif