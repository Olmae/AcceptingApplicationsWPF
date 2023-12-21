using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string name = NameTextBox.Text;
            string surname = SurnameTextBox.Text;
            string password = PasswordBox.Password;
            string phone = PhoneTextBox.Text;
            int clientID = GenerateRandomClientID();
            int roleID = 1;
            bool success = AddUserToDatabase(login, name, surname, password, phone, clientID, roleID);

            if (success)
            {
                MessageBox.Show("Регистрация успешна, добро пожаловать," + name + " " + surname);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private bool AddUserToDatabase(string login, string name, string surname, string password, string phone, int clientID, int roleID)
        {
            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();

                string query = "INSERT INTO dbo.[User] (Login, Name, Surname, Password, Phone, UserID, RoleID) VALUES (@Login, @Name, @Surname, @Password, @Phone, @UserID, @RoleID)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Surname", surname);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@UserID", clientID);
                command.Parameters.AddWithValue("@RoleID", roleID);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении пользователя: " + ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
        private int GenerateRandomClientID()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }
    }
}
