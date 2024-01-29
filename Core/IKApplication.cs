using System.Collections;

namespace KheaiGameEngine.Core
{
    public interface IKApplication
    {
        public string AppName { get; set; }
        public string configFilePath { get; set; }
        public Hashtable appConfig { get; set; }
        public KEngine Engine { get; set; }

        public void Start();
    }
}
