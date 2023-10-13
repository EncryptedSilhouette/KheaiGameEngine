using System.Collections;
using System.Text.Json;

namespace KheaiGameEngine
{
    public class KUserPreferences
    {
        public string PrefsFilePath { get; set; } = "prefs";
        public Hashtable Prefrences { get; set; } = new();

        #region Serialization
        public string LoadPrefsFromFile(string filePath)
        {
            KDebug.Log($"Loading prefrences from {filePath}.");
            if (File.Exists(filePath))
            {
                PrefsFilePath = filePath;
                return File.ReadAllText(filePath);
            }
            return null;
        }

        public void LoadPrefsFromJson(string jsonString)
        {
            if (jsonString == null) return;
            Prefrences = JsonSerializer.Deserialize<Hashtable>(jsonString);
        }

        public void ClearPrefs()
        {
            File.Create(PrefsFilePath);
        }
        #endregion
    }
}
