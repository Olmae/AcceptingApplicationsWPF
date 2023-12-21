using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using static WpfApp1.MainWindow;

namespace WpfApp1
{
    public partial class AddRequest : Window
    {
        public AddRequest()
        {
            InitializeComponent();

            if (UserData.RoleID == 2)
            {
                FillResponsibleComboBox();
            }
        }

        public class UserInfo
        {
            public int UserID { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int RoleID { get; set; }
        }

        private void FillResponsibleComboBox()
        {
            ResponsibleComboBox.Items.Clear();

            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Name, Surname FROM dbo.[User] WHERE RoleID = 1";
                SqlCommand command = new SqlCommand(query, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string responsibleName = reader["Name"].ToString();
                    string responsibleSurname = reader["Surname"].ToString();

                    ResponsibleComboBox.Items.Add($"{responsibleName} {responsibleSurname}");
                }

                reader.Close();
            }
        }

        private int GetSelectedSpecialistID()
        {
            UserInfo selectedSpecialist = GetSelectedSpecialistInfo();
            return selectedSpecialist != null ? selectedSpecialist.UserID : 0;
        }

        private void ClearFormFields()
        {
            EquipmentTextBox.Clear();
            IssueTypeTextBox.Clear();
            ClientInfoTextBox.Clear();
            ResponsibleComboBox.SelectedIndex = -1;
            // Clear other fields as needed
        }

        private int GenerateRequestNumber()
        {
            // Используем класс Random для генерации случайного числа
            Random random = new Random();

            // Генерируем случайное число и конвертируем его в строку
            int randomNumber = random.Next(100000, 999999);


            // Сочетаем случайное число и текущее время для создания уникального номера
            int requestNumber = randomNumber;

            return requestNumber;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполненность полей
            if (string.IsNullOrWhiteSpace(EquipmentTextBox.Text) ||
                string.IsNullOrWhiteSpace(IssueTypeTextBox.Text) ||
                string.IsNullOrWhiteSpace(ClientInfoTextBox.Text) ||
                StatusComboBox.SelectedItem == null ||
                (UserData.RoleID == 2 && ResponsibleComboBox.SelectedItem == null))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля и выберите значения для комбобоксов.");
                return;
            }

            // Проверка на корректность дат
            if (StartDatePicker.SelectedDate == null || (EndDatePicker.SelectedDate != null && EndDatePicker.SelectedDate < StartDatePicker.SelectedDate))
            {
                MessageBox.Show("Пожалуйста, выберите корректные даты.");
                return;
            }

            int requestNumber = GenerateRequestNumber();
            DateTime currentTime = DateTime.Now;

            string equipment = EquipmentTextBox.Text;
            string issueType = IssueTypeTextBox.Text;
            string clientInfo = ClientInfoTextBox.Text;
            string description = DescriptionTextBox.Text;
            string priceText = PriceTextBox.Text;
            int price = 0;
            if (!string.IsNullOrEmpty(priceText))
            {
                int.TryParse(priceText, out price);
            }

            int statusID = GetSelectedStatusID();
            int specialistID = GetSelectedSpecialistID();
            DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.Now;
            DateTime? endDate = EndDatePicker.SelectedDate;

            InsertRequestIntoDatabase(requestNumber, equipment, issueType, clientInfo, description, price, statusID, specialistID, startDate, endDate);

            MessageBox.Show($"Добавлена заявка #{requestNumber} в {currentTime}");

            ClearFormFields();
        }


        private int GetSelectedStatusID()
        {
            ComboBoxItem selectedItem = (ComboBoxItem)StatusComboBox.SelectedItem;
            string statusValue = selectedItem.Content.ToString();

            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT StatusID FROM dbo.Status WHERE Status = @Status";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", statusValue);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return 0;
        }

        private void InsertRequestIntoDatabase(int requestNumber, string equipment, string issueType, string clientInfo, string description, int price, int statusID, int specialistID, DateTime startDate, DateTime? endDate)
        {
            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO dbo.Orders (ID, Equipment, IssueType, ClientInfo, Description, Price, StatusID, UserID, StartDate, EndDate) " +
                               "VALUES (@ID, @Equipment, @IssueType, @ClientInfo, @Description, @Price, @StatusID, @UserID, @StartDate, @EndDate)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", requestNumber);
                    command.Parameters.AddWithValue("@Equipment", equipment);
                    command.Parameters.AddWithValue("@IssueType", issueType);
                    command.Parameters.AddWithValue("@ClientInfo", clientInfo);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@StatusID", statusID);
                    command.Parameters.AddWithValue("@UserID", specialistID);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        private UserInfo GetSelectedSpecialistInfo()
        {
            string selectedSpecialist = ResponsibleComboBox.SelectedItem?.ToString();
            UserInfo specialistInfo = null;

            if (!string.IsNullOrEmpty(selectedSpecialist))
            {
                string[] names = selectedSpecialist.Split(' ');
                string firstName = names[0];
                string lastName = names[1];

                string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT UserID, Name, Surname, RoleID FROM dbo.[User] WHERE RoleID = 1 AND Name = @FirstName AND Surname = @LastName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                specialistInfo = new UserInfo
                                {
                                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID"))
                                };
                            }
                        }
                    }
                }
            }

            return specialistInfo;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
        }
    }
}
