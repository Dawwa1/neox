using System;
using System.Windows;

namespace neox
{
    public partial class NameInput : Window
    {
        public NameInput(string defaultAnswer = "")
        {
            InitializeComponent();
            txtName.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtName.SelectAll();
            txtName.Focus();
        }

        public string Answer
        {
            get { return txtName.Text; }
        }
    }
}