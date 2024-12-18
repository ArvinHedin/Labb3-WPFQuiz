using Labb3.Model;
using System.Windows;

namespace Labb3.Dialogs
{
    public partial class PackOptionsDialog : Window
    {
        private QuestionPack _pack;

        public PackOptionsDialog(QuestionPack pack)
        {
            InitializeComponent();
            _pack = pack;

            DifficultyComboBox.ItemsSource = System.Enum.GetValues(typeof(Difficulty));

            PackNameTextBox.Text = pack.Name;
            DifficultyComboBox.SelectedItem = pack.Difficulty;
            TimeLimitTextBox.Text = pack.TimeLimit.ToString();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PackNameTextBox.Text))
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

            _pack.Name = PackNameTextBox.Text;
            _pack.Difficulty = (Difficulty)DifficultyComboBox.SelectedItem;
            _pack.TimeLimit = timeLimit;

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}