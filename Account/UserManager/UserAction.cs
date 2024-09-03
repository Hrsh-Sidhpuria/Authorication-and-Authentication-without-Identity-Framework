
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Cryptography;
using System.Text;
using Authorization_Authentication.Account.ClaimManager;
using Authorization_Authentication.Account.RoleManager;

namespace Authorization_Authentication.Account.UserManager
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
        //verifying if the username is available or not 
        public async Task<bool> UsernameAvailable(string Username)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string isAvailQuery = "select * from AspNetUsers where UserName = @Username";
                    SqlCommand idAvailcmd = new SqlCommand(isAvailQuery, conn);
                    idAvailcmd.Parameters.AddWithValue("Username", Username);
                    SqlDataReader rd = await idAvailcmd.ExecuteReaderAsync();

                        if (rd.Read()) { return false; }
                        else { return true; }
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



        //for Registering user
        public async Task<bool> createUser(string Username, string Password, string Role, string Email)
        {
            string Id = GenerateUniqueHashId();
            bool isregister = false;
            string passwordHash = HashPassword(Password);
            string NormalizedUserName = Normalize(Username);
            string RoleId = _roleAction.GetRoleId(Role);

            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                await conn.OpenAsync();
                if (conn.State == System.Data.ConnectionState.Open)
                {

                    

                    string userInsertQuery = "insert into AspNetUsers(Id,UserName,NormalizedUserName,Email,EmailConfirmed,PasswordHash,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount) values(@Id,@Username,@NormalizedUserName,@Email,@EmailConfirmed,@PasswordHash,@PhoneNumberConfirmed,@TwoFactorEnabled,@LockoutEnabled,@AccessFailedCount)";



                    SqlCommand cmdUser = new SqlCommand(userInsertQuery, conn);
                    cmdUser.Parameters.AddWithValue("Id", Id);
                    cmdUser.Parameters.AddWithValue("Username", Username);
                    cmdUser.Parameters.AddWithValue("NormalizedUserName", NormalizedUserName);
                    cmdUser.Parameters.AddWithValue("Email", Email);
                    cmdUser.Parameters.AddWithValue("EmailConfirmed", 0);
                    cmdUser.Parameters.AddWithValue("PasswordHash", passwordHash);
                    cmdUser.Parameters.AddWithValue("PhoneNumberConfirmed", 0);
                    cmdUser.Parameters.AddWithValue("TwoFactorEnabled", 0);
                    cmdUser.Parameters.AddWithValue("LockoutEnabled", 1);
                    cmdUser.Parameters.AddWithValue("AccessFailedCount", 0);

                    int respUserInsert = await cmdUser.ExecuteNonQueryAsync();


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

        //for login user 
        public bool LoginUser(string Username, string Password)
        {

            string passwordHash = HashPassword(Password);
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string LoginQuery = "select * from AspNetUsers where UserName = @Username";
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

        public int accessFailedCount(string Username,int totalFailedCount)
        {
            int currentfailedCount=0;
            int failCountRemain=0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {

                        string failCount = "SELECT * FROM AspNetUsers WHERE UserName = @UserName";
                        SqlCommand Usercmd = new SqlCommand(failCount, conn);
                        Usercmd.Parameters.AddWithValue("UserName", Username);
                        SqlDataReader rd = Usercmd.ExecuteReader();
                        if (rd != null)
                        {
                            if (rd.Read())
                            {
                                currentfailedCount = Convert.ToInt32(rd["AccessFailedCount"]);
                            }
                        }
                        currentfailedCount++;

                        string UpdateDate = "UPDATE AspNetUsers SET AccessFailedCount = @AccessFailedCount WHERE UserName = @UserName";
                        SqlCommand updatecmd = new SqlCommand(UpdateDate, conn);
                        updatecmd.Parameters.AddWithValue("AccessFailedCount", currentfailedCount);
                        updatecmd.Parameters.AddWithValue("UserName", Username);
                        int rowsAffected = updatecmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            failCountRemain = totalFailedCount - currentfailedCount;
                            return failCountRemain;
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
            return failCountRemain;

        }

        public bool resetFailCount(string Username)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string resetDate = "UPDATE AspNetUsers SET AccessFailedCount = @AccessFailedCount WHERE UserName = @UserName";
                        SqlCommand resetcmd = new SqlCommand(resetDate, conn);
                        resetcmd.Parameters.AddWithValue("AccessFailedCount", 0);
                        resetcmd.Parameters.AddWithValue("UserName", Username);
                        int rowsAffected = resetcmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return true;
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
        }

        public bool setLockAccountdate(string Username)
        {
            //giving 1 min for testing purpose
            var lockoutEndDate = DateTimeOffset.UtcNow.AddMinutes(1);
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string resetDate = "UPDATE AspNetUsers SET LockoutEnd = @LockoutEnd WHERE UserName = @UserName";
                        SqlCommand resetcmd = new SqlCommand(resetDate, conn);
                        resetcmd.Parameters.AddWithValue("LockoutEnd", lockoutEndDate);
                        resetcmd.Parameters.AddWithValue("UserName", Username);
                        int rowsAffected = resetcmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {

                            resetFailCount(Username);
                            return true;
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

            return false;
        }

        public bool getLockAccountdate(string Username)
        {
            
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                
                try
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string failCount = "SELECT * FROM AspNetUsers WHERE UserName = @UserName";
                        SqlCommand Usercmd = new SqlCommand(failCount, conn);
                        Usercmd.Parameters.AddWithValue("UserName", Username);
                        SqlDataReader rd = Usercmd.ExecuteReader();
                        if (rd != null)
                        {
                            if (rd.Read())
                            {
                                var lockoutEndDate = (DateTimeOffset)rd["LockoutEnd"];
                                if (lockoutEndDate > DateTimeOffset.UtcNow)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
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
                
            }

            return false;
        }

        public bool isLockAccount(string Username)
        {
            string lockdate = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string islock = "Select * from AspNetUsers where UserName = @UserName";
                        SqlCommand isLockcmd = new SqlCommand(islock, conn);
                        isLockcmd.Parameters.AddWithValue("UserName", Username);
                        SqlDataReader rd = isLockcmd.ExecuteReader();
                        if (rd != null)
                        {
                            if (rd.Read())
                            {
                                lockdate = rd["LockoutEnd"].ToString();

                            }

                        }
                    }
                    if (lockdate != null)
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
                return false;
            }
        }

        //method to fetch userid and roleid and then inserting it in AspNetUsersRoles table 
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

        //method to generate hash password 
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

        //method to normalize string 
        public static string Normalize(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Username cannot be null or empty.");


            return str.Trim().ToUpper();
        }


        //generate a unique hash id 
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

        public string getNameById(string UserId)
        {
             string name = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string fetchName = "select * from AspNetUsers where Id = @UserId";
                    SqlCommand cmd = new SqlCommand(fetchName, conn);
                    cmd.Parameters.AddWithValue("UserId", UserId);
                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd != null)
                    {
                        if (rd.Read())
                        {

                            name = rd["UserName"].ToString();
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
            return name;
        }
    }
    
}

