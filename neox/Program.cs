using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using neox.Utility;
using System.IO;

namespace neox
{
    public class Program
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string? LaunchOptions { get; set; }
        public string? Icon { get; set; }
        public int ButtonRow { get; set; }
        public int ButtonColumn { get; set; }
        public string Tab { get; set; }

        public static Program GetProgramFromButton(Button button, List<Program> progs)
        {
            foreach (Program prog in progs)
            {
                if (prog.Name == button.Content.ToString())
                {
                    return prog;
                }
            }

            throw new ArgumentException("No button found by name + " + button.Name);
        }

        public Program(string name, string path, string tab, int btn_row = 0, int btn_col = 0, string? icon = null, string? launchOptions = null)
        {
            Name = name;
            Path = path;
            Icon = icon;
            LaunchOptions = launchOptions;
            Tab = tab;
            ButtonRow = btn_row;
            ButtonColumn = btn_col;
        }

        public void Launch()
        {
            if (MessageBox.Show("Are you sure you want to launch " + this.Name, "Launch prog", MessageBoxButton.YesNo) == MessageBoxResult.OK)
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
