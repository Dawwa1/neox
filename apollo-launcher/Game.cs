using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;

namespace apollo_launcher
{
    public class Game
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string LaunchOptions { get; set; }
        public string Icon { get; set; }
        public int buttonRow { get; set; }
        public int buttonColumn { get; set; }
        public Game(string name, string path, int btn_row = 0, int btn_col = 0, string icon = null, string launchOptions = null)
        {
            Name = name;
            Path = path;
            Icon = icon;
            LaunchOptions = launchOptions;
        }

        public void launchGame()
        {
            if (MessageBox.Show("Are you sure you want to launch " + Path, "Launch game", MessageBoxButton.YesNo) == MessageBoxResult.OK)
            {
                Process.Start(Path);
            } else
            {
                // nothing
            }
        }
    }
}
