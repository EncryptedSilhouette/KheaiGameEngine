using System.Text.Json;

namespace KheaiGameEngine.Core
{
    internal class ResourceManager
    {
        void Load()
        {
            Type type = Type.GetType("");

            if (type != null) 
            {

            }

            JsonSerializer.Deserialize("", type);
        }
    }
}
