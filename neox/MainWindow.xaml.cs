using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using neox.Utility;

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
                foreach (TabItem ti in tabControl.Items) 
                {
                    if (ti.Header.ToString() == game.Tab) 
                    {
                        Grid g = getTabsGrid(ti);
                        addGame(game, g);
                    } 
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

        private Tuple<string, string> ofd()
        {
            string name = "", path = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Executables (*.exe)|*.exe";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                // very bad, dont feel like fixing
                string[] name_split = ofd.FileName.Split("\\");
                string[] name_tmp = name_split[name_split.Length - 1].Split(".")[..^1];
                name = string.Join(string.Empty, name_tmp);
                path = ofd.FileName;
            }

            return Tuple.Create(name, path);
        }

        private TabItem getCurrentTab(TabControl tc)
        {
            if (tabControl.SelectedItem != null) { return tc.SelectedItem as TabItem; }
            else { return tc.Items[0] as TabItem; }
        }

        private Grid getTabsGrid(TabItem ti)
        {
            return ti.Content as Grid;
        }

        // TODO: Save game to whichever tab it was added to; --Fix buttons on extra tabs--
        private void addGame_Click(object sender, RoutedEventArgs e)
        {
            var fd = ofd();

            if (fd.HasValue())
            {
                // very bad, dont feel like fixing
                string name = fd.Item1, path = fd.Item2;

                if (File.Exists(jsm.JsonFolderPath + "\\" + name + ".json")) { MessageBox.Show("Application by that name exists!"); return; }

                Game game = new Game(name, path, tab: getCurrentTab(tabControl).Header.ToString());

                NameInput nameInputDialog = new NameInput(game.Name);
                if (nameInputDialog.ShowDialog() == true) { game.Name = nameInputDialog.Answer; }

                if ((getCurrentTab(tabControl).Content as Grid).Children.Count >= 4 )
                {
                    var lastTab = tabControl.Items[^1] as TabItem;
                    TabItem ti = new TabItem() { Header = Int32.Parse(lastTab.Header.ToString()) + 1 };
                    Grid grid = new Grid();

                    for (int i = 0; i < 8; i++)
                    {
                        if (i < 4)
                        {
                            grid.RowDefinitions.Add(new RowDefinition());
                            Debug.WriteLine("Row created {0}", i);
                        }
                        else
                        {
                            grid.ColumnDefinitions.Add(new ColumnDefinition());
                            Debug.WriteLine("Col. created {0}", i);
                        }
                    }

                    ti.Content = grid;
                    tabControl.Items.Add(ti);

                    tabControl.SelectedIndex = tabControl.Items.Count-1;
                }

                addGame(game, getCurrentTab(tabControl).Content as Grid);
            }
        }

        private void addGame(Game game, Grid grid)
        {
            if (game.Icon == null)  // Can't add custom icons yet
            {
                // Creates button for program
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

                    // increments the next added program based on the row/col of the last one
                    if (col == 3) { row += 1; col = 0; }
                    else if (child == null) { col = 0; row = 0; }
                    else { col += 1; }

                    Grid.SetColumn(button, col);
                    Grid.SetRow(button, row);
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
            getTabsGrid(getCurrentTab(tabControl)).Children.Remove(button);
            this.jsm.removeGameFile(game);
        }

        private void onImage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Game game = Game.getGameFromButton(btn, games);
            game.launchGame();
        }
    }

    // thank you stack overflow
    public static class TupleExtensions
    {
        public static bool HasValue(this Tuple<string, string> tuple)
        {
            return !string.IsNullOrEmpty(tuple?.Item1) && !string.IsNullOrEmpty(tuple?.Item2);
        }
    }
}
