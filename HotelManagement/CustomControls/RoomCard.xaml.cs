using HotelManagement.View;
using HotelManagement.ViewModel;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                        color = new BrushConverter().ConvertFrom("#FCB7B7") as SolidColorBrush;
                        textcolor = new BrushConverter().ConvertFrom("#F56F6F") as SolidColorBrush;
                        break;
                    }
                case "Available":
                    {
                        color = new BrushConverter().ConvertFrom("#C2FCC1") as SolidColorBrush;
                        textcolor = new BrushConverter().ConvertFrom("#3AC291") as SolidColorBrush;
                        break;
                    }
                case "Occupied":
                    {
                        color = new BrushConverter().ConvertFrom("#FCFE7C") as SolidColorBrush;
                        textcolor = new BrushConverter().ConvertFrom("#BEC128") as SolidColorBrush;
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
