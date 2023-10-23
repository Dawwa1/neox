using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static apollo_launcher.Game;

namespace apollo_launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Game> games = new List<Game>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void resizeBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void addGame_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            ofd.Filter = "Executables (*.exe)|*.exe";
            ofd.Multiselect = false;

            string[] name_split = ofd.FileName.Split("\\");
            //string name = "../" + name_split[name_split.Length - 2] + "/" + name_split[name_split.Length - 1];

            string name = name_split[name_split.Length - 1];
            string path = ofd.FileName;
            Game game = new Game(name, path);
            games.Add(game);

            // Add to column/row
            addGameToView(game);
        }

        private void addGameToView(Game game)
        {
            Button button = new Button();
            if (game.Icon == null)
            {
                button.Content = game.Name;

                if (gamesGrid.Children.Count > 0) {
                    foreach (UIElementCollection child in gamesGrid.Children)
                    {
                        foreach (UIElement element in child)
                        {
                            Debug.Write(element.ToString());
                        }
                    }
                } else
                {
                    Grid.SetColumn(button, 0);
                    Grid.SetRow(button, 0);
                    gamesGrid.Children.Add(button);
                }
            }
        }

        private void onImage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
