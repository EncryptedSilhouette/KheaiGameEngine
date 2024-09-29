namespace KheaiGameEngine.Core
{
    public class KAppSettings
    {
        private Dictionary<string, object> s_appData = new();

        public uint UpdateRate = 30;

        public string AppName = "KheaiApp";

        public string DebugDirectory = "Debug";

        public void AddDataPair(string key, object value) => s_appData.Add(key, value);

        public void RemoveDataPair(string key) => s_appData.Remove(key);

        public void RestoreLast() 
        {
            
        }

        public void RestoreDefault() 
        {

        }
    }
}
