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
using System.Windows.Shapes;

namespace Tableware
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        public static class TempData
        {
            public static int idUser { get; set; }
            public static string idSelectedProduct { get; set; }
            public static int enterMode { get; set; }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            using (var dataBase = new TradeEntities())
            {
                var userData = (from user in dataBase.User
                                join
                                role in dataBase.Role on user.UserRole equals role.RoleID
                                where (user.UserLogin  == Login.Text && user.UserPassword == Password.Text)
                                select new
                                {
                                    user.UserID,
                                    role.RoleName,
                                }).FirstOrDefault();

                if (userData != null)
                {
                    TempData.idUser = userData.UserID;
                    ProductPanel productPanel = new ProductPanel();

                    switch (userData.RoleName)
                    {
                        case "Клиент":
                            TempData.enterMode = 1;
                            productPanel.Show();
                            this.Hide();
                            break;

                        case "Менеджер":
                            TempData.enterMode = 2;
                            productPanel.Show();
                            this.Hide();
                            break;

                        case "Администратор":
                            TempData.enterMode = 3;
                            productPanel.Show();
                            this.Hide();
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }

        private void GustEnter_Click(object sender, RoutedEventArgs e)
        {
            ProductPanel productPanel = new ProductPanel();
            TempData.enterMode = -1;
            productPanel.Show();
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
