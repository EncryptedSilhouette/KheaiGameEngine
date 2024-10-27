using static KheaiGameEngine.Commands.KCommander;

namespace KheaiGameEngine.Commands
{
    public record KCommand(string id, string syntax, int argCount, KCommandHandler cmdAction);
    public record KCommandResult(int exitCode, string errOutput)
    {
        public KCommandResult(int exitCode = 0) : this(exitCode, string.Empty) { }
    }

    public class KCommander
    {
        public delegate KCommandResult KCommandHandler(KCommand self, string[] args);

        #region Static

        private static Dictionary<string, KCommand> s_commandRegistry = new();

        public static IReadOnlyCollection<KCommand> RegisteredCommands => s_commandRegistry.Values;

        public static bool RegisterCommand(KCommand command) 
        {
            if (s_commandRegistry.ContainsKey(command.id)) return false;
            s_commandRegistry[command.id] = command;
            return true;
        }

        public static KCommand GetCommandStartingWith(string beginning) => s_commandRegistry.Values
            .Where(value => value.id.StartsWith(beginning)).Order().First();

        #endregion

        public KCommandResult InterpretString(string[] args)
        {
            //Return err if args is null or if the array is empty.
            if (args is null || args.Length < 1) return new(1, "No command provided.");

            //Checks if the command exists, if it doesn't try to find one that matches.
            KCommand cmd = s_commandRegistry.ContainsKey(args[0]) ? s_commandRegistry[args[0]] : GetCommandStartingWith(args[0]);

            //If command is null return err, otherwise execute command and return result.
            return cmd is null ? new(1, $"command {args[0]} doesn't exist.") : cmd.cmdAction.Invoke(cmd, args);
        }

        public KCommandResult InterpretString(string str) => InterpretString(str.Split());
    }
}
