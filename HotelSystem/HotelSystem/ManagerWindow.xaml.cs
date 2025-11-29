using System.Windows;

namespace HotelSystem
{
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
            [cite_start]// Навигация по фреймам к соответствующим страницам 
            GuestsFrame.Navigate(new GuestsPage());
            RoomsFrame.Navigate(new RoomsPage());
            BookingsFrame.Navigate(new BookingPage());
            CleaningManagementFrame.Navigate(new CleaningManagementPage());
        }
    }
}