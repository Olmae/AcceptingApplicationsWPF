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
    /// Логика взаимодействия для TrackOrderStatus.xaml
    /// </summary>
    public partial class TrackOrderStatus : Window
    {
        public TrackOrderStatus()
        {
            InitializeComponent();
        }

        private void TrackStatusButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем номер заявки из TextBox
            if (int.TryParse(OrderNumberTextBox.Text, out int orderNumber))
            {
                // Поиск заявки в базе данных
                if (TryGetOrderFromDatabase(orderNumber, out var selectedOrder))
                {
                    // Открываем OrderPage с найденной заявкой
                    OrderPage orderPage = new OrderPage(selectedOrder);
                    orderPage.Closed += OrderPage_Closed;
                    orderPage.Show();
                }
                else
                {
                    StatusTextBlock.Text = "Заявка не найдена.";
                }
            }
            else
            {
                StatusTextBlock.Text = "Некорректный номер заявки.";
            }
        }
        private void OrderPage_Closed(object sender, EventArgs e)
        {
            // Обработка события закрытия OrderPage
            // Можно выполнить дополнительные действия при закрытии
        }
        private bool TryGetOrderFromDatabase(int orderNumber, out RequestList.Order selectedOrder)
        {
            selectedOrder = null;

            string connectionString = "Data Source=OLMAE\\OLMAE;Initial Catalog=UP;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM dbo.Orders WHERE ID = @OrderID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderNumber);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            selectedOrder = new RequestList.Order
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

                            return true;
                        }
                    }
                }
            }

            return false;
        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
        }
    }
}
