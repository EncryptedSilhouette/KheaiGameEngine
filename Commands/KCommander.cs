using KheaiGameEngine.Core;

namespace KheaiGameEngine.Commands
{
    public delegate KCommandResult KCommandEventHandler(string[] args);

    public record KCommandResult(int ExitCode, string? ErrorLog);
    public record struct KCommandData(int ParameterCount, string ID, string Description, KCommandEventHandler ExecutionAction);

    public class KCommandHandler
    {
        public static readonly KCommandResult COMMAND_NOT_FOUND = new(1, "Err: Command not found");
        protected static readonly Dictionary<string, KCommandData> s_registeredCommands = [];

        public static void RegisterCommand(KCommandData commandData) => s_registeredCommands.Add(commandData.ID, commandData);

        public static string AutoCompleteCommand(string commandID) => s_registeredCommands.Keys
                .Where(str => str.StartsWith(commandID))
                .Order()?
                .First() ?? commandID;

        public static KCommandResult ExecuteString(string input)
        {
            string[] tokens = input.Split(' ');
            string commandID = AutoCompleteCommand(tokens[0]);
            KCommandResult result;

            if (!s_registeredCommands.ContainsKey(commandID)) tokens[0] = AutoCompleteCommand(commandID);
            if (tokens[0] is null)
            {
                result = COMMAND_NOT_FOUND;
                KDebugger.ErrorLog($"{result.ErrorLog!} - {commandID}");
                return result;
            }
            return s_registeredCommands[commandID].ExecutionAction.Invoke(tokens);
        }
    }
}
