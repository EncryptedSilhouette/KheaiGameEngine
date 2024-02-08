using System.Collections;

namespace KheaiGameEngine.Core
{
    ///<summary>
    ///Interface for a basic application using the standard KEngine.
    ///</summary>
    public interface IKApplication
    {
        ///<summary>
        ///The specified name for the app.
        ///</summary>
        public string AppName { get; set; }
        ///<summary>
        ///Filepath for the config file.
        ///</summary>
        public string configFilePath { get; set; }
        ///<summary>
        ///Stores the config data for the app.
        ///</summary>
        public Hashtable appConfig { get; set; }
        ///<summary>
        ///Reference for the KEngine.
        ///</summary>
        public KEngine Engine { get; set; }

        ///<summary>
        ///Start method for the app.
        ///</summary>
        public void Start();
    }
}
