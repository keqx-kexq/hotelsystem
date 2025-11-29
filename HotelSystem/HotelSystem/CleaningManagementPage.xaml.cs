using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity; // Обязательно добавьте для работы Include и запросов

namespace HotelSystem
{
    public partial class CleaningManagementPage : Page
    {
        private readonly hotel_managementEntities _context; // Контекст БД

        // Конструктор страницы
        public CleaningManagementPage()
        {
            InitializeComponent(); // Инициализация визуальных компонентов
            _context = new hotel_managementEntities(); // Подключение к БД
            LoadData(); // Загрузка сотрудников и таблицы расписания
            LoadRooms(); // Загрузка списка комнат
        }

        // 1. Метод загрузки данных сотрудников и расписания
        private void LoadData()
        {
            try
            {
                // --- Загрузка сотрудников (только Staff) в ComboBox ---
                var employees = _context.Users
                    .Where(u => u.role == "Staff")
                    .ToList()
                    .Select(u => new {
                        Id = u.id,
                        FullName = $"{u.lastname} {u.firstname}"
                    })
                    .ToList();

                CleaningOfficerComboBox.ItemsSource = employees;
                // DisplayMemberPath и SelectedValuePath заданы в XAML, но можно и тут продублировать, если нужно

                // --- Загрузка таблицы расписания уборок ---
                var cleaningSchedule = _context.Cleaning_Schedule
                    .Include("Rooms") // Подгружаем данные о комнате
                    .Include("Users") // Подгружаем данные о сотруднике
                    .ToList()
                    .Select(cs => new
                    {
                        cs.cleaning_date,
                        // Безопасное получение номера и имени (на случай удаления данных)
                        RoomNumber = cs.Rooms?.number.ToString() ?? "Нет данных",
                        UserFullName = $"{cs.Users?.lastname ?? ""} {cs.Users?.firstname ?? ""}",
                        cs.status
                    })
                    .ToList();

                CleaningScheduleGrid.ItemsSource = cleaningSchedule;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 2. Метод загрузки доступных номеров
        private void LoadRooms()
        {
            try
            {
                // Берем все номера (или можно отфильтровать только "Свободен", если нужно)
                var rooms = _context.Rooms
                    .Select(r => new { r.id, Number = r.number }) // Используем свойство Number для отображения
                    .ToList();

                RoomComboBox.ItemsSource = rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке комнат: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 3. Обработчик кнопки "Назначить уборку"
        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка, что всё выбрано
                if (CleaningOfficerComboBox.SelectedValue == null ||
                    RoomComboBox.SelectedValue == null ||
                    CleaningDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля (Сотрудник, Комната, Дата).", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Получение ID из выпадающих списков
                int selectedEmployeeId = (int)CleaningOfficerComboBox.SelectedValue;
                int selectedRoomId = (int)RoomComboBox.SelectedValue;
                DateTime selectedDate = CleaningDatePicker.SelectedDate.Value;

                // Создание новой записи
                var newSchedule = new Cleaning_Schedule
                {
                    user_id = selectedEmployeeId,
                    room_id = selectedRoomId,
                    cleaning_date = selectedDate,
                    status = "Запланировано" // Начальный статус
                };

                // Сохранение в БД
                _context.Cleaning_Schedule.Add(newSchedule);
                _context.SaveChanges();

                // Обновление таблицы, чтобы увидеть новую запись
                LoadData();

                MessageBox.Show("Уборка успешно назначена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}