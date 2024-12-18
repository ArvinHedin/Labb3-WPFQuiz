using Labb3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Labb3.Dialogs
{
    /// <summary>
    /// Interaction logic for CreateNewPackDialog.xaml
    /// </summary>
    public partial class CreateNewPackDialog : Window
    {
        public CreateNewPackDialog()
        {
            InitializeComponent();
            DifficultyComboBox.ItemsSource = Enum.GetValues(typeof(Difficulty));
        }

        public string PackName => PackNameTextBox.Text;
        private Difficulty _difficulty;

        public Difficulty Difficulty
        {
            get => _difficulty;
            set 
            { 
                _difficulty = value;
                
            }
        }

        public int TimeLimit => int.Parse(TimeLimitTextBox.Text);
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PackName))
            {
                MessageBox.Show("Please enter a pack name.", "Validation Error");
                return;
            }

            if (!int.TryParse(TimeLimitTextBox.Text, out int timeLimit) || timeLimit <= 0)
            {
                MessageBox.Show("Please enter a valid time limit (positive number).",
                              "Validation Error");
                return;
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}