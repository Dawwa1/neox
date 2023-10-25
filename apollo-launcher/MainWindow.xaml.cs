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
            ofd.Filter = "Executables (*.exe)|*.exe";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                string[] name_split = ofd.FileName.Split("\\");
                string[] name_tmp = name_split[name_split.Length - 1].Split(".")[..^1];
                string name = string.Join(string.Empty, name_tmp);
                string path = ofd.FileName;
                Game game = new Game(name, path);

                NameInput nameInputDialog = new NameInput(game.Name);
                if (nameInputDialog.ShowDialog() == true) { game.Name = nameInputDialog.Answer; }
                else { return; }

                games.Add(game);

                // Add to column/row
                addGameToView(game);
                if (games.Count == 16)
                {
                    disableBtn(addGame_btn);
                }
            }
        }

        //private Button createButton(string name, RoutedEventHandler clickHandler)
        //{
        //    Button button = new Button();
        //
        //    return button;
        //}

        private void addGameToView(Game game)
        {
            if (game.Icon == null)
            {
                Button button = new Button
                {
                    Content = game.Name,
                    ContextMenu = new ContextMenu(),
                };
                button.Click += new RoutedEventHandler(onImage_Click);
                //button.ContextMenu = new ContextMenu();
                MenuItem deleteMi = new MenuItem();
                deleteMi.Header = "Delete";
                deleteMi.Click += deleteMi_Click;
                button.ContextMenu.Items.Add(deleteMi);

                if (gamesGrid.Children.Count > 0) {
                    var child = gamesGrid.Children[gamesGrid.Children.Count - 1];

                    int row = Grid.GetRow(child);
                    int col = Grid.GetColumn(child);
                    //int col = Grid.GetColumn(child);
                    if (col == 3) { row += 1; col = 0; }
                    else { col += 1; }

                    Grid.SetColumn(button, col);
                    Grid.SetRow(button, row);
                    gamesGrid.Children.Add(button);
                    game.buttonRow = row;
                    game.buttonColumn = col;
                } else
                {
                    Grid.SetColumn(button, 0);
                    Grid.SetRow(button, 0);
                    gamesGrid.Children.Add(button);
                }
            }
        }

        private void deleteMi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            Button button = (Button)contextMenu.PlacementTarget;

            foreach (Game game in games)
            {
                if (game.buttonColumn == Grid.GetColumn(button) && game.buttonRow == Grid.GetRow(button))
                {
                    games.Remove(game);
                    gamesGrid.Children.Remove(button);
                    break;
                }
            }
        }

        private Game getGameFromButton(Button button)
        {
            foreach (Game game in games)
            {
                if (game.buttonColumn == Grid.GetColumn(button) && game.buttonRow == Grid.GetRow(button))
                {
                    return game;
                }
            }

            return new Game(null, null);
        }

        private void disableBtn(Button btn)
        {
            btn.Opacity = 0.3;
            btn.IsEnabled = false;
        }

        private void onImage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (MessageBox.Show("Are you sure you want to launch " + btn.Content) == MessageBoxResult.OK)
            {
                Process.Start(getGameFromButton(btn).Path);
            }
        }
    }
}
