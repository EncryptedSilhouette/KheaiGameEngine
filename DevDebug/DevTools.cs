namespace KheaiGameEngine.DevDebug
{
    public class DevTools
    {
        #region DevCommand
        private class DevCommand
        {
            public string ID { get; private set; }
            public string Description { get; set; }
            public Action CMDAction { get; private set; }

            public DevCommand(string id, string description, Action action)
            {
                ID = id;
                Description = description;
                CMDAction = action;
            }
        }
        #endregion

        private List<DevCommand> _devCommands = new();
        private Dictionary<string, DevCommand> _activeCommands = new();

        public DevTools()
        {
            LoadCMDs();
        }

        public void Open()
        {
            Console.WriteLine("DevTools");


        }

        private void ProcessCMD(string input)
        {
            string[] args = input.ToLower().Split(" ");

            _activeCommands[args[0]]?.CMDAction.Invoke();
        }

        private void CMDHelp()
        {

        }

        private void LoadCMDs()
        {
            foreach (var cmd in _devCommands)
            {
                _activeCommands.Add(cmd.ID, cmd);
            }
        }
    }
}
