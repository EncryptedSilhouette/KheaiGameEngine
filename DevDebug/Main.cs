#if DEBUG

using KheaiGameEngine;
using KheaiGameEngine.EngineComponents;

KApplication app = new("Debug");
KWindow window = new KWindow();  
KEngine engine = new KEngine();

app.AddComponents(new KAppComponent[]
{
    window,
    engine,
});

KSceneHandler sceneManager = new KSceneHandler();

engine.AddComponents(new KEngineComponent[]
{
    new KSceneRenderer(),
    sceneManager
});

#region Load
KGameObject test = new("debug");
test.AddComponents(new KObjectComponent[]
{
    new KSpriteRenderer()
});

sceneManager.AddObjects(new KGameObject[]
{
    test
});

app.Start();
KDebug.DumpLog();
#endregion

#endif