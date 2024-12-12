#nullable disable

namespace KheaiGameEngine.Core
{
    ///<summary>Interface for IKEngineObjects.</summary>
    public interface IKEngineObject
    {
        ///<summary>The active state of the IKEngineObject.</summary>
        bool Enabled { get; }

        ///<summary>The priority this IKEngineObject will be updated by the game-loop.</summary>
        int Order { get; }

        ///<summary>The identifier for the IKEngineObject.</summary>
        string ID { get; }

        ///<summary>Executes starting tasks for the IKEngineObject.</summary>
        void Start();

        ///<summary>Executes tasks for the end of execution.</summary>
        void End();

        ///<summary>Executes update tasks every update.</summary>
        ///<param name = "currentUpdate">Keeps track of the current frame.</param>
        void Update(ulong currentUpdate);

        ///<summary>Executes pre-draw tasks every update. This method is called after the update method.</summary>
        ///<param name = "currentUpdate">Keeps track of the current frame.</param>
        void FrameUpdate(ulong currentUpdate);
    }

    public class KEngineObjectComparer<T> : IComparer<T> where T : IKEngineObject
    {
        public int Compare(T a, T b) => a.Order > b.Order ? 1 : a.Order < b.Order ? -1 : 0;
    }
}
