#if DEBUG
using KheaiGameEngine.Core;
using System.Diagnostics;

KEngine engine = new KEngine();
engine.Attach(new TestRenderer());

engine.Start();

class TestRenderer : IKRenderer
{
    int ups = 0;
    int fps = 0;
    Stopwatch? stopwatch = null;

    public bool Enabled => true;

    public int Order => 0;

    public string ID => "renderer";

    public void Init<TParent>(TParent parent) { }

    public void Start()
    {
        Console.WriteLine("Starting renderer");
        stopwatch = Stopwatch.StartNew();
    }

    public void End() => Console.WriteLine("End of execution");

    public void Update(ulong currentUpdate)
    {
        if (stopwatch?.Elapsed.Seconds >= 1)
        {
            Console.WriteLine($"ups: {ups}, fps:{fps}");
            fps = ups = 0;
            stopwatch.Restart();
        }

        if (currentUpdate % 600 == 0 && currentUpdate != 0)
        {
            Thread.Sleep(1000);
        }
        ups++;
    }

    public void FrameUpdate(ulong currentUpdate) 
    {
        fps++;
    }

    public void RenderFrame(ulong currentFrame) { }
}

#endif