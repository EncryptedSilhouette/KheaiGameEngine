namespace KheaiGameEngine
{
    ///<summary>A basic interface for creating a KApplication.</summary>
    public interface IKApplication
    {
        ///<summary>The name of the application.</summary>
        public string AppName { get; init; }

        ///<summary>A reference to the engine.</summary>
        public KEngine Engine { get; init; }

        ///<summary>Executes tasks for loading resources.</summary>
        public void Load();

        ///<summary>Executes initilization tasks for the application</summary>
        public void Init();

        ///<summary>Starting point for the engine.</summary>
        public void Start();
    }
}
