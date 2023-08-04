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

namespace stickycardsv2.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для TimerView.xaml
    /// </summary>
    public partial class TimerView : Window
    {
        public TimerView()
        {
            InitializeComponent();
            TimeType.ItemsSource = new string[]
            {
                "sec",
                "mins",
                "hours"
            };
            TimeType.SelectedIndex = 0;

            SoundTypeBox.ItemsSource = new string[]
            {
                nameof(System.Media.SystemSounds.Asterisk),
                nameof(System.Media.SystemSounds.Beep),
                nameof(System.Media.SystemSounds.Exclamation),
                nameof(System.Media.SystemSounds.Hand),
                nameof(System.Media.SystemSounds.Question),
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
