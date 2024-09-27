#if DEBUG

using KheaiGameEngine.Core;

namespace KheaiGameEngine.wip
{
    public class KCommand
    {
        public delegate int KCommandAction(KCommand caller, Span<string> input, int exitCode = 0);

        public string ID => "exit";

        public event KCommandAction OnExecution;
        public event Action<KCommand> OnError;

        public KCommand(KCommandAction action) => OnExecution += action;

        public int Execute(KCommand caller, Span<string> strings, int exitCode = 0)
        {
            if (OnExecution?.Invoke(caller, strings) == 1)
            {
                OnError?.Invoke(caller);
                return 1;
            };
            return 0;
        }

    }

    public class KConsole : IKObject
    {
        private bool _enabled;
        private string _id;

        protected int order;

        public bool IsUnique => true;

        public bool Enabled => _enabled;

        public int Order => order;

        public string ID => throw new NotImplementedException();

        public IKObject Parent => throw new NotImplementedException();

        public KConsole() => _id = GetType().Name;

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public void Update(uint currentUpdate)
        {
            throw new NotImplementedException();
        }

        public void FrameUpdate(uint currentUpdate)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<string, KCommand> commands = new()
        {
            //"exit", "exit w/task", "log open", "log close", "log clear"
        };

        public static void AddCommand()
        {

        }

        //public static void InterpretString(string str) => InterpretString(str.Split(' ').AsSpan(1));

        /*public static int InterpretString(Span<string> strings)
        {
            if (strings.Length > 1)
            {
                //commands[strings[0]].Execute(strings.Slice(1));
                return 0;
            }
        }*/
    }
}

#endif
