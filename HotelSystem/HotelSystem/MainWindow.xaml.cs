using System;
using System.Linq;
using System.Windows;
using System.Data.Entity; // Не забудьте добавить это

namespace HotelSystem
{
    public partial class MainWindow : Window
    {
        [cite_start]// Конструктор выполняет инициализацию окна [cite: 79]
        public MainWindow()
        {
            InitializeComponent();
        }

        [cite_start]// Обработчик кнопки входа: проверка логина/пароля, ролей и блокировок 
        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            using (var context = new hotel_managementEntities())
            {
                var user = await context.Users
                    .Where(u => u.username == username)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    MessageBox.Show("Неправильный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.IsLocked.HasValue && user.IsLocked.Value)
                {
                    MessageBox.Show("Вы заблокированы, обратитесь к администратору.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                [cite_start]// Автоматическая блокировка при отсутствии более 30 дней (кроме админов) 
                if (user.LastLoginDate.HasValue && (DateTime.Now - user.LastLoginDate.Value).TotalDays > 30 && user.role != "Admin")
                {
                    user.IsLocked = true;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Ваша учетная запись заблокирована из-за длительного отсутствия", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.password == password)
                {
                    [cite_start]// Успешный вход: сброс счетчика ошибок и вход 
                    user.LastLoginDate = DateTime.Now;
                    user.FailedLoginAttempts = 0;
                    await context.SaveChangesAsync();

                    [cite_start]// Проверка смены пароля при первом входе 
                    if (user.isFirstLogin.HasValue && user.isFirstLogin.Value)
                    {
                        ChangePassword changePass = new ChangePassword(user.id);
                        changePass.Owner = this;
                        changePass.ShowDialog();
                    }
                    else
                    {
                        [cite_start]// Распределение по ролям 
                        if (user.role == "Admin")
                        {
                            new Admin().Show();
                        }
                        else if (user.role == "Management")
                        {
                            new ManagerWindow().Show();
                        }
                        else if (user.role == "Staff")
                        {
                            new CleaningStaffScheduleWindow(user.id).Show();
                        }
                        this.Close();
                    }
                }
                else
                {
                    [cite_start]// Неудачная попытка: счетчик +1, блокировка после 3 неудач 
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts == 3)
                    {
                        user.IsLocked = true;
                        MessageBox.Show("Вы заблокированы после 3 неудачных попыток.", "Блокировка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        int attemptsLeft = 3 - (user.FailedLoginAttempts ?? 0);
                        MessageBox.Show($"Неправильный пароль. Осталось попыток: {attemptsLeft}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}