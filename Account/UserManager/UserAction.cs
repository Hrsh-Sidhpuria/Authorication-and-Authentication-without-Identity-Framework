
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Cryptography;
using System.Text;
using Test3.Account.ClaimManager;
using Test3.Account.RoleManager;

namespace Test3.Account.UserManager
{
    public class UserAction : IUserAction
    {

        private const string Salt = "Rq3F5s8v2y4B9";
        private readonly string _connectionString;
        private readonly RoleModel _roleModel;
        private readonly IRoleAction _roleAction;
        private readonly IClaimAction _claimAction;

        public UserAction(IConfiguration configuration, RoleModel RoleModel, IRoleAction roleAction, IClaimAction claimAction)
        {
            _connectionString = configuration.GetConnectionString("dbcs");
            _roleModel = RoleModel;
            this._roleAction = roleAction;
            this._claimAction = claimAction;
        }

        //for Registering user
        public bool createUser(string Username, string Password, string Role, string Email)
        {
            string Id = GenerateUniqueHashId();
            bool isregister = false;
            string passwordHash = HashPassword(Password);
            string NormalizedUserName = Normalize(Username);
            string RoleId = _roleAction.GetRoleId(Role);

            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    Console.WriteLine("connected");

                    string userInsertQuery = "insert into AspNetUsers(Id,Username,NormalizedUserName,Email,EmailConfirmed,PasswordHash,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount) values(@Id,@Username,@NormalizedUserName,@Email,@EmailConfirmed,@PasswordHash,@PhoneNumberConfirmed,@TwoFactorEnabled,@LockoutEnabled,@AccessFailedCount)";



                    SqlCommand cmdUser = new SqlCommand(userInsertQuery, conn);
                    cmdUser.Parameters.AddWithValue("Id", Id);
                    cmdUser.Parameters.AddWithValue("Username", Username);
                    cmdUser.Parameters.AddWithValue("NormalizedUserName", NormalizedUserName);
                    cmdUser.Parameters.AddWithValue("Email", Email);
                    cmdUser.Parameters.AddWithValue("EmailConfirmed", 0);
                    cmdUser.Parameters.AddWithValue("PasswordHash", passwordHash);
                    cmdUser.Parameters.AddWithValue("PhoneNumberConfirmed", 0);
                    cmdUser.Parameters.AddWithValue("TwoFactorEnabled", 0);
                    cmdUser.Parameters.AddWithValue("LockoutEnabled", 0);
                    cmdUser.Parameters.AddWithValue("AccessFailedCount", 0);

                    int respUserInsert = cmdUser.ExecuteNonQuery();


                    bool isUserRoleAssign = AssignUsersRoles(Username, Role);
                    
                    if (respUserInsert > 0 && isUserRoleAssign == true)
                    {
                            isregister = true;
                            return isregister;
                        
                    }
                    
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return isregister;
        }

        //method to fetch userid and roleid to insert it in AspNetUsersRoles table 
        public bool AssignUsersRoles(string Username, string Role)
        {
            bool isUserRoleAssign = false;
            string UserId = null;
            string RoleID = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    Console.WriteLine("connected");


                    string FetchUserId = "Select * from AspNetUsers where Username = @Username";
                    SqlCommand cmdUserId = new SqlCommand(FetchUserId, conn);
                    cmdUserId.Parameters.AddWithValue("Username", Username);
                    using (SqlDataReader rd = cmdUserId.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            UserId = rd["Id"].ToString();

                        }
                    }

                    string FetchRoleId = "Select * from AspNetRoles where Name = @Name";
                    SqlCommand cmdRoleId = new SqlCommand(FetchRoleId, conn);
                    cmdRoleId.Parameters.AddWithValue("Name", Role);

                    using (SqlDataReader rd1 = cmdRoleId.ExecuteReader())
                    {
                        if (rd1.Read())
                        {
                            RoleID = rd1["Id"].ToString();

                        }
                    }

                    string insertUserIdRoleId = "insert into AspNetUserRoles values(@UserId,@RoleId)";
                    SqlCommand cmdUserIdRoleId = new SqlCommand(insertUserIdRoleId, conn);
                    cmdUserIdRoleId.Parameters.AddWithValue("UserId", UserId);
                    cmdUserIdRoleId.Parameters.AddWithValue("RoleId", RoleID);
                    int resp = cmdUserIdRoleId.ExecuteNonQuery();
                    if (resp > 0)
                    {
                        isUserRoleAssign = true;
                        return isUserRoleAssign;
                    }
                    else
                    {
                        isUserRoleAssign = false;
                        return isUserRoleAssign;
                    }



                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return isUserRoleAssign;
        }

        //for login user 
        public bool LoginUser(string Username, string Password)
        {

            string passwordHash = HashPassword(Password);
            Console.WriteLine(passwordHash);
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string LoginQuery = "select * from AspNetUsers where Username = @Username";
                    SqlCommand cmd = new SqlCommand(LoginQuery, conn);
                    cmd.Parameters.AddWithValue("Username", Username);
                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd != null)
                    {
                        if (rd.Read())
                        {

                            string storedPasswordHash = rd["PasswordHash"].ToString();
                            if (storedPasswordHash == passwordHash)
                            {

                                return true;
                            }
                            else
                            {


                                return false;
                            }
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }



        public bool deleteUser(string username, string password)
        {
            
            string fetchPassword = getPassword(username);
            string passwordHash = HashPassword(password);
            string UserId = getIdByName(username);

            if (fetchPassword == passwordHash)
            {
                SqlConnection conn = new SqlConnection(_connectionString);
                try
                {
                    
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        bool deleteRole = _roleAction.deleteRoleById(UserId);
                        string deleteuser = "DELETE FROM AspNetUsers WHERE Username = @Username;";
                        SqlCommand cmdDelete = new SqlCommand(deleteuser, conn);
                        cmdDelete.Parameters.AddWithValue("Username", username);
                        int row = cmdDelete.ExecuteNonQuery();
                        
                        if (row > 0 )
                        {

                            return true;
                        }  
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return false;

        }



        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        //use for normalize string 
        public static string Normalize(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Username cannot be null or empty.");


            return str.Trim().ToUpper();
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
                return Convert.ToBase64String(hashBytes);
            }
        }

        public string getEmail(string Username)
        {
            string email = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string fetchEmail = "select * from AspNetUsers where Username = @Username";
                    SqlCommand cmd = new SqlCommand(fetchEmail, conn);
                    cmd.Parameters.AddWithValue("Username", Username);
                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd != null)
                    {
                        if (rd.Read())
                        {

                            email = rd["Email"].ToString();
                        }

                    }

                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return email;
        }

        public bool updateUser(string Id, string Username, string Email, string role)
        {
            string NormalizeUsername = Normalize(Username);
            string RoleId = _roleAction.GetRoleId(role);
            bool Userdata = false;
            bool Roledata = false;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string UpdateUserDate = "UPDATE AspNetUsers SET UserName = @Username,NormalizedUserName = @NormalizeUsername, Email = @Email WHERE Id = @Id;";
                        SqlCommand Usercmd = new SqlCommand(UpdateUserDate, conn);
                        Usercmd.Parameters.AddWithValue("Username", Username);
                        Usercmd.Parameters.AddWithValue("NormalizeUsername", NormalizeUsername);
                        Usercmd.Parameters.AddWithValue("Email", Email);
                        Usercmd.Parameters.AddWithValue("Id", Id);
                        int rowsAffected = Usercmd.ExecuteNonQuery();
                        if (rowsAffected > 0) { Userdata = true; }
                        else { Userdata = false; }
                    }
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string UpdateRoleDate = "UPDATE AspNetUserRoles SET RoleId = @RoleId WHERE UserId = @UserId;";
                        SqlCommand Rolecmd = new SqlCommand(UpdateRoleDate, conn);
                        Rolecmd.Parameters.AddWithValue("RoleId", RoleId);
                        Rolecmd.Parameters.AddWithValue("UserId", Id);
                        int rowsAffected = Rolecmd.ExecuteNonQuery();
                        if (rowsAffected > 0) { Roledata = true; }
                        else { Roledata = false; }
                    }
                    if (Userdata == true && Roledata == true)
                    {
                        return true;
                    }
                }

                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return false;
        }

        public string getIdByName(string Username)
        {
            string Id = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string fetchId = "select * from AspNetUsers where Username = @Username";
                    SqlCommand cmd = new SqlCommand(fetchId, conn);
                    cmd.Parameters.AddWithValue("Username", Username);
                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd != null)
                    {
                        if (rd.Read())
                        {

                            Id = rd["Id"].ToString();
                        }

                    }

                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Id;
        }

        public bool UpdatePassword(string Username, string currentPass, string Newpass)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            string hashCurrentPass = HashPassword(currentPass);
            string hashNewPass = HashPassword(Newpass);
            string fetchPassword = getPassword(Username);
            try
            {
                conn.Open();
                if (hashCurrentPass == fetchPassword)

                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string updatePassword = "update AspNetUsers set PasswordHash = @password Where Username = @Username";
                        SqlCommand cmdupdate = new SqlCommand(updatePassword, conn);
                        cmdupdate.Parameters.AddWithValue("password", hashNewPass);
                        cmdupdate.Parameters.AddWithValue("Username", Username);
                        int row = cmdupdate.ExecuteNonQuery();
                        if (row > 0)
                        {
                            return true;
                        }


                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public string getPassword(string Username)
        {
            string fetchedPassword = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string fetchPassword = "select * from AspNetUsers where Username = @Username";
                    SqlCommand cmdfetch = new SqlCommand(fetchPassword, conn);
                    cmdfetch.Parameters.AddWithValue("Username", Username);
                    SqlDataReader rd = cmdfetch.ExecuteReader();

                    if (rd != null)
                    {
                        if (rd.Read())
                        {

                            fetchedPassword = rd["PasswordHash"].ToString();
                            return fetchedPassword;
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return fetchedPassword;
        }

        
    }
    
}

