#if DEBUG

using KheaiGameEngine.Core;
using System.Collections;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using KheaiGameEngine.GameObjects;
using KheaiGameEngine.ObjectComponents;

JsonSerializerOptions options = new()
{
    WriteIndented = true,
    TypeInfoResolver = new KPolyTypeResolver()
};

KObjectData objectData = new()
{
    ID = "test",
    Components = new()
    {
        new KTransform()
    }
};

string jsonString = JsonSerializer.Serialize(objectData, options);

Console.WriteLine(jsonString);

public class Application : IKApplication
{
    public string AppName { get; set; }
    public string configFilePath { get; set; }
    public Hashtable appConfig { get; set; }
    public KEngine Engine { get; set; }

    public void Start()
    {
        Engine.Start();
    }
}

#endif