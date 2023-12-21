using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using static WpfApp1.MainWindow;

namespace WpfApp1
{
    public partial class OrderPage : Window
    {
        // Add properties for data
        public int OrderID { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StatusID { get; set; }
        public string ClientInfo { get; set; }
        public int Price { get; set; }
        public string Equipment { get; set; }
        public string IssueType { get; set; }
        public string MasterComment { get; set; }

        public OrderPage(RequestList.Order selectedOrder)
        {
            InitializeComponent();

            // Use the selectedOrder object to populate data in your OrderPage
            OrderID = selectedOrder.ID;

            // Populate other properties from the database
            PopulateDataFromDatabase();

            // Bind properties to controls in XAML to display data
            DataContext = this;
            EnableEditingBasedOnRole();
        }
        private void EnableEditingBasedOnRole()
        {
            // Проверяем RoleID и разрешаем или запрещаем редактирование
            if (UserData.RoleID == 1)
            {
                // RoleID 1 может редактировать только комментарии мастера
                EnableMasterCommentEditing();
            }
            else if (UserData.RoleID == 2 || UserData.RoleID == 3)
            {
                // RoleID 2 и 3 могут полностью редактировать
                EnableFullEditing();
            }
            else
            {
                // Другие случаи, по умолчанию ограничиваем редактирование
                DisableEditing();
            }
        }

        private void EnableMasterCommentEditing()
        {
            // Разрешить редактирование комментария мастера
            // Например, TextBoxMasterComment.IsReadOnly = false;
            TextBoxMasterComment.IsReadOnly = false;
        }

        private void EnableFullEditing()
        {
            // Разрешить полное редактирование
            // Например, TextBoxDescription.IsReadOnly = false;
            // И так далее
            TextBoxID.IsReadOnly = false;
            TextBoxDS.IsReadOnly = false;

            TextBoxDE.IsReadOnly = false;

            TextBoxClientInfo.IsReadOnly = false;

            TextBoxPrice.IsReadOnly = false;

            TextBoxEquipment.IsReadOnly = false;

            TextBoxIssueType.IsReadOnly = false;

            TextBoxDescription.IsReadOnly = false;

            TextBoxMasterComment.IsReadOnly = false;


        }

        private void DisableEditing()
        {
            // Запретить редактирование
            // Например, TextBoxDescription.IsReadOnly = true;
            // И так далее
        }

        // Вызов этого метода из конструктора или другого подходящего места
        private void SetupEditingPermissions()
        {
            EnableEditingBasedOnRole();
        }

        private void PopulateDataFromDatabase()
        {
            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM dbo.Orders WHERE ID = @OrderID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", OrderID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Description = reader.GetString(reader.GetOrdinal("Description"));
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                            StatusID = reader.GetInt32(reader.GetOrdinal("StatusID"));
                            ClientInfo = reader.GetString(reader.GetOrdinal("ClientInfo"));
                            Price = reader.GetInt32(reader.GetOrdinal("Price"));
                            Equipment = reader.GetString(reader.GetOrdinal("Equipment"));
                            IssueType = reader.GetString(reader.GetOrdinal("IssueType"));
                            MasterComment = reader.GetString(reader.GetOrdinal("MasterComment"));
                        }
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the "Back" button click
            Close();
        }
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SaveChangesToDatabase();
                MessageBox.Show("Изменения сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveChangesToDatabase()
        {
            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE dbo.Orders SET " +
                               "Description = @Description, " +
                               "StartDate = @StartDate, " +
                               "EndDate = @EndDate, " +
                               "StatusID = @StatusID, " +
                               "ClientInfo = @ClientInfo, " +
                               "Price = @Price, " +
                               "Equipment = @Equipment, " +
                               "IssueType = @IssueType, " +
                               "MasterComment = @MasterComment " +
                               "WHERE ID = @OrderID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", OrderID);
                    command.Parameters.AddWithValue("@Description", Description);
                    command.Parameters.AddWithValue("@StartDate", StartDate);
                    command.Parameters.AddWithValue("@EndDate", EndDate);
                    command.Parameters.AddWithValue("@StatusID", StatusID);
                    command.Parameters.AddWithValue("@ClientInfo", ClientInfo);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.Parameters.AddWithValue("@Equipment", Equipment);
                    command.Parameters.AddWithValue("@IssueType", IssueType);
                    command.Parameters.AddWithValue("@MasterComment", MasterComment);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
