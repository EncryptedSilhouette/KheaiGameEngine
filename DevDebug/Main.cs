#if DEBUG

using KheaiGameEngine.Core;
using KheaiGameEngine.DevDebug;
using System.Collections;

ResourceManager.RegisterComponent("Transform");

/*
Application application = new Application();
application.Start();
*/
public class Application : IKApplication
{
    public string AppName { get; set; } = "DebugTest";
    public string configFilePath { get; set; }
    public Hashtable appConfig { get; set; }
    public KEngine Engine { get; set; }
    
    public Application()
    {
        Engine = new(this);
    }

    public void Start()
    {
        DevTest.GenerateScene();
        Engine.Start();
    }
}

#endif