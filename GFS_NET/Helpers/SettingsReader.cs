using GFS_NET.Objects;
using System.Text.Json;

namespace GFS_NET.Helpers
{
    public class SettingsReader
    {

        private string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\.."));
        private string jsonFilePath;

        //private string jsonFilePath;

        public SettingsReader() => this.jsonFilePath = Path.Combine(projectDir, "settings.json");

        public AppSettings ReadSettings()
        {
            if (File.Exists(jsonFilePath))
            {
                try
                {
                    string json = File.ReadAllText(jsonFilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading settings: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Settings file not found.");
            }

            return null;
        }
    }
}
