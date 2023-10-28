#if DEBUG
using KheaiGameEngine;
using SFML.Graphics;

KApplication app = new("Debug");
KWindow window = new KWindow();  
KEngine engine = new KEngine();

app.AddComponents(new KAppComponent[]
{
    window,
    engine,
});

SceneManager sceneManager = new SceneManager();

engine.AddComponents(new KEngineComponent[]
{
    new RenderStuff(),
    sceneManager
});

#region Load
GameObject test = new("debug");
test.AddComponents(new ObjectComponent[]
{
    new Transform(),
    new DebugRenderer()
});

sceneManager.AddObjects(new GameObject[]
{
    test
});

app.Start();
KDebug.DumpLog();
#endregion

#region Renderer
class RenderStuff : KRenderer
{
    List<DebugRenderer> renderers = new();
    Drawable[] Drawables;

    public override void Draw(RenderTarget target)
    {
        foreach (var renderer in renderers)
        {
            target.Draw(renderer.sprite);
        }
    }

    public override void Init()
    {
        ActiveRenderer = this;
    }

    public void AddEntityRenderer(DebugRenderer renderer)
    {

    }

    public void AddEntityRenderers()
    {

    }

    public void RemoveEntityRenderer()
    {

    }

    #region Ignored
    public override void Start() { }
    public override void End() { }
    public override void FixedUpdate() { }
    public override void FrameUpdate(double deltaTIme) { }
    #endregion
}
#endregion

#region SceneManager
public class SceneManager : KEngineComponent
{
    List<string> removeObjects = new();
    List<GameObject> addObjects = new();
    Dictionary<string, GameObject> gameObjects = new();

    public override void Init()
    {

    }

    public override void Start()
    {

    }

    public override void End()
    {
        foreach (GameObject gameObject in gameObjects.Values)
        {
            gameObject.End();
        }
    }

    public override void FixedUpdate()
    {
        foreach (GameObject obj in gameObjects.Values)
        {
            obj.FixedUpdate();
        }

        lock (removeObjects)
        {
            if (removeObjects.Count > 0)
            {
                foreach (string name in removeObjects)
                {
                    gameObjects[name].End();
                    gameObjects.Remove(name);
                }
                removeObjects.Clear();
            }
        }

        lock (addObjects)
        {
            if (addObjects.Count > 0)
            {
                foreach (GameObject obj in addObjects)
                {
                    gameObjects.Add(GenerateObjectID(obj), obj);
                    obj.Start();
                }
                addObjects.Clear();
            }
        }
    }

    public override void FrameUpdate(double deltaTIme)
    {
        foreach (GameObject obj in gameObjects.Values)
        {
            obj.FrameUpdate(deltaTIme);
        }
    }

    public void Draw(RenderTarget target)
    {

    }

    public void RemoveObject(string id)
    {
        //Locked for insertion
        lock (removeObjects)
        {
            removeObjects.Add(id);
        }
    }

    public void AddObject(GameObject gameObject)
    {
        gameObject.Init();

        //Locked for insertion
        lock (addObjects)
        {
            addObjects.Add(gameObject);
        }
    }

    public void AddObject(GameObject gameObject, string name)
    {
        gameObject.Name = name;
        gameObject.SceneManager = this;
        AddObject(gameObject);
    }

    public void AddObjects(GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            AddObject(gameObject);
        }
    }

    private string GenerateObjectID(GameObject obj)
    {
        string name = obj.Name;
        for (int i = 0; gameObjects.ContainsKey(name); i++) 
        {
            name = obj.Name + i;
        }
        return name;
    }
}
#endregion

#region Entities

#region ObjectComponent
public abstract class ObjectComponent : IKComponent, IKEngineManaged
{
    public int Order { get; set; }
    public string ID { get; init; }
    public GameObject Owner { get; set; }

    public ObjectComponent()
    {
        ID = GetType().Name;    
    }

    public abstract void Init();
    public abstract void Start();
    public abstract void End();
    public abstract void FixedUpdate();
    public abstract void FrameUpdate(double deltaTIme);
}
#endregion

#region GameObject
public class GameObject : IKComponentContainer<ObjectComponent>, IKContainerManaged, IKEngineManaged
{
    public string ID { get; set; }
    public string Name { get; set; }
    public SceneManager SceneManager { get; set; }

    protected SortedSet<ObjectComponent> objectComponents = new(new KComponentSorter<ObjectComponent>());

    public GameObject(string id) : this(id, id) { }

    public GameObject(string id, string name)
    {
        ID = id;
        Name = name;
    }

    public void Init() 
    {
    }

    public void Start()
    {
        foreach (ObjectComponent component in objectComponents)
        {
            component.Start();
        }
    }

    public void End() 
    {
        foreach (ObjectComponent component in objectComponents)
        {
            component.End();
        }
    }

    public void AddComponent(ObjectComponent component)
    {
        component.Owner = this;
        component.Init();
        objectComponents.Add(component);
    }

    public void AddComponents(ObjectComponent[] components)
    {
        foreach (var component in components)
        {
            AddComponent(component);
        }
    }

    public void RemoveComponent(string id)
    {
        foreach (var component in objectComponents)
        {
            if (component.ID.Equals(id))
            {
                objectComponents.Remove(component);
                return;
            }
        }
    }

    public void RemoveComponent<Component>()
    {
        foreach (var component in objectComponents)
        {
            if (component is Component)
            {
                objectComponents.Remove(component);
                return;
            }
        }
    }

    public bool HasComponent<Component>()
    {
        foreach (var component in objectComponents)
        {
            if (component is Component) return true;
        }
        return false;
    }

    public bool HasComponent(string id)
    {
        foreach (var component in objectComponents)
        {
            if (component.ID.Equals(id)) return true;
        }
        return false;
    }

    public Component GetComponent<Component>() where Component : ObjectComponent
    {
        foreach (IKComponent component in objectComponents)
        {
            if (component is Component) return (Component) component;
        }
        return null;
    }

    public ObjectComponent GetComponent(string id)
    {
        foreach (var component in objectComponents)
        {
            if (component.ID.Equals(id)) return component;
        }
        return null;
    }

    public void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    public void FrameUpdate(double deltaTIme)
    {
        throw new NotImplementedException();
    }
}
#endregion

#region EntityCompoents

#region Transform
class Transform : ObjectComponent
{
    public int X {  get; set; }
    public int Y { get; set; }

    public override void End()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void FrameUpdate(double deltaTIme)
    {
        
    }

    public override void Init()
    {
        
    }

    public override void Start()
    {
        
    }
}
#endregion

#region DebugRenderer
class DebugRenderer : ObjectComponent
{
    public Drawable sprite;
    public KRenderer renderer;

    public override void Init()
    {
        CircleShape image = new(64);
        image.FillColor = Color.Green;
        image.Position = new(100, 100);

        sprite = image;
    }

    public override void Start()
    {
        renderer = Owner.SceneManager.Engine.GetComponent<RenderStuff>();
    }

    public override void FixedUpdate()
    {
        
    }

    public override void FrameUpdate(double deltaTIme)
    {
        
    }

    public override void End()
    {

    }
}
#endregion

#endregion

#endregion

#endif