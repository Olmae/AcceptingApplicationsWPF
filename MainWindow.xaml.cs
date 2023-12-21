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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Diagnostics;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static class UserData
        {
            public static string Login { get; set; }
            public static string Name { get; set; }
            public static string Surname { get; set; }
            public static int RoleID { get; set; }
            public static string Password { get; set; }
            public static string Phone { get; set; }
            public static int UserID { get; set; }
        }


        public MainWindow()
        {
            InitializeComponent();

            // Проверяем, есть ли сохраненный логин и пароль
            if (Application.Current.Properties.Contains("SavedLogin") && Application.Current.Properties.Contains("SavedPassword"))
            {
                string savedLogin = Application.Current.Properties["SavedLogin"].ToString();
                string savedPassword = Application.Current.Properties["SavedPassword"].ToString();


                // Устанавливаем сохраненные значения в соответствующие поля
                LoginTextBox.Text = savedLogin;
                PasswordBox.Password = savedPassword;
                RememberCheckBox.IsChecked = true; // Помечаем галочку "Запомнить пароль"
            }
        }



        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;
            bool rememberPassword = RememberCheckBox.IsChecked ?? false;

            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();

                string query = "SELECT * FROM dbo.[User] WHERE Login=@Login AND Password=@Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    string surname = reader["Surname"].ToString();
                    int roleID = Convert.ToInt32(reader["RoleID"]);
                    int userID = Convert.ToInt32(reader["UserID"]);
                    string phone = reader["Phone"].ToString();

                    UserData.Login = login;
                    UserData.Name = name;
                    UserData.Surname = surname;
                    UserData.RoleID = roleID;
                    UserData.Password = password;
                    UserData.Phone = phone;
                    UserData.UserID = userID;

                    if (rememberPassword)
                    {
                        Application.Current.Properties["SavedLogin"] = login;
                        Application.Current.Properties["SavedPassword"] = password;
                    }

                    MainMenu mainMenu = new MainMenu();
                    mainMenu.Show();
                    this.Close();
                    Debug.WriteLine($"Debug: UserID = {UserData.UserID}");

                }
                else
                {
                    ErrorMessage.Text = "Неверный логин или пароль. Попробуйте снова.";
                    ErrorMessage.Visibility = Visibility.Visible;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при подключении к базе данных: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private string GetClientInfo(string login)
        {
            // Здесь нужно получить информацию о клиенте используя логин
            // Верните строку с информацией о клиенте
            return "Информация о клиенте";

        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

    }
}
