using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Collections.Generic;

namespace HotelSystem
{
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            LoadUsers(); // Загрузка списка пользователей при старте [cite: 84]
        }

        private async void LoadUsers()
        {
            using (var context = new hotel_managementEntities())
            {
                var users = await context.Users.ToListAsync(); // Асинхронная загрузка [cite: 85]
                Users.ItemsSource = users;
            }
        }

        [cite_start]// Добавление пользователя через модальное окно [cite: 86]
        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var newUserWindow = new AddUserWindow();
            if (newUserWindow.ShowDialog() == true && newUserWindow.NewUser != null)
            {
                var newUser = newUserWindow.NewUser;
                using (var context = new hotel_managementEntities())
                {
                    if (await context.Users.AnyAsync(u => u.username == newUser.username))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.");
                    }
                    else
                    {
                        context.Users.Add(newUser);
                        await context.SaveChangesAsync();
                        LoadUsers();
                        MessageBox.Show("Пользователь успешно добавлен.");
                    }
                }
            }
        }

        [cite_start]// Разблокировка пользователя [cite: 87]
        private async void UnlockUser_Click(object sender, RoutedEventArgs e)
        {
            if (Users.SelectedItem is Users selectedUser)
            {
                using (var context = new hotel_managementEntities())
                {
                    var user = await context.Users.FindAsync(selectedUser.id);
                    if (user != null)
                    {
                        user.IsLocked = false;
                        user.LastLoginDate = null;
                        await context.SaveChangesAsync();
                        LoadUsers();
                        MessageBox.Show("Пользователь разблокирован.");
                    }
                }
            }
        }

        [cite_start]// Сохранение изменений в таблице [cite: 88]
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new hotel_managementEntities())
            {
                foreach (var user in Users.ItemsSource as IEnumerable<Users>)
                {
                    var existingUser = await context.Users.FindAsync(user.id);
                    if (existingUser != null)
                    {
                        existingUser.lastname = user.lastname;
                        existingUser.firstname = user.firstname;
                        existingUser.role = user.role;
                        existingUser.username = user.username;
                        existingUser.IsLocked = user.IsLocked;
                    }
                }
                await context.SaveChangesAsync();
                MessageBox.Show("Изменения сохранены.");
            }
        }
    }
}