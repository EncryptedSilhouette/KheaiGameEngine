#if DEBUG

using KheaiGameEngine.Core;
using System.Collections;

public class Application : IKApplication
{
    public string AppName { get; set; }
    public string configFilePath { get; set; }
    public Hashtable appConfig { get; set; }
    public KEngine Engine { get; set; }

    public static void Main()
    {
        Application application = new();
        application.Engine = new(application);
        application.Start();
    }

    public void Start()
    {
        Engine.Start();
    }
}

#endif