using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Timer = System.Timers.Timer;

namespace HotelManagement.CustomControls.MessageBox
{
    public partial class MessageBox : Window, INotifyPropertyChanged
    {
        #region Private Fields

        private List<CancelEventHandler> _preCloseEvents = new List<CancelEventHandler>();

        private object _buttonContent = null;

        private bool _isCloseable = true;

        private string _caption;
        private string _message;
        private string _okText = "Yes";
        private string _cancelText = "No";

        private MessageBoxButton _boxButton = MessageBoxButton.OK;
        private MessageBoxImage _boxImage = MessageBoxImage.NONE;

        #endregion

        #region Private Methods

        private bool RaiseCloseEvent()
        {
            var args = new CancelEventArgs();
            foreach (var preCloseEvent in _preCloseEvents)
            {
                preCloseEvent(this, args);

                if (args.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Private Events

        private void Event_HideAnimation_Completed(object sender, EventArgs e)
        {
            if (_isCloseable)
            {
                return;
            }

            Close();
        }

        private void Event_Click_Button(object sender, RoutedEventArgs e)
        {
            if (!_isCloseable)
            {
                return;
            }

            if (sender == _OkButton)
            {
                Result = MessageBoxResult.OK;
            }

            Close();
        }

        private void Event_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Event_Closing(object sender, CancelEventArgs e)
        {
            var hideAnimation = this.FindResource("HideAnimation") as Storyboard;
            if (hideAnimation == null || !_isCloseable)
            {
                return;
            }

            e.Cancel = true;

            if (!RaiseCloseEvent())
            {
                _isCloseable = true;
                return;
            }

            _isCloseable = false;

            hideAnimation.Completed += Event_HideAnimation_Completed;
            hideAnimation.Begin(_Dialog);
        }

        #endregion

        public MessageBox()
        {
            InitializeComponent();

            _CancelButton.Focus();
        }

        #region Public Properties

        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;

                Title = _caption;

                NotifyPropertyChanged("Caption");
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;

                NotifyPropertyChanged("Message");
            }
        }

        public MessageBoxButton MsgButton
        {
            get => _boxButton;
            set
            {
                _boxButton = value;

                NotifyPropertyChanged("MsgButton");
            }
        }

        public MessageBoxImage MsgImage
        {
            get => _boxImage;
            set
            {
                _boxImage = value;

                NotifyPropertyChanged("MsgImage");
            }
        }

        public MessageBoxResult Result { get; private set; } = MessageBoxResult.CANCEL;

        public string OkText
        {
            get => _okText;
            set
            {
                _okText = value;

                NotifyPropertyChanged("OkText");
            }
        }

        public string CancelText
        {
            get => _cancelText;
            set
            {
                _cancelText = value;

                NotifyPropertyChanged("CancelText");
            }
        }

        public object ButtonContent
        {
            get => _buttonContent;
            set
            {
                _buttonContent = value;

                NotifyPropertyChanged("ButtonContent");
            }
        }

        public event CancelEventHandler PreClose
        {
            add => _preCloseEvents.Add(value);
            remove => _preCloseEvents.Remove(value);
        }

        #endregion

        #region Public Methods

        public static MessageBoxResult Show(Window owner, string caption, string content, MessageBoxButton msgButton = MessageBoxButton.OK,
            MessageBoxImage msgImage = MessageBoxImage.NONE)
        {
            var alert = new MessageBox()
            {
                Caption = caption,
                Message = content,
                MsgButton = msgButton,
                MsgImage = msgImage,
                OkText = msgButton == MessageBoxButton.OK ? "OK" : "Yes",
                ShowInTaskbar = false,
                Owner = owner
            };

            return alert.ShowDialog(msgButton);
        }

        public MessageBoxResult ShowDialog(MessageBoxButton msgButton)
        {
            if(msgButton == MessageBoxButton.OK)
            {
                var time1 = new Timer();
                time1.Elapsed += ((sender, args) => Application.Current.Dispatcher.Invoke(Close));
                time1.Interval = 3000; 
                time1.Enabled = true;
            }
            
            base.ShowDialog();
            
            return Result;
        }
        
        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}