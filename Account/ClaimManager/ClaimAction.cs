﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Test3.Account.RoleManager;
using Test3.Account.UserManager;

namespace Test3.Account.ClaimManager
{
    public class ClaimAction : IClaimAction
    {
        private readonly string _connectionString;


        public ClaimAction(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("dbcs");


        }
        public bool addUserClaim(string UserId,string Email)
        {

            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                conn.Open();


                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string claimQuery = "INSERT INTO AspNetUserClaims VALUES (@UserId,@ClaimType,@ClaimValue)";
                    SqlCommand addClaim = new SqlCommand(claimQuery, conn);
                    addClaim.Parameters.AddWithValue("UserId", UserId);
                    addClaim.Parameters.AddWithValue("ClaimType", "Email");
                    addClaim.Parameters.AddWithValue("ClaimValue", Email);
                    int row = addClaim.ExecuteNonQuery();
                    if (row > 0)
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
            return false;
        }

        public string getUserClaim(string UserId)
        {
            string claimValue = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string fetchClaim = "select * from AspNetUserClaims where UserId = @UserId";
                    SqlCommand cmdfetch = new SqlCommand(fetchClaim, conn);
                    cmdfetch.Parameters.AddWithValue("UserId", UserId);
                    SqlDataReader rd = cmdfetch.ExecuteReader();

                    if (rd != null)
                    {
                        if (rd.Read())
                        {

                            claimValue = rd["ClaimValue"].ToString();
                            return claimValue;
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
            return claimValue;

        }

        public bool addRoleClaim(string RoleId, string Role)
        {
            
            SqlConnection conn = new SqlConnection(_connectionString);

            try
            {
                conn.Open();


                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string claimQuery = "INSERT INTO AspNetRoleClaims VALUES (@RoleId,@ClaimType,@ClaimValue)";
                    SqlCommand addClaim = new SqlCommand(claimQuery, conn);
                    addClaim.Parameters.AddWithValue("RoleId", RoleId);
                    addClaim.Parameters.AddWithValue("ClaimType", "Role");
                    addClaim.Parameters.AddWithValue("ClaimValue", Role);
                    int row = addClaim.ExecuteNonQuery();
                    if (row > 0)
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
            return false;
        }

        public List<Claim> GetRoleClaims(string roleId)
        {
            List<Claim> claims = new List<Claim>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                     conn.OpenAsync();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string fetchClaim = "SELECT ClaimType, ClaimValue FROM AspNetRoleClaims WHERE RoleId = @RoleId";
                        using (SqlCommand cmdFetch = new SqlCommand(fetchClaim, conn))
                        {
                            cmdFetch.Parameters.AddWithValue("@RoleId", roleId);

                            using (SqlDataReader rd =  cmdFetch.ExecuteReader())
                            {
                                while ( rd.Read())
                                {
                                    string claimType = rd["ClaimType"].ToString();
                                    string claimValue = rd["ClaimValue"].ToString();

                                    claims.Add(new Claim(claimType, claimValue));
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

            return claims;
        }



    }

}



