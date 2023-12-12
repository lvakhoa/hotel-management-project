using HotelManagement.ViewModel;
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

        private void _RoomCard_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush? color = new BrushConverter().ConvertFrom("#E4FFE0") as SolidColorBrush;
            switch(Status)
            {
                case "Out of Order":
                    {
                        color = new BrushConverter().ConvertFrom("#FFCDCD") as SolidColorBrush;
                        break;
                    }
                case "Available":
                    {
                        color = new BrushConverter().ConvertFrom("#E4FFE0") as SolidColorBrush;
                        break;
                    }
                case "Occupied":
                    {
                        color = new BrushConverter().ConvertFrom("#F7F990") as SolidColorBrush;
                        break;
                    }
            }
            BorderCardStyle.Background = color;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            menu.Items.Add("Edit");
            menu.Items.Add("Remove");
            menu.IsOpen = true;
        }
    }
}
