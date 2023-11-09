using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace neox.Utility
{
    internal class Tab
    {

        static readonly int MAX_GAME_COUNT = 4;
        public TabControl TabControl {  get; set; }
        public string Header { get; set; }
        public Grid? Content { get; set; }
        public TabItem? Item { get; set; }
        public static List<Tab>? Tab_List = new List<Tab>();

        public Tab(TabControl tabControl, string header, Grid? content = null)
        {
            TabControl = tabControl;
            Header = header;
            Content = content;
            Item = new TabItem()
            {
                Header = header,
                Content = content
            };
        }

        public static bool IsFull(Grid g)
        {
            return g.Children.Count >= Tab.MAX_GAME_COUNT;
        }

        public static Tab FindTabByHeader(string header)
        {
            foreach (Tab tab in Tab_List) { if (tab.Header == header) { return tab; } }
            return null;
        }

        public void FocusOnTab(int tabIndex=0)
        {
            if (tabIndex != 0)
            {
                this.TabControl.SelectedIndex = tabIndex;
            }
            else
            {
                this.TabControl.SelectedIndex = this.TabControl.Items.Count - 1;
            }

        }

        public void AddButton(Button button)
        {
            Content?.Children.Add(button);
        }

        public void RemoveButton(Button button)
        {
            Content?.Children.Remove(button);
        }

        public Int16 HeaderAsInt()
        {
            return Int16.Parse(Header);
        }

        public Grid GetContent()
        {
            return Content;
        }

        public bool IsFull()
        {
            return GetContent().Children.Count >= Tab.MAX_GAME_COUNT;
        }
    }
}
