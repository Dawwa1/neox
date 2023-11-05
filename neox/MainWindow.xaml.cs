using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace neox
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
            foreach (Game game in JsonStorageManager.loadAllApps(jsm))
            {
                addGame(game);
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

        // TODO: Save game to whichever tab it was added to; Fix buttons on extra tabs; refactor this dogshit
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
                //if (games.Count >= 16)
                if(true)
                {
                    var lastTab = tabControl.Items[^1] as TabItem;
                    TabItem ti = new TabItem() { Header = Int32.Parse(lastTab.Header.ToString())+1};
                    Grid grid = new Grid();

                    for (int i = 0; i < 8; i++)
                    {
                        if (i != 4)
                        {
                            grid.RowDefinitions.Add(new RowDefinition());
                        }
                        else
                        {
                            grid.ColumnDefinitions.Add(new ColumnDefinition());
                        }
                    }

                    ti.Content = grid;
                    tabControl.Items.Add(ti);
                }
            }
        }

        private void addGame(Game game)
        {
            Grid grid;
            if (game.Icon == null)  // Can't add custom icons yet
            {
                if (tabControl.SelectedItem != null) { grid = (tabControl.SelectedItem as TabItem).Content as Grid; }
                else
                {
                    grid = (tabControl.Items[0] as TabItem).Content as Grid;
                }
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

                if (grid.Children.Count > 0) {
                    var child = grid.Children[grid.Children.Count - 1];

                    int row = Grid.GetRow(child);
                    int col = Grid.GetColumn(child);
                    if (col == 3) { row += 1; col = 0; }
                    else if (child == null) { col = 0; row = 0; }
                    else { col += 1; }

                    Grid.SetColumn(button, col);
                    Grid.SetRow(button, row);

                    game.buttonRow = row;
                    game.buttonColumn = col;
                }
                grid.Children.Add(button);
                games.Add(game);
                this.jsm.addNewGameFile(game);
            }
        }

        private void deleteMi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            Button button = (Button)contextMenu.PlacementTarget;
            Game game = Game.getGameFromButton(button, games);

            games.Remove(game);
            gamesGrid.Children.Remove(button);
            this.jsm.removeGameFile(game);
        }

        private void disableBtn(Button btn)
        {
            btn.Opacity = 0.3;
            btn.IsEnabled = false;
        }

        private void onImage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Game game = Game.getGameFromButton(btn, games);
            game.launchGame();
        }
    }
}
