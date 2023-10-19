#if DEBUG
using KheaiGameEngine;
using SFML.Graphics;

KApplication app = new("Debug");
KWindow window = new KWindow();  
KEngine engine = new KEngine();

#region Engine
SceneManager sceneManager = new SceneManager();

engine.AddComponent(sceneManager);
#endregion

app.AddComponents(new IKAppComponent[]
{
    window,
    engine,

    //Test
    new RenderStuff()
});

app.Start();
KDebug.DumpLog();


#region Renderer
class RenderStuff : KRenderer
{
    Drawable[] Drawables;

    public override void Draw(RenderTarget target)
    {
        
    }


    public override void Init()
    {
        ActiveRenderer = this;
    }

    #region Ignored
    public override void Start() { }
    public override void Update() { }
    public override void End() { }
    #endregion
}
#endregion

#region SceneManager
class SceneManager : IKEngineComponent
{
    HashSet<string> names = new();
    List<string> removeObjects = new();
    List<GameObject> addObjects = new();
    Dictionary<string, GameObject> gameObjects = new();

    public int Order { get; set; }
    public string ID { get; init; }
    public KEngine Engine { get; set; }

    public void Init()
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void End()
    {
        throw new NotImplementedException();
    }

    public void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    public void FrameUpdate(double deltaTIme)
    {
        throw new NotImplementedException();
    }

    public void RemoveObject()
    {

    }

    public void AddObject()
    {

    }
}
#endregion

#region Entities
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

public class GameObject : IKComponentContainer<ObjectComponent>, IKEngineManaged
{
    protected SceneManager sceneManager { get; set; }
    protected SortedSet<ObjectComponent> engineComponents = new(new KComponentSorter<ObjectComponent>());

    public void AddComponent(ObjectComponent component)
    {
        component.Owner = this;
        component.Init();
        engineComponents.Add(component);
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
        foreach (var component in engineComponents)
        {
            if (component.ID.Equals(id))
            {
                engineComponents.Remove(component);
                return;
            }
        }
        KDebug.Log($"Failed to remove component {id}.");
    }

    public void RemoveComponent<Component>()
    {
        foreach (var component in engineComponents)
        {
            if (component is Component)
            {
                engineComponents.Remove(component);
                return;
            }
        }
        KDebug.Log($"Failed to remove component {typeof(Component).Name}.");
    }

    public bool HasComponent<Component>()
    {
        foreach (var component in engineComponents)
        {
            if (component is Component) return true;
        }
        return false;
    }

    public bool HasComponent(string id)
    {
        foreach (var component in engineComponents)
        {
            if (component.ID.Equals(id)) return true;
        }
        return false;
    }

    public Component GetComponent<Component>() where Component : class, ObjectComponent
    {
        foreach (IKComponent component in engineComponents)
        {
            if (component is Component) return (Component) component;
        }
        return null;
    }

    public ObjectComponent GetComponent(string id)
    {
        foreach (var component in engineComponents)
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
        throw new NotImplementedException();
    }

    public override void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    public override void FrameUpdate(double deltaTIme)
    {
        throw new NotImplementedException();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }

    public override void Start()
    {
        throw new NotImplementedException();
    }
}
#endregion

#region SpriteRenderer
class SpriteRenderer : ObjectComponent
{
    Drawable sprite;

    public override void Init()
    {
        
    }

    public override void Start()
    {

    }

    public override void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    public override void FrameUpdate(double deltaTIme)
    {
        throw new NotImplementedException();
    }

    public override void End()
    {
        throw new NotImplementedException();
    }
}
#endregion

#endregion
#endif
