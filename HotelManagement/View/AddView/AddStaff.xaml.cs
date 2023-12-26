using System.Windows;
using System.Windows.Input;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.View.AddView
{
    /// <summary>
    /// Interaction logic for AddStaff.xaml
    /// </summary>
    public partial class AddStaff : Window
    {
        public AddStaff(object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;

            (DataContext as StaffList).GenerateStaffId();
        }
        
        public AddStaff(string? id, object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;
            
            if(id != null)
                (DataContext as StaffList).GetStaffById(id);

        }

        private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddStaff_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddStaff_OnLoaded(object sender, RoutedEventArgs e)
        {
            if(BirthBox.SelectedDate == null)
                BirthBox.SelectedDate = DateTime.Now;
        }
    }
}
