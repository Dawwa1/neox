using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace apollo_launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Game> games = new List<Game>();
        public JsonStorageManager jsm;

        public MainWindow()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\manifests";
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            jsm = new JsonStorageManager(path);

            if (Directory.GetFiles(path).Length > 0)
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    GameFile j = jsm.readGameFile(path: file);
                    Game game = new Game(j.title, j.target);
                    addGame(game); // worked first try /s
                }
            }
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

                if (File.Exists(jsm.JsonFolderPath + "\\" + name + ".json")) { MessageBox.Show("Application by that name exists!"); return; }

                Game game = new Game(name, path);

                NameInput nameInputDialog = new NameInput(game.Name);
                if (nameInputDialog.ShowDialog() == true) { game.Name = nameInputDialog.Answer; }
                else { return; }

                // Add to column/row
                addGame(game);
                if (games.Count == 16)
                {
                    disableBtn(addGame_btn);
                }
            }
        }

        private void addGame(Game game)
        {

            if (game.Icon == null)  // Can't add custom icons yet
            {
                Button button = new Button
                {
                    Content = game.Name,
                    ContextMenu = new ContextMenu(),
                };
                button.Click += new RoutedEventHandler(onImage_Click);
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

                    game.buttonRow = row;
                    game.buttonColumn = col;

                    jsm.addNewGameFile(game);

                } else
                {
                    Grid.SetColumn(button, 0);
                    Grid.SetRow(button, 0);
                }

                games.Add(game);
                gamesGrid.Children.Add(button);
                jsm.addNewGameFile(game);
            }
        }

        private void deleteMi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            Button button = (Button)contextMenu.PlacementTarget;
            Game game = getGameFromButton(button);

            games.Remove(game);
            gamesGrid.Children.Remove(button);
            jsm.removeGameFile(game);
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
