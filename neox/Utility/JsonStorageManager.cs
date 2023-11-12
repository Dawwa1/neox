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
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace neox.Utility
{

    public class ProgramFile
    {
        public readonly string title = "";
        public readonly string target = "";
        public readonly string launchOptions = "";
        public readonly string tab = "";
        public readonly string row = "";
        public readonly string col = "";
    }

    public class JsonStorageManager
    {

        public static string JsonFolderPath = "";

        public JsonStorageManager(string json_folder_path)
        {
            if (!Directory.Exists(json_folder_path)) { Directory.CreateDirectory(json_folder_path); }
            JsonFolderPath = json_folder_path;
        }

        public static List<Program> GetAllPrograms(JsonStorageManager jsm)
        {
            List<Program> ret = new List<Program>();
            string[] folder = JsonStorageManager.GetFolder();

            if (folder.Length > 0)
            {
                foreach (string file in folder)
                {
                    ProgramFile j = jsm.ReadProgramFile(file);
                    Program prog = new Program(j.title, j.target, j.tab);
                    ret.Add(prog);
                }
            }

            return ret;
        }

        public static string[] GetFolder()
        {
            return Directory.GetFiles(JsonFolderPath).OrderByDescending(d => new FileInfo(d).CreationTime).Reverse().ToArray();
        }

        public static bool DoesProgramExist(Program prog)
        {
            return File.Exists(JsonStorageManager.JsonFolderPath + "\\" + prog.Name + ".json");
        }

        public static Program? GetProgramFromPath(string path, List<Program> ProgramList)
        {
            foreach (Program prog in ProgramList)
            {
                if (path == prog.Path) { return prog; }
            }

            return null;
        }

        public void AddProgram(Program prog)
        {
            var j = new JsonObject()
            {
                ["title"] = prog.Name,
                ["target"] = prog.Path,
                ["launchOptions"] = prog.LaunchOptions,
                ["tab"] = prog.Tab,
                ["row"] = prog.ButtonRow,
                ["col"] = prog.ButtonColumn
            };

            var jOptions = new JsonSerializerOptions() { WriteIndented = true };
            var jString = j.ToJsonString(jOptions);

            File.WriteAllText(JsonFolderPath + "\\" + prog.Name + ".json", jString);
        }

        public void RemoveProgramFile(Program prog)
        {
            foreach (string file in Directory.GetFiles(JsonFolderPath))
            {
                if (file.Contains(prog.Name))
                {
                    File.Delete(file);
                }
            }
        }

        public ProgramFile ReadProgramFile(string path)
        {
            string jString = File.ReadAllText(path);

            ProgramFile ProgFile = JsonConvert.DeserializeObject<ProgramFile>(jString) ?? new ProgramFile();
            return ProgFile;
        }
    }
}
