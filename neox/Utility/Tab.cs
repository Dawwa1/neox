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

            //tabControl.Items.Add(Item);
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

            //var selected_item = this.TabControl.SelectedItem as TabItem;
            //
            //return new Tab(this.TabControl, selected_item.Header.ToString(), selected_item.Content as Grid);

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
