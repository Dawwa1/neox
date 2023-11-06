using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Documents;
using neox;

namespace neox.Utility
{

    public class GameFile
    {
        public string title;
        public string target;
        public string launchOptions;
        public string tab;
    }

    public class JsonStorageManager
    {

        public string JsonFolderPath { get; set; }

        public JsonStorageManager(string json_folder_path)
        {
            if (!Directory.Exists(json_folder_path)) { Directory.CreateDirectory(json_folder_path); }
            JsonFolderPath = json_folder_path;
        }

        public static List<Game> loadAllApps(JsonStorageManager jsm)
        {
            List<Game> ret = new List<Game>();

            if (Directory.GetFiles(jsm.JsonFolderPath).Length > 0)
            {
                foreach (string file in Directory.GetFiles(jsm.JsonFolderPath))
                {
                    GameFile j = jsm.readGameFile(path: file);
                    Game game = new Game(j.title, j.target, j.tab);
                    ret.Add(game);
                }
            }

            return ret;
        }

        public void addNewGameFile(Game game)
        {

            var j = new JsonObject()
            {
                ["title"] = game.Name,
                ["target"] = game.Path,
                ["launchOptions"] = game.LaunchOptions,
                ["tab"] = game.Tab
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

        public GameFile readGameFile(Game? game = null, string? path = null)
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
