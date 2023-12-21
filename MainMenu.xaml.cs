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
using static WpfApp1.MainWindow;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        int roleID = UserData.RoleID;
        int clientID = UserData.UserID;
        string phone = UserData.Phone;
        string name = UserData.Name;
        string surname = UserData.Surname;

        public MainMenu()
        {
            InitializeComponent();
           SetWelcomeMessage(name, surname, roleID);

 
        }

        private void SetWelcomeMessage(string name, string surname, int roleID)
        {
            WelcomeText.Text = $"Добро пожаловать, {name} {surname} {roleID} !";

            // Проверка RoleID и отключение соответствующих кнопок
            if (roleID == 1)
            {
                AddRequestButton.IsEnabled = false;
            }
            else
            {
                AddRequestButton.IsEnabled = true;
            }
        }


        private void AddRequestButton_Click(object sender, RoutedEventArgs e)
        {
            AddRequest addRequestWindow = new AddRequest();
            addRequestWindow.Show();
            this.Close();

        }

        private void TrackOrderStatusButton_Click(object sender, RoutedEventArgs e)
        {
            TrackOrderStatus trackOrderStatus = new TrackOrderStatus();
            trackOrderStatus.Show();
            this.Close();
        }

        private void RequestListButton_Click(object sender, RoutedEventArgs e)
        {
            RequestList requestList = new RequestList();
            requestList.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }



    }
}
