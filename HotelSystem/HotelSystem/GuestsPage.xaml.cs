using System.Linq;
using System.Windows.Controls;

namespace HotelSystem
{
    public partial class GuestsPage : Page
    {
        private hotel_managementEntities _context;

        public GuestsPage()
        {
            InitializeComponent();
            _context = new hotel_managementEntities();
            LoadData();
        }

        private void LoadData()
        {
            [cite_start]// Загрузка всех гостей и привязка к DataGrid 
            var guests = _context.Guests.ToList();
            GuestsDataGrid.ItemsSource = guests;
        }
    }
}