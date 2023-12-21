// RequestList.xaml.cs

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using static WpfApp1.MainWindow;

namespace WpfApp1
{
    public partial class RequestList : Window
    {
        private bool isOrderPageOpen = false;

        public RequestList()
        {
            InitializeComponent();
            Loaded += (sender, e) => LoadOrdersForUser(UserData.UserID);
            dataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
        }

        public class Order
        {
            public int ID { get; set; }
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int StatusID { get; set; }
            public string ClientInfo { get; set; }
            public int Price { get; set; }
            public string Equipment { get; set; }
            public string IssueType { get; set; }
            public string MasterComment { get; set; }
        }

        private void LoadOrdersForUser(int userID)
        {
            try
            {
                List<Order> orders = GetOrdersForUser(userID);
                dataGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
        }

        private List<Order> GetOrdersForUser(int userID)
        {
            List<Order> orders = new List<Order>();

            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM dbo.Orders";

                // Добавляем условие для RoleID 2 и 3
                if (UserData.RoleID != 2 && UserData.RoleID != 3)
                {
                    query += " WHERE UserID = @UserID";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Передаем параметр, если это не пользователи с RoleID 2 и 3
                    if (UserData.RoleID != 2 && UserData.RoleID != 3)
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order order = new Order
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                                ClientInfo = reader.GetString(reader.GetOrdinal("ClientInfo")),
                                Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                Equipment = reader.GetString(reader.GetOrdinal("Equipment")),
                                IssueType = reader.GetString(reader.GetOrdinal("IssueType")),
                                MasterComment = reader.GetString(reader.GetOrdinal("MasterComment")),
                            };

                            orders.Add(order);
                        }
                    }
                }
            }

            return orders;
        }


        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Get the selected order
            Order selectedOrder = (Order)dataGrid.SelectedItem;

            if (selectedOrder != null && !isOrderPageOpen)
            {
                // Create an instance of OrderPage and pass the data
                OrderPage orderPage = new OrderPage(selectedOrder);
                orderPage.Closed += OrderPage_Closed;
                orderPage.Show();

                isOrderPageOpen = true;
            }
        }

        private void OrderPage_Closed(object sender, EventArgs e)
        {
            isOrderPageOpen = false;
        }
    }
}
