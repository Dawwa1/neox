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

        public List<Program> progs = new List<Program>();
        public JsonStorageManager jsm;
        

        public MainWindow()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\manifests";
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            jsm = new JsonStorageManager(path);
            foreach (Program prog in JsonStorageManager.GetAllPrograms(jsm))
            {
                Tab tab = GetCurrentTab(tabControl);
                if (Int16.Parse(prog.Tab) > tab.HeaderAsInt())
                {
                    while (tab.HeaderAsInt() < Int16.Parse(prog.Tab))
                    {
                        tab = CreateNewTab(tabControl);
                    }
                }

                DrawProgramButton(prog, tab);
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

                Debug.WriteLine(name);
                Debug.WriteLine(path);
            }

            return Tuple.Create(name, path);
        }

        private Tab GetCurrentTab(TabControl tc)
        {
            TabItem tab;
            if (tabControl.SelectedItem != null) { tab = (TabItem)tc.SelectedItem; }
            else { tab = (TabItem)tc.Items[0]; }

            return new Tab(tabControl, tab);
        }

        private Tab CreateNewTab(TabControl tc)
        {
            var lastTab = (TabItem)tc.Items[^1];
            int header = Int16.Parse(new Tab(tabControl, lastTab).Header) + 1;
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

            var t = new Tab(tabControl, new TabItem() { Header = header.ToString(), Content=grid });
            tc.Items.Add(t);
            Tab.Tab_List.Add(t);

            return t;
        }

        private void DrawProgramButton(Program prog, Tab tab)
        {
            // Creates button for program
            Button button = new Button
            {
                Content = prog.Name,
                ContextMenu = new ContextMenu(),
            };
            button.Click += new RoutedEventHandler(onImage_Click);
            MenuItem deleteMi = new MenuItem();
            deleteMi.Header = "Delete";
            deleteMi.Click += deleteMi_Click;
            button.ContextMenu.Items.Add(deleteMi);

            UIElement? child = null;
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
            prog.ButtonColumn = col;
            prog.ButtonRow = row;

            tab.AddButton(button);
            progs.Add(prog);
            this.jsm.AddProgram(prog);
        }

        private void deleteMi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            Button button = (Button)contextMenu.PlacementTarget;

            Debug.WriteLine(Grid.GetColumn(button));
            Debug.WriteLine(Grid.GetRow(button));

            Program prog = Program.GetProgramFromButton(button, progs);

            Tab tab = GetCurrentTab(tabControl);

            progs.Remove(prog);
            tab.RemoveButton(button);
            this.jsm.RemoveProgramFile(prog);

            if (tab.Content.Children.Count == 0)
            {
                tabControl.Items.Remove(tab.Item);
            }
        }

        private void onImage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Program prog = Program.GetProgramFromButton(btn, progs);
            prog.Launch();
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
