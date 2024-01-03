using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotelManagement.CustomControls
{
    /// <summary>
    /// Interaction logic for RoomCard.xaml
    /// </summary>
    public partial class RoomCard : UserControl
    {
        public RoomCard()
        {
            InitializeComponent();
        }
        public string RoomNumber
        {
            get { return (string)GetValue(RoomNumberProperty); }
            set { SetValue(RoomNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoomNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoomNumberProperty =
            DependencyProperty.Register("RoomNumber", typeof(string), typeof(RoomCard));

        public string RoomID
        {
            get { return (string)GetValue(RoomIDProperty); }
            set { SetValue(RoomIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoomID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoomIDProperty =
            DependencyProperty.Register("RoomID", typeof(string), typeof(RoomCard));
        
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(RoomCard));

        public int Floor
        {
            get { return (int)GetValue(FloorProperty); }
            set { SetValue(FloorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Floor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FloorProperty =
            DependencyProperty.Register("Floor", typeof(int), typeof(RoomCard), new PropertyMetadata(0));

        public string Capacity
        {
            get { return (string)GetValue(CapacityProperty); }
            set { SetValue(CapacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Capacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CapacityProperty =
            DependencyProperty.Register("Capacity", typeof(string), typeof(RoomCard));

        public string RoomType
        {
            get { return (string)GetValue(RoomTypeProperty); }
            set { SetValue(RoomTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoomType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoomTypeProperty =
            DependencyProperty.Register("RoomType", typeof(string), typeof(RoomCard));

        private void _RoomCard_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush? color = new BrushConverter().ConvertFrom("#E4FFE0") as SolidColorBrush;
            SolidColorBrush? textcolor = new BrushConverter().ConvertFrom("#E4FFE0") as SolidColorBrush;
            switch (Status)
            {
                case "Out of Order":
                    {
                        color = Application.Current.Resources["OutOfOrderColor"] as SolidColorBrush;
                        textcolor = Application.Current.Resources["OutOfOrderTextColor"] as SolidColorBrush;
                        break;
                    }
                case "Available":
                    {
                        color = Application.Current.Resources["AvailableColor"] as SolidColorBrush;
                        textcolor = Application.Current.Resources["AvailableTextColor"] as SolidColorBrush;
                        break;
                    }
                case "Occupied":
                    {
                        color = Application.Current.Resources["OccupiedColor"] as SolidColorBrush;
                        textcolor = Application.Current.Resources["OccupiedTextColor"] as SolidColorBrush;
                        break;
                    }
            }
            BorderCardStyle.Background = color;
            TypeTxt.Foreground = textcolor;
            CapacityTxt.Foreground = textcolor;
            StatusTxt.Foreground = textcolor;
            FloorTxt.Foreground = textcolor;
        }
    }
}
