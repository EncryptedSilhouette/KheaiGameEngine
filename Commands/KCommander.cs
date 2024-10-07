namespace KheaiGameEngine.Commands
{
    public class KCommander
    {
        public delegate int KCommandHandler(string[] args);

        public record KCommand(string id, string syntax, int argCount, KCommandHandler cmdAction);

        public static Dictionary<string, KCommand> commandRegistry = new();

        static KCommander()
        {

        }

        public int InterpretString(string[] args) => commandRegistry[args[0]]?.cmdAction?.Invoke(args) ?? 1; 
        
        public int InterpretString(string str) => InterpretString(str.Split());
    }
}
