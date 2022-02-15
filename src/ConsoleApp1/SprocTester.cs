using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    public class SprocTester
    {
        private readonly string _connectionString;

        public SprocTester(string connectionString)
        {
            _connectionString = connectionString;
        }

        //CreateOwner
        public void CreateOwner(
            long companyId,
            string firstName,
            string lastName,
            DateTime dateOfBirth)
        {
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.CreateOwner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@companyId", companyId);
            command.Parameters.AddWithValue("@firstName", firstName);
            command.Parameters.AddWithValue("@lastName", lastName);
            command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth);

            conn.Open();
            command.ExecuteNonQuery();
        }

        //FindOwnersCarByName
        public record FindOwnersCarByName_Result(IEnumerable<FindOwnersCarByName_Result_Car> Items);

        public record FindOwnersCarByName_Result_Car(long Id, long OwnerId, string Make, string Model);

        public FindOwnersCarByName_Result FindOwnersCarByName(string firstName, string lastName)
        {
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("FindOwnersCarByName", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@firstName", firstName);
            command.Parameters.AddWithValue("@lastName", lastName);

            conn.Open();
            //var rdr = command.ExecuteReader();

            var da = new SqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);

            //For each resultset
            if (ds.Tables.Count != 1) throw new Exception("Return count differs to expected. Please regenerate");

            var table = ds.Tables[0];

            var items = from row in table.AsEnumerable()
                select new FindOwnersCarByName_Result_Car(
                    Convert.ToInt64(row["Id"]),
                    Convert.ToInt64(row["OwnerId"]),
                    row["Make"].ToString(),
                    row["Model"].ToString()
                );

            return new FindOwnersCarByName_Result(items);
        }

        //GetCompanyInfo
        public record GetCompanyInfo_Result(
            IEnumerable<GetCompanyInfo_Result_Owners> Owners,
            IEnumerable<GetCompanyInfo_Result_Cars> Cars);

        public record GetCompanyInfo_Result_Owners(System.Int64 Id,System.Int64 CompanyId,System.String FirstName,System.String LastName,System.DateTime DateOfBirth);
        public record GetCompanyInfo_Result_Cars(System.Int64 Id,System.Int64 OwnerId,System.String Make,System.String Model);
        public GetCompanyInfo_Result GetCompanyInfo(
			System.Int64 @companyId
        ) 
        {
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetCompanyInfo", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@companyId", @companyId);

            conn.Open();
            var da = new SqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables.Count != 2) throw new Exception("Return count differs to expected. Please regenerate");

            
            var Owners_table = ds.Tables[0];

            var OwnersResult = from row in Owners_table.AsEnumerable()
                select new GetCompanyInfo_Result_Owners(
                    row.Field<System.Int64>("Id"),
                    row.Field<System.Int64>("CompanyId"),
                    row.Field<System.String>("FirstName"),
                    row.Field<System.String>("LastName"),
                    row.Field<System.DateTime>("DateOfBirth")
                );
            
            var Cars_table = ds.Tables[1];

            var CarsResult = from row in Cars_table.AsEnumerable()
                select new GetCompanyInfo_Result_Cars(
                    row.Field<System.Int64>("Id"),
                    row.Field<System.Int64>("OwnerId"),
                    row.Field<System.String>("Make"),
                    row.Field<System.String>("Model")
                );

            return new GetCompanyInfo_Result(
                OwnersResult,
                CarsResult
);

        }
    
    

        //CreateCars
        public record CreateCars_Request_Car(string Make, string Model, string OwnerName);
        public void CreateCars(IEnumerable<CreateCars_Request_Car> cars)
        {
            var dt = new DataTable();
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("OwnerName", typeof(string));
            foreach (var (make, model, ownerName) in cars)
            {
                dt.Rows.Add(make, model, ownerName);
            }
            
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("CreateCars", conn) { 
                CommandType = CommandType.StoredProcedure 
            };
            var param = command.Parameters.AddWithValue("@car", dt);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.CompanyCarType";
            
            conn.Open();
            command.ExecuteNonQuery();
        }

        //CreateCarsAndReturn
        public record CreateCarsAndReturn_Result(IEnumerable<CreateCarsAndReturn_Result_Car> Cars);
        public record CreateCarsAndReturn_Result_Car(long Id, long OwnerId, string Make, string Model);
        public record CreateCarsAndReturn_Request_Car(string Make, string Model, string OwnerName);
        public CreateCarsAndReturn_Result CreateCarsAndReturn(IEnumerable<CreateCarsAndReturn_Request_Car> cars)
        {
            var dt = new DataTable();
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("OwnerName", typeof(string));
            foreach (var (make, model, ownerName) in cars)
            {
                dt.Rows.Add(make, model, ownerName);
            }
            
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("CreateCarsAndReturn", conn) { 
                CommandType = CommandType.StoredProcedure 
            };
            var param = command.Parameters.AddWithValue("@car", dt);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.CompanyCarType";
            
            conn.Open();
            
            var da = new SqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);

            //For each resultset
            if (ds.Tables.Count != 1) throw new Exception("Return count differs to expected. Please regenerate");

            var cars_table = ds.Tables[0];

            var carsResult = from row in cars_table.AsEnumerable()
                select new CreateCarsAndReturn_Result_Car(
                    row.Field<Int64>("Id"),
                    Convert.ToInt64(row["OwnerId"]),
                    row["Make"].ToString(),
                    row["Model"].ToString()
                );

            return new CreateCarsAndReturn_Result(carsResult);
        }
        
        //Ping
        public void Ping()
        {
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("Ping", conn) { 
                CommandType = CommandType.StoredProcedure 
            };
            
            conn.Open();
            command.ExecuteNonQuery();
        }
        
        //GetAllCars
        public record GetAllCars_Result(IEnumerable<GetAllCars_Result_Car> Items);

        public record GetAllCars_Result_Car(long Id, long OwnerId, string Make, string Model);
        public GetAllCars_Result GetAllCars()
        {
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetAllCars", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();
            //var rdr = command.ExecuteReader();

            var da = new SqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);

            //For each resultset
            if (ds.Tables.Count != 1) throw new Exception("Return count differs to expected. Please regenerate");

            var table = ds.Tables[0];

            var items = from row in table.AsEnumerable()
                select new GetAllCars_Result_Car(
                    Convert.ToInt64(row["Id"]),
                    Convert.ToInt64(row["OwnerId"]),
                    row["Make"].ToString(),
                    row["Model"].ToString()
                );

            return new GetAllCars_Result(items);
        }
        //
        // //GetCompanyInfo
        // public record GetDatabase_Result(
        //     IEnumerable<GetDatabase_Result_Company> Companies,
        //     IEnumerable<GetDatabase_Result_Owner> Owners,
        //     IEnumerable<GetDatabase_Result_Car> Cars);
        //
        // public record GetDatabase_Result_Company(long Id, string Name, long EmployeeCount)
        // {
        //     public static GetDatabase_Result_Company FromDataRow(DataRow row)
        //     {
        //         return new(
        //             Convert.ToInt64(row["Id"]),
        //             row["Name"].ToString(),
        //             Convert.ToInt64(row["ExployeeCount"])
        //         );
        //     }
        // }
        //
        // public record GetDatabase_Result_Owner(long Id, long CompanyId, string FirstName, string LastName,
        //     DateTime DateOfBirth)
        // {
        //     public static GetDatabase_Result_Owner FromDataRow(DataRow row)
        //     {
        //         return new(
        //             Convert.ToInt64(row["Id"]),
        //             Convert.ToInt64(row["CompanyId"]),
        //             row["FirstName"].ToString(),
        //             row["LastName"].ToString(),
        //             Convert.ToDateTime(row["DateOfBirth"])
        //         );
        //     }
        // }
        //
        // public record GetDatabase_Result_Car(long Id, long OwnerId, string Make, string Model)
        // {
        //     public static GetDatabase_Result_Car FromDataRow(DataRow row)
        //     {
        //         return new(
        //             Convert.ToInt64(row["Id"]),
        //             Convert.ToInt64(row["OwnerId"]),
        //             row["Make"].ToString(),
        //             row["Model"].ToString()
        //         );
        //     }
        // }
        //
        // public GetDatabase_Result GetDatabase()
        // {
        //     using var conn = new SqlConnection(_connectionString);
        //     using var command = new SqlCommand("GetDatabase", conn)
        //     {
        //         CommandType = CommandType.StoredProcedure
        //     };
        //
        //     conn.Open();
        //     //var rdr = command.ExecuteReader();
        //
        //     var da = new SqlDataAdapter(command);
        //     var ds = new DataSet();
        //     da.Fill(ds);
        //
        //     //For each resultset
        //     if (ds.Tables.Count != 3) throw new Exception("Return count differs to expected. Please regenerate");
        //
        //     var companies = from row in ds.Tables[0].AsEnumerable()
        //         select GetDatabase_Result_Company.FromDataRow(row);
        //     var owners = from row in ds.Tables[1].AsEnumerable()
        //         select GetDatabase_Result_Owner.FromDataRow(row);
        //     var cars = from row in ds.Tables[2].AsEnumerable()
        //         select GetDatabase_Result_Car.FromDataRow(row);
        //
        //     return new GetDatabase_Result(companies, owners, cars);
        // }
        
        
        
        public record GetDatabase_Result(
            IEnumerable<GetDatabase_Result_Companies> Companies,
            IEnumerable<GetDatabase_Result_Owners> Owners,
            IEnumerable<GetDatabase_Result_Cars> Cars);

        public record GetDatabase_Result_Companies(System.Int64 Id,System.String Name,System.Int64 ExployeeCount);
        public record GetDatabase_Result_Owners(System.Int64 Id,System.Int64 CompanyId,System.String FirstName,System.String LastName,System.DateTime DateOfBirth);
        public record GetDatabase_Result_Cars(System.Int64 Id,System.Int64 OwnerId,System.String Make,System.String Model);
        public GetDatabase_Result GetDatabase(

        ) 
        {
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetDatabase", conn)
            {
                CommandType = CommandType.StoredProcedure
            };


            conn.Open();
            var da = new SqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables.Count != 3) throw new Exception("Return count differs to expected. Please regenerate");

            
            var Companies_table = ds.Tables[0];

            var CompaniesResult = from row in Companies_table.AsEnumerable()
                select new GetDatabase_Result_Companies(
                    row.Field<System.Int64>("Id"),
                    row.Field<System.String>("Name"),
                    row.Field<System.Int64>("ExployeeCount")
                );
            
            var Owners_table = ds.Tables[1];

            var OwnersResult = from row in Owners_table.AsEnumerable()
                select new GetDatabase_Result_Owners(
                    row.Field<System.Int64>("Id"),
                    row.Field<System.Int64>("CompanyId"),
                    row.Field<System.String>("FirstName"),
                    row.Field<System.String>("LastName"),
                    row.Field<System.DateTime>("DateOfBirth")
                );
            
            var Cars_table = ds.Tables[2];

            var CarsResult = from row in Cars_table.AsEnumerable()
                select new GetDatabase_Result_Cars(
                    row.Field<System.Int64>("Id"),
                    row.Field<System.Int64>("OwnerId"),
                    row.Field<System.String>("Make"),
                    row.Field<System.String>("Model")
                );

            return new GetDatabase_Result(
                CompaniesResult,
                OwnersResult,
                CarsResult
            );

        }
    }
}