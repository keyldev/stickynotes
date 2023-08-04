using Microsoft.Win32;
using stickycardsv2.Core;
using stickycardsv2.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace stickycardsv2.MVVM.ViewModel
{
    internal class CardViewModel : ObservableObject
    {
        private ObservableCollection<object>? _itemsList;

        public ObservableCollection<object>? ItemsList
        {
            get { return _itemsList; }
            set { _itemsList = value; NotifyPropertyChanged(); }
        }
        private bool _isPinned = false;

        public bool IsPinned
        {
            get { return _isPinned; }
            set
            {
                _isPinned = value;
                if (_isPinned)
                {
                    CardTitle = "StickyCard (pinned)";
                }
                else CardTitle = "StickyCard (unpinned)";
                NotifyPropertyChanged();
            }
        }

        private string? _cardTitle;

        public string? CardTitle
        {
            get { return _cardTitle; }
            set
            {
                _cardTitle = value;
                NotifyPropertyChanged();
            }
        }

        private string _headerColor = "#3eb489";
        private DispatcherTimer? notifyTimer;

        public string HeaderColor
        {
            get { return _headerColor; }
            set { _headerColor = value; NotifyPropertyChanged(); }
        }

        private int soundType = 0;
        private string soundPath = ""; // if custom sound


        public RelayCommand? OpenSettingsCommand { get; set; }
        public RelayCommand? AddNewCardCommand { get; set; }
        public RelayCommand? PinCardCommand { get; set; }
        public RelayCommand? AddTimerCommand { get; set; }
        public RelayCommand? AddTextBoxCommand { get; set; }
        public RelayCommand? SetItalicText { get; set; }
        public RelayCommand? SetUnderlineText { get; set; }
        public RelayCommand? AddImage { get; set; }
        public RelayCommand? SetFontWeight { get; set; }

        public CardViewModel()
        {
            ItemsList = new ObservableCollection<object>();
            PinCardCommand = new RelayCommand(o => IsPinned = IsPinned ? false : true);
            AddNewCardCommand = new RelayCommand(o =>
            {
                CardView card = new CardView();
                card.Show();

            });
            AddTimerCommand = new RelayCommand(o =>
            {
                TimerView timer = new TimerView();
                timer.Owner = App.Current.MainWindow;
                App.Current.MainWindow.Effect = new BlurEffect() { Radius = 3 };
                if (timer.ShowDialog() == true)
                {
                    notifyTimer = new DispatcherTimer();

                    notifyTimer.Interval = timer.TimeType.SelectedIndex switch
                    {
                        0 => new TimeSpan(0, 0, int.Parse(timer.TimeCount.Text)),
                        1 => new TimeSpan(0, int.Parse(timer.TimeCount.Text), 0),
                        2 => new TimeSpan(int.Parse(timer.TimeCount.Text), 0, 0),
                        _ => new TimeSpan(0, 0, int.Parse(timer.TimeCount.Text))
                    };
                    notifyTimer.Tick += Timer_Tick;
                    notifyTimer.Start();
                    soundType = timer.SoundTypeBox.SelectedIndex;

                    //MessageBox.Show(timer.TimeType.SelectedItem.ToString());
                }

                App.Current.MainWindow.Effect = null;
            });
            OpenSettingsCommand = new RelayCommand(o =>
            {
                SettingsView settings = new SettingsView();
                settings.Owner = App.Current.MainWindow;
                App.Current.MainWindow.Effect = new BlurEffect() { Radius = 3 };
                settings.ShowDialog(); // add the settings content (from trello)
                App.Current.MainWindow.Effect = null;


            });
            AddTextBoxCommand = new RelayCommand(o =>
            {
                ItemsList.Add(new RichTextBox()
                {
                    BorderThickness = new Thickness(0),
                    Background = null,
                    Foreground = Brushes.White,
                    ToolTip = "Введите текст",
                    CaretBrush = Brushes.White,
                    FontSize = 14,
                    Document = new FlowDocument(new Paragraph(new Run("Type.."))),
                });

            });
            // курсив
            SetItalicText = new RelayCommand((o) =>
            {
                RichTextBox? textSelection = null;

                foreach (var item in ItemsList)
                {
                    if (item is RichTextBox)
                        textSelection = (RichTextBox)item;
                }
                TextSelection? text = textSelection?.Selection;
                if (!text.IsEmpty)
                {
                    try
                    {
                        text.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            });
            // подчеркнутый текст
            SetUnderlineText = new RelayCommand((o) =>
            {
                RichTextBox? textSelection = null;
                foreach (var item in ItemsList)
                {
                    if (item is RichTextBox)
                        textSelection = (RichTextBox)item;
                }
                TextSelection? text = (TextSelection)textSelection?.Selection;
                if (!text.IsEmpty)
                {
                    text.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
                }
            });
            // картинка
            AddImage = new RelayCommand((o) =>
            {
                string? pathToImage = null;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();
                pathToImage = openFileDialog.FileName;
                if (pathToImage == String.Empty) return;

                ItemsList.Add(new Image()
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(pathToImage)),
                });
            });
            // жирность 
            SetFontWeight = new RelayCommand(o =>
            {
                RichTextBox? textSelection = null;
                foreach (var item in ItemsList)
                {
                    if (item is RichTextBox)
                        textSelection = (RichTextBox)item;
                }
                TextSelection? text = textSelection?.Selection;
                if (!text.IsEmpty)
                {
                    try
                    {
                        text.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            });
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            NotificationWindow notification = new NotificationWindow();
            notification.Show();
            switch (soundType)
            {
                case 0: 
                    System.Media.SystemSounds.Asterisk.Play(); break;
                case 1:
                    System.Media.SystemSounds.Beep.Play(); break;
                case 2:
                    System.Media.SystemSounds.Exclamation.Play(); break;
                case 3:
                    System.Media.SystemSounds.Hand.Play(); break;
                case 4: 
                    System.Media.SystemSounds.Question.Play(); break;
            }

            notifyTimer?.Stop();
        }
    }
}
