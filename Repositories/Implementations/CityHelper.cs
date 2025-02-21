using Kendo.Models;
using Kendo.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace Kendo.Repositories.Implementations
{
    public class CityHelper : ICityHelper
    {
        private readonly NpgsqlConnection con;
        private readonly string connectionString = "Server=cipg01;port=5432;Database=Intern025;User Id=postgres;Password=123456";

        public CityHelper()
        {
            con = new NpgsqlConnection(connectionString);
        }

        public List<tblCity> GetAll()
        {
            var cityList = new List<tblCity>();
            using (var cmd = new NpgsqlCommand("Select * from t_city", con))
            {
                var dt = new DataTable();
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                }
                cityList = (from DataRow dr in dt.Rows
                            select new tblCity
                            {
                                c_cityid = Convert.ToInt32(dr["c_cityid"]),
                                c_cname = dr["c_cname"].ToString()
                            }).ToList();
                con.Close();
            }
            return cityList;
        }

        public tblCity GetOne(int id)
        {
            var dt = new DataTable();
            using (var cmd = new NpgsqlCommand("select * from t_city where c_cityid = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                }
                con.Close();
            }

            return (from DataRow dr in dt.Rows
                    select new tblCity
                    {
                        c_cityid = Convert.ToInt32(dr["c_cityid"]),
                        c_cname = dr["c_cname"].ToString()
                    }).FirstOrDefault();
        }

        public void Insert(tblCity data)
        {
            using (var cmd = new NpgsqlCommand("insert into t_city (c_cname) values (@name)", con))
            {
                cmd.Parameters.AddWithValue("@name", data.c_cname);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void Update(tblCity data)
        {
            using (var cmd = new NpgsqlCommand("Update t_city set c_cname=@name where c_cityid=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", data.c_cityid);
                cmd.Parameters.AddWithValue("@name", data.c_cname);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void Delete(tblCity data)
        {
            using (var cmd = new NpgsqlCommand("delete from t_city where c_cityid=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", data.c_cityid);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}