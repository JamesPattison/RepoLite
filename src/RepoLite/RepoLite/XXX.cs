using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace ConsoleApp3
{
    class Program
    {
        private const string cn = "Data Source=W101BF1JP2\\SQLEXPRESS;Initial Catalog=Jim;Integrated Security=true;";

        private static void Sproc1()
        {
            using var conn = new SqlConnection(cn);
            using var command = new SqlCommand("Sproc1", conn) { 
                CommandType = CommandType.StoredProcedure 
            };
            
            conn.Open();
            //var rdr = command.ExecuteReader();

            var da = new SqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);

            //For each resultset
            foreach (DataTable table in ds.Tables)
            {
                foreach (DataColumn column in table.Columns)
                {
                    var vals = column.Table.Rows;
                }
            }
        }

        private class CarTableType
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string company { get; set; }
        }
        
        private static void Sproc2()
        {
            var cars = new List<CarTableType>
            {
                new()
                {
                    Id = 1,
                    Name = "Jim",
                    company = "Reddeer"
                },
                new()
                {
                    Id = 2,
                    Name = "Sebbie",
                    company = "Reddeer"
                },
            };

            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("company", typeof(string));
            foreach (var car in cars)
            {
                dt.Rows.Add(car.Id, car.Name, car.company);
            }
            
            using var conn = new SqlConnection(cn);
            using var command = new SqlCommand("Sproc2", conn) { 
                CommandType = CommandType.StoredProcedure 
            };
            var param = command.Parameters.AddWithValue("@vals", dt);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.CarTableType";
            
            conn.Open();
            //var rdr = command.ExecuteReader();

            var da = new SqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);

            //For each resultset
            foreach (DataTable table in ds.Tables)
            {
                foreach (DataColumn column in table.Columns)
                {
                    var vals = column.Table.Rows;
                }
            }
        }
    }
}