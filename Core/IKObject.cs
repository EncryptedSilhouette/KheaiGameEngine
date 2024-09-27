namespace KheaiGameEngine.Core
{
    /* 
     * The purpose of this interface is to give a foundation for any objects that will be updated by a game-loop.
     * All the below summaries suggest the intents I had for them.
     * However, I keep these open for expansion to allow for alternative intentions. 
     */

    ///<summary>Interface for KObjects.</summary>
    public interface IKObject
    {
        ///<summary>The active state of the IKObject.</summary>
        bool Enabled { get; }

        ///<summary>Represents whether this object should have any accompanying objects driving from the same type.</summary>
        bool IsUnique { get; }

        ///<summary>The priority this IKObject will be updated by the game-loop.</summary>
        int Order { get; }

        ///<summary>The identifier for the KObject.</summary>
        string ID { get; }

        ///<summary>The refrence to the parent object.</summary>
        IKObject Parent { get; }

        ///<summary>Executes starting tasks for the KObject.</summary>
        void Start();

        ///<summary>Executes tasks for the end of execution.</summary>
        void End();

        ///<summary>Executes update tasks every update.</summary>
        ///<param name = "currentUpdate">Keeps track of the current frame.</param>
        void Update(uint currentUpdate);

        ///<summary>Executes pre-draw tasks every update. This method is called after the update method.</summary>
        ///<param name = "currentUpdate">Keeps track of the current frame.</param>
        void FrameUpdate(uint currentUpdate);
    }
}
