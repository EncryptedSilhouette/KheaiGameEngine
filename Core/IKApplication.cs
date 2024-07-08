namespace KheaiGameEngine
{
    ///<summary>A basic interface for creating a KApplication.</summary>
    public interface IKApplication
    {
        ///<summary>The name of the app.</summary>
        public string AppName { get; set; }

        ///<summary>The reference for the engine.</summary>
        public KEngine Engine { get; set; }

        ///<summary>Executes code for loading resources.</summary>
        public void Load();

        ///<summary>Executes any initilization code for the application</summary>
        public void Init();

        ///<summary>Starting point for the engine.</summary>
        public void Start();
    }
}
