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
                Tab tab = getCurrentTab(tabControl);
                if (int16(game.Tab) > tab.HeaderAsInt())
                {
                    while (tab.HeaderAsInt() < int16(game.Tab))
                    {
                        tab = createNewTab(tabControl);
                    }
                }

                addGame(game, tab);
            }
        }

        private static Int16 int16(string i)
        {
            if (Int32.Parse(i) <= Int16.MaxValue && Int32.Parse(i) != 0) { return Int16.Parse(i); }
            return Int16.MaxValue;
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

                Debug.WriteLine(name);
                Debug.WriteLine(path);
            }

            return Tuple.Create(name, path);
        }

        private Tab getCurrentTab(TabControl tc)
        {
            TabItem tab;
            if (tabControl.SelectedItem != null) { tab = tc.SelectedItem as TabItem; }
            else { tab = tc.Items[0] as TabItem; }

            return new Tab(tabControl, tab.Header.ToString(), tab.Content as Grid);
        }

        private Tab createNewTab(TabControl tc)
        {
            var lastTab = tc.Items[^1] as TabItem;
            int header = int16(lastTab.Header.ToString()) + 1;
            Grid grid = new Grid();

            for (int i = 0; i < 8; i++)
            {
                if (i < 4)
                {
                    grid.RowDefinitions.Add(new RowDefinition());
                }
                else
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }
            }

            var t = new Tab(tabControl, header.ToString(), grid);
            tc.Items.Add(t.Item);
            Tab.Tab_List.Add(t);

            return t;
        }

        // TODO: Save game to whichever tab it was added to; --Fix buttons on extra tabs--
        private void addGame_Click(object sender, RoutedEventArgs e)
        {
            var fd = ofd();

            if (fd.HasValue())
            {
                // ~~very bad, dont feel like fixing~~
                // edit: a little less bad then previously
                string name = fd.Item1, path = fd.Item2;

                if (File.Exists(jsm.JsonFolderPath + "\\" + name + ".json")) { MessageBox.Show("Application by that name exists!"); return; }

                Game game = new Game(name, path, tab: getCurrentTab(tabControl).Header);

                NameInput nameInputDialog = new NameInput(game.Name);
                if (nameInputDialog.ShowDialog() == true) { game.Name = nameInputDialog.Answer; }

                addGame(game, getCurrentTab(tabControl));
            }
        }

        private void addGame(Game game, Tab tab)
        {
            //Adds a game to Grid

            // Tab if tab is full
            foreach (Tab t in Tab.Tab_List) 
            {
                if (t.IsFull())
                {
                    tab = createNewTab(tabControl);
                    tab.FocusOnTab();
                }
            }

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

            UIElement child = null;
            int row = 0;
            int col = 0;
            if (tab.Content.Children.Count > 0)
            {
                child = tab.Content.Children[tab.Content.Children.Count - 1];
                row = Grid.GetRow(child);
                col = Grid.GetColumn(child);
            }

            // increments the next added program based on the row/col of the last one
            if (col == 3) { row += 1; col = 0; }
            else if (child != null) { col += 1; }

            Grid.SetColumn(button, col);
            Grid.SetRow(button, row);
            game.buttonColumn = col;
            game.buttonRow = row;

            tab.AddButton(button);
            games.Add(game);
            this.jsm.addNewGameFile(game);
        }

        private void deleteMi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            Button button = (Button)contextMenu.PlacementTarget;

            Debug.WriteLine(Grid.GetColumn(button));
            Debug.WriteLine(Grid.GetRow(button));

            Game game = Game.getGameFromButton(button, games);

            Tab tab = getCurrentTab(tabControl);

            games.Remove(game);
            tab.RemoveButton(button);
            this.jsm.removeGameFile(game);

            if (tab.Content.Children.Count == 0)
            {
                tabControl.Items.Remove(tab.Item);
            }
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
