#if DEBUG

//TODO 
/* Optimizations:
 * Optimize before threading.
 * Free refrences when possible.
 * Use data types in loops.
 * Reuse allocations.
 * Reduce string allocations, especially in loops.
 * 
 * Math class:
 * Vector math extensions.
 * 
 * Improved texture atlasing.
 */

using KheaiGameEngine;
using KheaiUtils;
using SFML.Graphics;
using SFML.Window;

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
        KGameManager gameManager = new();

        Engine.AddComponent(gameManager);
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
    private string _textureID = "a";
    private KTextureAtlas _atlas;

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
        _atlas = ((KBatchRenderer) KEngine.ActiveRenderer).TextureAtlas;
        _atlas.GetTexCoords(_textureID);
    }

    public void Start()
    {
        
    }

    public void End()
    {

    }

    public void Update(uint currentTick)
    {
        
    }

    public void FrameUpdate(uint currentFrame)
    {
        
    }
}

public class KPlayer : KObjectComponent
{
    public int MoveSpeed = 5;

    public short Order { get; init; }
    public bool Enabled { get; set; }
    public string ID { get; init; }
    public KGameObject Owner { get; set; }

    public void Init()
    {

    }

    public void Start()
    {

    }

    public void End()
    {
        
    }

    public void Update(uint currentTick)
    {
        //Horizontal movement
        if (Keyboard.IsKeyPressed(Keyboard.Key.A))
        {
            Owner.Transform.PosX -= MoveSpeed;
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.D))
        {
            Owner.Transform.PosX += MoveSpeed;
        }

        //Vertical movement
        if (Keyboard.IsKeyPressed(Keyboard.Key.W))
        {
            Owner.Transform.PosY -= MoveSpeed;
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.S))
        {
            Owner.Transform.PosY += MoveSpeed;
        }
    }

    public void FrameUpdate(uint currentFrame)
    {

    }
}

#endif