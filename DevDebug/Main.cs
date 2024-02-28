#if DEBUG

using KheaiGameEngine.Core;
using System.Collections;

class Application : IKApplication
{
    public string AppName { get; set; }
    public string configFilePath { get; set; }
    public Hashtable appConfig { get; set; }
    public KEngine Engine { get; set; }

    public void Init()
    {
        Engine = new(this);
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