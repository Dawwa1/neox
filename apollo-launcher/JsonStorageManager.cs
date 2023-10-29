using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace apollo_launcher
{

    public class GameFile
    {
        public string title;
        public string target;
        public string launchOptions;
    }

    public class JsonStorageManager
    {

        public string JsonFolderPath {  get; set; }

        public JsonStorageManager(string json_folder_path)
        {
            if (!Directory.Exists(json_folder_path)) { Directory.CreateDirectory(json_folder_path); }
            JsonFolderPath = json_folder_path;
        }

        public void addNewGameFile(Game game)
        {

            var j = new JsonObject()
            {
                ["title"] = game.Name,
                ["target"] = game.Path,
                ["launchOptions"] = game.LaunchOptions
            };

            var jOptions = new JsonSerializerOptions() { WriteIndented = true };
            var jString = j.ToJsonString(jOptions);

            File.WriteAllText(JsonFolderPath + "\\" + game.Name + ".json", jString);
        }

        public void removeGameFile(Game game)
        {
            foreach (string file in Directory.GetFiles(JsonFolderPath))
            {
                if (file.Contains(game.Name))
                {
                    File.Delete(file);
                }
            }
        }

        public GameFile readGameFile(Game ?game = null, string ?path = null)
        {
            string jString;
            if (game != null)
            {
                jString = File.ReadAllText(findGameFile(game));
            }
            else
            {
                jString = File.ReadAllText(path);
            }

            GameFile gameFileJson = JsonConvert.DeserializeObject<GameFile>(jString);
            Debug.WriteLine(jString);
            Debug.WriteLine(gameFileJson);
            return gameFileJson;
        }

        public string findGameFile(Game game)
        {
            foreach (string file in Directory.GetFiles(JsonFolderPath))
            {
                if (file.Contains(game.Name))
                {
                    return file;
                }
            }
            return "";
        }
    }
}
