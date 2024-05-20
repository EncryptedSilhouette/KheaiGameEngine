#if DEBUG

using KheaiGameEngine;
using System.Collections;

Console.WriteLine($"Test: {1 % 5}");

//Application application = new Application();
//application.Start();

class Application : IKApplication
{
    public string AppName { get; set; }
    public string configFilePath { get; set; }
    public Hashtable appConfig { get; set; }
    public KEngine Engine { get; set; }

    public void Init()
    {
        Engine = new KEngine(this);
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