using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace neox
{
    public class Game
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string LaunchOptions { get; set; }
        public string Icon { get; set; }
        public int buttonRow { get; set; }
        public int buttonColumn { get; set; }
        public string Tab { get; set; }

        static public Game getGameFromButton(Button button, List<Game> games)
        {
            foreach (Game game in games)
            {
                if (game.Name == button.Content)
                {
                    return game;
                }
            }

            return new Game(null, null, null);
        }

        public Game(string name, string path, string tab, int btn_row = 0, int btn_col = 0, string icon = null, string launchOptions = null)
        {
            Name = name;
            Path = path;
            Icon = icon;
            LaunchOptions = launchOptions;
            Tab = tab;
        }

        public void launchGame()
        {
            if (MessageBox.Show("Are you sure you want to launch " + this.Name, "Launch game", MessageBoxButton.YesNo) == MessageBoxResult.OK)
            {
                Process.Start(this.Path);
            } else
            {
                return;
            }
        }

        public bool Exists()
        {
            if (Name == null || Path == null) { return false; }
            return true;
        }
    }
}
