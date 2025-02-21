using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Models;
using Kendo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Kendo.Repositories.Implementations
{
    public class UserRepository : IUserInterface
    {
        private readonly NpgsqlConnection _conn;
        public UserRepository(NpgsqlConnection connection)
        {
            _conn = connection;
        }

        public async Task<User> GetUser(int userid)
        {
            User userData = new User();
            var query = "SELECT * FROM t_users WHERE c_userid=@c_userid;";
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_userid", userid);
                    await _conn.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        userData.c_userId = (int)reader["c_userid"];
                        userData.c_userName = (string)reader["c_username"];
                        userData.c_Email = (string)reader["c_email"];
                        userData.c_Gender = (string)reader["c_gender"];
                        userData.c_Mobile = (string)reader["c_mobile"];
                        userData.c_Address = (string)reader["c_address"];
                        userData.c_Image = (string)reader["c_image"];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetUser Error : " + e.Message);
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return userData;
        }

        public async Task<User> Login(Login user)
        {
            User userData = null; // Change from 'new t_User()' to 'null'

            var query = "SELECT * FROM t_users WHERE c_email=@c_email AND c_password=@c_password;";
            try
            {
                await _conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_email", user.c_Email);
                    cmd.Parameters.AddWithValue("@c_password", user.c_Password);

                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        userData = new User
                        {
                            c_userId = reader["c_userid"] != DBNull.Value ? Convert.ToInt32(reader["c_userid"]) : 0,
                            c_userName = Convert.ToString(reader["c_username"]),
                            c_Email = Convert.ToString(reader["c_email"]),
                            c_Gender = Convert.ToString(reader["c_gender"]),
                            c_Mobile = Convert.ToString(reader["c_mobile"]),
                            c_Address = Convert.ToString(reader["c_address"]),
                            c_Image = Convert.ToString(reader["c_image"])
                        };
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Login Error: " + e.Message);
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return userData; // Return null if user is not found
        }


        public async Task<int> Register(User userData)
        {
            int status = 0;
            try
            {
                await _conn.OpenAsync();

                using (NpgsqlCommand comcheck = new NpgsqlCommand(@"SELECT COUNT(*) FROM t_Users WHERE c_email = @c_email;", _conn))
                {
                    comcheck.Parameters.AddWithValue("@c_email", userData.c_Email);
                    int count = Convert.ToInt32(await comcheck.ExecuteScalarAsync());

                    if (count > 0)
                    {
                        await _conn.CloseAsync();
                        return 0; // User already exists
                    }
                }

                using (NpgsqlCommand cm = new NpgsqlCommand(@"INSERT INTO t_users 
            (c_username, c_email, c_password, c_address, c_mobile, c_gender, c_image) 
            VALUES (@c_username, @c_email, @c_password, @c_address, @c_mobile, @c_gender, @c_image);", _conn))
                {
                    cm.Parameters.AddWithValue("@c_username", userData.c_userName);
                    cm.Parameters.AddWithValue("@c_email", userData.c_Email);
                    cm.Parameters.AddWithValue("@c_password", userData.c_Password);
                    cm.Parameters.AddWithValue("@c_address", userData.c_Address);
                    cm.Parameters.AddWithValue("@c_mobile", userData.c_Mobile);
                    cm.Parameters.AddWithValue("@c_gender", userData.c_Gender);
                    cm.Parameters.AddWithValue("@c_image", userData.c_Image ?? (object)DBNull.Value);

                    await cm.ExecuteNonQueryAsync();
                }

                status = 1; // Registration successful
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Register failed, error: " + ex.Message);
                status = -1;
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return status;
        }
    }
}