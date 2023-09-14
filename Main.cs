using KheaiGameEngine;

KApplication app = new("Debug");
app.AddComponents(new KAppComponent[]
{
    new KWindow(),
    new KEngine()
});

app.Start();
KDebug.Log("fsdk");
KDebug.DumpLog();
