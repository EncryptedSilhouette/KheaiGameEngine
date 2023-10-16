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

app.AddComponents(new KAppComponent[]
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
class SceneManager : KEngineComponent
{
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

#region Entities
abstract class ObjectComponent : KEngineComponent
{
    
}

class GameObject : KComponentContainer<ObjectComponent>
{
    protected SortedSet<ObjectComponent> engineComponents = new(new KComponentSorter<ObjectComponent>());

    public void AddComponent(ObjectComponent component)
    {
        component.owner = (KComponentContainer<KComponent>)this;
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

    public Component GetComponent<Component>() where Component : ObjectComponent
    {
        foreach (KComponent component in engineComponents)
        {
            if (component is Component) return (Component)component;
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
    KEngine engine;

    public override void Init()
    {
        engine = (KEngine) owner;
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
