#if DEBUG
using KheaiGameEngine;

KApplication app = new("Debug");
app.AddComponents(new KComponent<KApplication>[]
{
    new KWindow(),
    new KEngine()
});

app.Start();
KDebug.DumpLog();
#endif
