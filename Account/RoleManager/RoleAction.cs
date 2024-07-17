using Microsoft.Data.SqlClient;
using System.Data;
using Test3.Account.ClaimManager;
using Test3.Account.UserManager;

namespace Test3.Account.RoleManager
{
    public class RoleAction : IRoleAction
    {

        private readonly string _connectionString;
        private readonly UserModel _user;

        public RoleAction(IConfiguration configuration,UserModel user)
        {
            _connectionString = configuration.GetConnectionString("dbcs");
            this._user = user;
        }

        public string GetRole(string Username)
        {
            string UserId = null;
            string RoleId = null;
            string RoleName = null;
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

                    

                    string FetchRoleId = "Select * from AspNetUserRoles where UserId = @UserId";
                    SqlCommand cmdRoleId = new SqlCommand(FetchRoleId, conn);
                    cmdRoleId.Parameters.AddWithValue("UserId", UserId);

                    using (SqlDataReader rd1 = cmdRoleId.ExecuteReader())
                    {
                        if (rd1.Read())
                        {
                            RoleId = rd1["RoleId"].ToString();

                        }
                    }

                    string FetchRoleName = "Select * from AspNetRoles where Id = @Id";
                    SqlCommand cmdrole = new SqlCommand(FetchRoleName, conn);
                    cmdrole.Parameters.AddWithValue("Id", RoleId);
                    using (SqlDataReader rd = cmdrole.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            RoleName = rd["Name"].ToString();

                        }
                    }
                }
                conn.Close();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RoleName;
        }
        public void DeleteRole()
        {
            throw new NotImplementedException();
        }

        public void UpdateRole()
        {
            throw new NotImplementedException();
        }

        public string GetRoleId(string Name)
        {
            string RoleId = null ;
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string FetchRoleId = "Select * from AspNetRoles where Name = @Name";
                    SqlCommand cmdRoleId = new SqlCommand(FetchRoleId, conn);
                    cmdRoleId.Parameters.AddWithValue("Name", Name);

                    using (SqlDataReader rd1 = cmdRoleId.ExecuteReader())
                    {
                        if (rd1.Read())
                        {
                            RoleId = rd1["Id"].ToString();

                        }
                    }
                }
                conn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RoleId;
        }

        public bool deleteRoleById(string UserId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string FetchRoleId = "delete from AspNetUserRoles where UserId = @UserId";
                    SqlCommand deleteRoleId = new SqlCommand(FetchRoleId, conn);
                    deleteRoleId.Parameters.AddWithValue("UserId", UserId);

                    int row = deleteRoleId.ExecuteNonQuery();
                    if (row > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
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

        

    }
}
