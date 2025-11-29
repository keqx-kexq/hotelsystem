using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HotelSystem
{
    public partial class RoomsPage : Page
    {
        private hotel_managementEntities _context;

        public RoomsPage()
        {
            InitializeComponent();
            _context = new hotel_managementEntities();
            LoadRoomsAndStatistics();
        }

        private void LoadRoomsAndStatistics()
        {
            var rooms = _context.Rooms.ToList();
            RoomsDataGrid.ItemsSource = rooms;

            [cite_start]// Расчет статистики 
            int totalRooms = rooms.Count;
            int occupiedRooms = rooms.Count(r => r.status == "Занят");
            double occupancyPercentage = totalRooms > 0 ? (double)occupiedRooms / totalRooms * 100 : 0;

            OccupancyPercentageText.Text = $"Загрузка: {occupancyPercentage:F2}%";
        }

        private void RefreshRooms_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomsAndStatistics(); // Перезагрузка данных [cite: 103]
        }
    }
}