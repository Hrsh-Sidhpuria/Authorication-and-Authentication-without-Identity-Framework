using Microsoft.Data.SqlClient;
using Authorization_Authentication.Services.ServicesModel;
using System.Data;
using System.Text;
using System.Security.Cryptography;
using NuGet.Protocol.Plugins;
using System;
using System.Reflection;
using Authorization_Authentication.Account.UserManager;


namespace Authorization_Authentication.Services
{
    public class TicketManagementServices : ITicketManagementServices
    {
        private readonly string _connectionString;
        private static readonly string Salt = "harsh96671@";
        private readonly IUserAction _userManager;

        public TicketManagementServices(IConfiguration configuration, IUserAction userManager)
        {
            this._connectionString = configuration.GetConnectionString("dbcs");
            this._userManager = userManager;
        }

        public async Task<List<Modules>> GetModuleList()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            var modules = new List<Modules>();

            string ModuleListCmd = "Select * from Modules";
            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    SqlCommand moduleList = new SqlCommand(ModuleListCmd, conn);
                    SqlDataReader rd = moduleList.ExecuteReader();

                    while (rd.Read())
                    {
                        modules.Add(new Modules
                        {
                            ModuleId = Convert.ToInt32(rd["ModuleId"]),
                            ModuleName = rd["ModuleName"].ToString()
                        });
                    }



                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return modules;


        }

        public async Task<string> issueticket(Tickets tickets)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            string ticketId = GenerateUniqueHashId();

            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {


                    string ticketCreation = "insert into Tickets(TicketId,Title,Detail,Priority,Status,ModuleId,UserId,CreatedAt,Reply) values(@TicketId,@Title,@Detail,@Priority,@Status,@ModuleId,@UserId,@CreatedAt,@Reply)";



                    SqlCommand createTicket = new SqlCommand(ticketCreation, conn);
                    createTicket.Parameters.AddWithValue("TicketId", ticketId);
                    createTicket.Parameters.AddWithValue("Title", tickets.Title);
                    createTicket.Parameters.AddWithValue("Detail", tickets.Detail);
                    createTicket.Parameters.AddWithValue("Priority", tickets.Priority);
                    createTicket.Parameters.AddWithValue("Status", tickets.Status);
                    createTicket.Parameters.AddWithValue("ModuleId", tickets.ModuleId);
                    createTicket.Parameters.AddWithValue("UserId", tickets.UserId);
                    createTicket.Parameters.AddWithValue("CreatedAt", tickets.CreatedAt);
                    createTicket.Parameters.AddWithValue("Reply", "Pending");

                    int resp = await createTicket.ExecuteNonQueryAsync();


                    if (resp > 0)
                    {
                        return ticketId;

                    }

                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static string GenerateUniqueHashId()
        {
            string uniqueString = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid().ToString()}";
            return HashString(uniqueString);
        }


        private static string HashString(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedString = input + Salt;
                var bytes = Encoding.UTF8.GetBytes(combinedString);
                var hashBytes = sha256.ComputeHash(bytes);

                // Convert to Base64 and replace special characters
                var base64String = Convert.ToBase64String(hashBytes);
                var safeString = base64String.Replace('+', 'A')
                                             .Replace('/', 'B')
                                             .Replace('=', 'C');  // Replace '=' with 'C'

                return safeString;
            }
        }

        public async Task<List<Tickets>> GetticketsList(string UserId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            List<Tickets> UserTicket = new List<Tickets>();

            string listOfTicket = "Select * from Tickets where UserID = @UserId";
            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    SqlCommand list = new SqlCommand(listOfTicket, conn);
                    list.Parameters.AddWithValue("UserId", UserId);

                    SqlDataReader rd = list.ExecuteReader();

                    while (rd.Read())
                    {
                        UserTicket.Add(new Tickets
                        {
                            TicketId = rd["TicketId"].ToString(),
                            Title = rd["Title"].ToString(),
                            Detail = rd["Detail"].ToString(),
                            Status = rd["Status"].ToString(),
                        });
                    }



                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return UserTicket;

        }

        public async Task<Tickets> getTicketDetail(string TicketId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            Tickets UserTicket = new Tickets();

            string TicketQuery = "Select * from Tickets where TicketId = @TicketId";
            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    SqlCommand ticket = new SqlCommand(TicketQuery, conn);
                    ticket.Parameters.AddWithValue("TicketId", TicketId);

                    SqlDataReader rd = ticket.ExecuteReader();

                    while (rd.Read())
                    {
                        UserTicket.TicketId = rd["TicketId"].ToString();
                        UserTicket.Title = rd["Title"].ToString();
                        UserTicket.Detail = rd["Detail"].ToString();
                        UserTicket.Status = rd["Status"].ToString();
                        UserTicket.Reply = rd["Reply"].ToString();
                    }



                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return UserTicket;
        }

        public async Task<List<Tickets>> GetAllTickets()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            List<Tickets> UserTicket = new List<Tickets>();

            string listOfTicket = "Select * from Tickets";
            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    SqlCommand list = new SqlCommand(listOfTicket, conn);

                    SqlDataReader rd = list.ExecuteReader();

                    while (rd.Read())
                    {
                        UserTicket.Add(new Tickets
                        {
                            TicketId = rd["TicketId"].ToString(),
                            Title = rd["Title"].ToString(),
                            Detail = rd["Detail"].ToString(),
                            Priority = rd["Priority"].ToString(),
                            Status = rd["Status"].ToString(),
                            ModuleId = rd.GetInt32("ModuleId"),
                            UserId = rd["UserId"].ToString(),
                            UserName = _userManager.getNameById(rd["UserId"].ToString())



                        });
                    }



                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return UserTicket;
        }

        public async Task<bool> AddToTicketReplies(string sender, string chat, string ticketId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {


                    string ticketCreation = "insert into Ticket_Replies(TicketId,Sender,Chat,Date) values(@TicketId,@Sender,@Chat,@Date)";



                    SqlCommand createTicket = new SqlCommand(ticketCreation, conn);
                    createTicket.Parameters.AddWithValue("TicketId", ticketId);
                    createTicket.Parameters.AddWithValue("Sender", sender);
                    createTicket.Parameters.AddWithValue("Chat", chat);
                    createTicket.Parameters.AddWithValue("Date", DateTime.Now);


                    int resp = await createTicket.ExecuteNonQueryAsync();


                    if (resp > 0)
                    {
                        return true;

                    }

                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<List<ChatViewModel>> GetChatMessagesAsync(string ticketId)
        {
            var chatMessages = new List<ChatViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = "SELECT TicketId, Sender, Chat, ImageUrl, IsImage, Date FROM Ticket_Replies WHERE TicketId = @TicketId ORDER BY Date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);

                    using (SqlDataReader rd = await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            var chatMessage = new ChatViewModel
                            {
                                TicketId = rd["TicketId"].ToString(),
                                Sender = rd["Sender"].ToString(),
                                Message = rd["Chat"].ToString(), 
                                ImageUrl = rd["ImageUrl"].ToString(),
                                IsImage = rd.IsDBNull(rd.GetOrdinal("IsImage")) ? false : rd.GetBoolean(rd.GetOrdinal("IsImage")),
                                Date = rd.GetDateTime(rd.GetOrdinal("Date"))
                            };

                            chatMessages.Add(chatMessage);
                        }
                    }
                }
            }

            return chatMessages;
        }


        public async Task<bool> AddReply(string sender, string ticketId, string reply)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {


                    string ticketCreation = "insert into Ticket_Replies(TicketId,Sender,Chat,Date) values(@TicketId,@Sender,@Chat,@Date)";



                    SqlCommand createTicket = new SqlCommand(ticketCreation, conn);
                    createTicket.Parameters.AddWithValue("TicketId", ticketId);
                    createTicket.Parameters.AddWithValue("Sender", sender);
                    createTicket.Parameters.AddWithValue("Chat", reply);
                    createTicket.Parameters.AddWithValue("Date", DateTime.Now);


                    int resp = await createTicket.ExecuteNonQueryAsync();


                    if (resp > 0)
                    {
                        return true;

                    }

                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<bool> UpdateStatus(string status, string ticketId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {


                    string ticketCreation = "UPDATE Tickets SET Status = @Status where TicketId = @TicketId;";



                    SqlCommand createTicket = new SqlCommand(ticketCreation, conn);
                    createTicket.Parameters.AddWithValue("TicketId", ticketId);
                    createTicket.Parameters.AddWithValue("Status", status);


                    int resp = await createTicket.ExecuteNonQueryAsync();


                    if (resp > 0)
                    {
                        return true;

                    }

                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<string> getStatusFromTicketId(string ticketId)
        {
            string Status = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            List<Tickets> UserTicket = new List<Tickets>();

            string listOfTicket = "Select Status from Tickets where TicketId = @TicketId";
            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    SqlCommand statusCmd = new SqlCommand(listOfTicket, conn);
                    statusCmd.Parameters.AddWithValue("TicketId", ticketId);

                    SqlDataReader rd = statusCmd.ExecuteReader();

                    while (rd.Read())
                    {


                        Status = rd["Status"].ToString();


                    }



                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Status;
        }

        public async Task<bool> UploadChatImage(string ticketId, string sender, string message, string imageUrl)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {


                    string ticketCreation = "insert into Ticket_Replies(TicketId,Sender,Chat,Date,ImageUrl,IsImage) values(@TicketId,@Sender,@Chat,@Date,@ImageUrl,@IsImage)";



                    SqlCommand createTicket = new SqlCommand(ticketCreation, conn);
                    createTicket.Parameters.AddWithValue("TicketId", ticketId);
                    createTicket.Parameters.AddWithValue("Sender", sender);
                    createTicket.Parameters.AddWithValue("Chat", message);
                    createTicket.Parameters.AddWithValue("Date", DateTime.Now);
                    createTicket.Parameters.AddWithValue("ImageUrl", imageUrl);
                    createTicket.Parameters.AddWithValue("IsImage", true);


                    int resp = await createTicket.ExecuteNonQueryAsync();


                    if (resp > 0)
                    {
                        return true;

                    }

                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }


        /*public async Task<bool> UpdateStatus(string status, string ticketId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {


                    string ticketCreation = "UPDATE Tickets SET Status = @Status where TicketId = @TicketId;";



                    SqlCommand createTicket = new SqlCommand(ticketCreation, conn);
                    createTicket.Parameters.AddWithValue("TicketId", ticketId);
                    createTicket.Parameters.AddWithValue("Status", status);


                    int resp = await createTicket.ExecuteNonQueryAsync();


                    if (resp > 0)
                    {
                        return true;

                    }

                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }*/

        public string GetIdfromTicketId(string TicketId)
        {
            string userId = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            List<Tickets> UserTicket = new List<Tickets>();

            string listOfTicket = "Select UserId from Tickets where TicketId = @TicketId";
            try
            {
                 conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    SqlCommand statusCmd = new SqlCommand(listOfTicket, conn);
                    statusCmd.Parameters.AddWithValue("TicketId", TicketId);

                    SqlDataReader rd = statusCmd.ExecuteReader();

                    while (rd.Read())
                    {


                        userId = rd["UserId"].ToString();


                    }



                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return userId;
        }
    }
}
