using System.Text.Json;

namespace KheaiGameEngine.Core
{
    public static class ResourceManager
    {
        //Component Registry
        private static Dictionary<string, Type> componentRegistry = new();

        public static void RegisterComponent(string componentID)
        {
            string path = $"KheaiGameEngine.KheaiGameEngine.ObjectComponents.{componentID}";
            Type type = Type.GetType(path);

            if (type == null)
            {
                Console.WriteLine($"{componentID}, not found.");
            }
            else 
            {
                Console.WriteLine($"Registering component: {componentID}.");
            }
        }

        static void Load()
        {
            //Load entitydata
            string[] files = { };

            foreach (var componentID in files) 
            {
                Type type = Type.GetType(componentID);

                if (type != null)
                {
                    KDebugger.ErrorLog($"");
                }
                JsonSerializer.Deserialize("", type);
            }
        }
    }
}
