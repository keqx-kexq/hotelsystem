using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
// using System.Data.Entity; // Обязательно для .Include

namespace HotelSystem
{
    public partial class BookingPage : Page
    {
        public BookingPage()
        {
            InitializeComponent();
            LoadBookings();
        }

        private void LoadBookings()
        {
            using (var context = new hotel_managementEntities())
            {
                // Загрузка с включением связанных таблиц Guests и Rooms 
                var bookings = context.Reservations
                    .Include("Guests")
                    .Include("Rooms")
                    .ToList();

                // Проецирование в анонимный тип для красивого отображения в таблице 
                var selectedBookings = bookings.Select(r => new
                {
                    r.id,
                    FullName = r.Guests != null ? $"{r.Guests.first_name} {r.Guests.last_name}" : "Нет данных",
                    RoomNumber = r.Rooms != null ? r.Rooms.number.ToString() : "Нет данных",
                    r.check_in_date,
                    r.check_out_date,
                    r.total_price,
                    r.status
                }).ToList();

                BookingsDataGrid.ItemsSource = selectedBookings;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна создания брони [cite: 107]
            CreateBookingWindow createBookingWindow = new CreateBookingWindow();
            createBookingWindow.ShowDialog();
            LoadBookings(); // Обновление после закрытия
        }
    }
}