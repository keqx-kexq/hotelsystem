using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HotelSystem
{
    public partial class CreateBookingWindow : Window
    {
        private hotel_managementEntities _context;

        public CreateBookingWindow()
        {
            InitializeComponent();
            _context = new hotel_managementEntities();
            LoadRooms();
        }

        private void LoadRooms()
        {
           // Загрузка только свободных номеров [cite: 110]
            var availableRooms = _context.Rooms
                .Where(r => r.status == "Свободен")
                .Select(r => new { r.id, r.number })
                .ToList();

            RoomComboBox.ItemsSource = availableRooms;
            RoomComboBox.DisplayMemberPath = "number";
            RoomComboBox.SelectedValuePath = "id";
        }

        private void SaveBooking_Click(object sender, RoutedEventArgs e)
        {
           // Сбор данных и валидация 
            if (CheckInDatePicker.SelectedDate == null || CheckOutDatePicker.SelectedDate == null || RoomComboBox.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            int roomId = (int)RoomComboBox.SelectedValue;
            var room = _context.Rooms.FirstOrDefault(r => r.id == roomId);

            // Расчет цены и создание объектов 
            int nights = (CheckOutDatePicker.SelectedDate.Value - CheckInDatePicker.SelectedDate.Value).Days;
            decimal totalPrice = nights * (room?.price_per_night ?? 0);

            var newGuest = new Guests
            {
                first_name = GuestFirstNameTextBox.Text,
                last_name = GuestLastNameTextBox.Text,
                email = EmailTextBox.Text,
                phone = PhoneTextBox.Text,
                document_number = GuestDocumentNumberTextBox.Text
            };
            _context.Guests.Add(newGuest);
            _context.SaveChanges(); // Сначала сохраняем гостя, чтобы получить ID

            var reservation = new Reservations
            {
                guest_id = newGuest.id,
                room_id = roomId,
                check_in_date = CheckInDatePicker.SelectedDate.Value,
                check_out_date = CheckOutDatePicker.SelectedDate.Value,
                total_price = totalPrice,
                status = "Подтверждено"
            };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            MessageBox.Show($"Бронирование создано. Сумма: {totalPrice}");
            this.Close();
        }
    }
}