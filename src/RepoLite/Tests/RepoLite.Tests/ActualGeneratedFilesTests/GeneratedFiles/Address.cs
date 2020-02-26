using NS.Base;
using NS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;

namespace NS
{
	public class AddressKeys
	{
		public Int32 Id { get; set; }
		public String AnotherId { get; set; }
		public AddressKeys() {}
		public AddressKeys(
			Int32 id,
			String anotherid)
		{
			Id = id;
			AnotherId = anotherid;
		}
	}
	public partial interface IAddressRepository : IBaseRepository<Address>
	{
		Address Get(Int32 id, String anotherid);
		Address Get(AddressKeys compositeId);
		IEnumerable<Address> Get(List<AddressKeys> compositeIds);
		IEnumerable<Address> Get(params AddressKeys[] compositeIds);
		bool DeleteById(Int32 id);
		bool DeleteByAnotherId(String anotherid);
		bool DeleteByPersonId(Int32 personid);
		bool DeleteByLine1(String line1);
		bool DeleteByLine2(String line2);
		bool DeleteByLine3(String line3);
		bool DeleteByLine4(String line4);
		bool DeleteByPostCode(String postcode);
		bool DeleteByPhoneNumber(String phonenumber);
		bool DeleteByCOUNTRY_CODE(String country_code);
		bool Update(Address item);
		bool Delete(Address address);
		bool Delete(Int32 id, String anotherid);
		bool Delete(AddressKeys compositeId);
		bool Delete(IEnumerable<AddressKeys> compositeIds);
		bool Merge(List<Address> items);
		bool Merge(string csvPath);
		IEnumerable<Address> Search(
			Int32? id = null,
			String anotherid = null,
			Int32? personid = null,
			String line1 = null,
			String line2 = null,
			String line3 = null,
			String line4 = null,
			String postcode = null,
			String phonenumber = null,
			String country_code = null);
		IEnumerable<Address> FindById(Int32 id);
		IEnumerable<Address> FindById(FindComparison comparison, Int32 id);
		IEnumerable<Address> FindByAnotherId(String anotherid);
		IEnumerable<Address> FindByAnotherId(FindComparison comparison, String anotherid);
		IEnumerable<Address> FindByPersonId(Int32 personid);
		IEnumerable<Address> FindByPersonId(FindComparison comparison, Int32 personid);
		IEnumerable<Address> FindByLine1(String line1);
		IEnumerable<Address> FindByLine1(FindComparison comparison, String line1);
		IEnumerable<Address> FindByLine2(String line2);
		IEnumerable<Address> FindByLine2(FindComparison comparison, String line2);
		IEnumerable<Address> FindByLine3(String line3);
		IEnumerable<Address> FindByLine3(FindComparison comparison, String line3);
		IEnumerable<Address> FindByLine4(String line4);
		IEnumerable<Address> FindByLine4(FindComparison comparison, String line4);
		IEnumerable<Address> FindByPostCode(String postcode);
		IEnumerable<Address> FindByPostCode(FindComparison comparison, String postcode);
		IEnumerable<Address> FindByPhoneNumber(String phonenumber);
		IEnumerable<Address> FindByPhoneNumber(FindComparison comparison, String phonenumber);
		IEnumerable<Address> FindByCOUNTRY_CODE(String country_code);
		IEnumerable<Address> FindByCOUNTRY_CODE(FindComparison comparison, String country_code);
	}
	public sealed partial class AddressRepository : BaseRepository<Address>, IAddressRepository
	{
		public AddressRepository(string connectionString) : this(connectionString, exception => { }) { }
		public AddressRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "Address", Address.Columns)
		{
		}

		public Address Get(Int32 id, String anotherid)
		{
			return Where("Id", Comparison.Equals, id).And("AnotherId", Comparison.Equals, anotherid).Results().FirstOrDefault();
		}

		public Address Get(AddressKeys compositeId)
		{
			return Where("Id", Comparison.Equals, compositeId.Id).And("AnotherId", Comparison.Equals, compositeId.AnotherId).Results().FirstOrDefault();
		}

		public IEnumerable<Address> Get(List<AddressKeys> compositeIds)
		{
			return Get(compositeIds.ToArray());
		}

		public IEnumerable<Address> Get(params AddressKeys[] compositeIds)
		{
			var result = Where("Id", Comparison.In, compositeIds.Select(x => x.Id).ToList()).Or("AnotherId", Comparison.In, compositeIds.Select(x => x.AnotherId).ToList()).Results().ToArray();
			var filteredResults = new List<Address>();

			foreach (var compositeKey in compositeIds)
			{
				filteredResults.AddRange(result.Where(x => x.Id == compositeKey.Id && x.AnotherId == compositeKey.AnotherId));
			}
			return filteredResults;
		}

		public override bool Create(Address item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.AnotherId, item.PersonId, item.Line1, item.Line2, 
				item.Line3, item.Line4, item.PostCode, item.PhoneNumber, item.COUNTRY_CODE);
			if (createdKeys.Count != Address.Columns.Count(x => x.PrimaryKey))
				return false;

			item.Id = (Int32)createdKeys[nameof(Address.Id)];
			item.AnotherId = (String)createdKeys[nameof(Address.AnotherId)];
			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params Address[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in Address.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.AnotherId, item.PersonId, item.Line1, item.Line2, item.Line3, 
				item.Line4, item.PostCode, item.PhoneNumber, item.COUNTRY_CODE); 
			}

			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<Address> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(Address item)
		{
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.Id, item.AnotherId, item.PersonId, item.Line1, item.Line2, 
				item.Line3, item.Line4, item.PostCode, item.PhoneNumber, item.COUNTRY_CODE);

			if (success)
			item.ResetDirty();

			return success;
		}
		public bool Delete(Address address)
		{
			if (address == null)
				return false;

			var deleteColumn = new DeleteColumn("Id", address.Id, SqlDbType.Int);

			return BaseDelete(deleteColumn);
		}

		public bool Delete(Int32 id, String anotherid)
		{
			return Delete(new Address { Id = id,AnotherId = anotherid});
		}

		public bool Delete(AddressKeys compositeId)
		{
			return Delete(new Address { Id = compositeId.Id,AnotherId = compositeId.AnotherId});
		}

		public bool Delete(IEnumerable<AddressKeys> compositeIds)
		{
			var tempTableName = $"staging{DateTime.Now.Ticks}";
			var dt = new DataTable();
			foreach (var mergeColumn in Address.Columns.Where(x => x.PrimaryKey))
			{
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);
			}
			foreach (var compositeId in compositeIds)
			{
				dt.Rows.Add(compositeId.Id,compositeId.AnotherId);
			}
			CreateStagingTable(tempTableName, true);
			BulkInsert(dt, tempTableName);
			using (var cn = new SqlConnection(ConnectionString))
			{
				using (var cmd = CreateCommand(cn, $@";WITH cte AS (SELECT * FROM dbo.Address o
							WHERE EXISTS (SELECT 'x' FROM {tempTableName} i WHERE i.[Id] = o.[Id] AND i.[AnotherId] = o.[AnotherId]))
							DELETE FROM cte"))
				{
					try
					{
						cn.Open();
						return (int)cmd.ExecuteScalar() > 0;
					}
					finally { cn.Close(); }
				}
			}
		}

		public bool DeleteById(Int32 id)
		{
			return BaseDelete(new DeleteColumn("Id", id, SqlDbType.Int));
		}
		public bool DeleteByAnotherId(String anotherid)
		{
			return BaseDelete(new DeleteColumn("AnotherId", anotherid, SqlDbType.NVarChar));
		}
		public bool DeleteByPersonId(Int32 personid)
		{
			return BaseDelete(new DeleteColumn("PersonId", personid, SqlDbType.Int));
		}
		public bool DeleteByLine1(String line1)
		{
			return BaseDelete(new DeleteColumn("Line1", line1, SqlDbType.NVarChar));
		}
		public bool DeleteByLine2(String line2)
		{
			return BaseDelete(new DeleteColumn("Line2", line2, SqlDbType.NVarChar));
		}
		public bool DeleteByLine3(String line3)
		{
			return BaseDelete(new DeleteColumn("Line3", line3, SqlDbType.NVarChar));
		}
		public bool DeleteByLine4(String line4)
		{
			return BaseDelete(new DeleteColumn("Line4", line4, SqlDbType.NVarChar));
		}
		public bool DeleteByPostCode(String postcode)
		{
			return BaseDelete(new DeleteColumn("PostCode", postcode, SqlDbType.NVarChar));
		}
		public bool DeleteByPhoneNumber(String phonenumber)
		{
			return BaseDelete(new DeleteColumn("PhoneNumber", phonenumber, SqlDbType.NVarChar));
		}
		public bool DeleteByCOUNTRY_CODE(String country_code)
		{
			return BaseDelete(new DeleteColumn("COUNTRY_CODE", country_code, SqlDbType.NVarChar));
		}

		public bool Merge(List<Address> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.Id,
					item.AnotherId,
					item.PersonId, item.DirtyColumns.Contains("PersonId"),
					item.Line1, item.DirtyColumns.Contains("Line1"),
					item.Line2, item.DirtyColumns.Contains("Line2"),
					item.Line3, item.DirtyColumns.Contains("Line3"),
					item.Line4, item.DirtyColumns.Contains("Line4"),
					item.PostCode, item.DirtyColumns.Contains("PostCode"),
					item.PhoneNumber, item.DirtyColumns.Contains("PhoneNumber"),
					item.COUNTRY_CODE, item.DirtyColumns.Contains("COUNTRY_CODE")
				});
			}
			return BaseMerge(mergeTable);
		}

		public bool Merge(string csvPath)
		{
			var mergeTable = new List<Address>();
			using (var sr = new StreamReader(csvPath))
			{
				var line = sr.ReadLine();
				if (line == null) return false;

				var firstItem = line.Split(',')[0];
				if (firstItem == "Id")
				{
					//CSV has headers
					//Run to the next line
					line = sr.ReadLine();
					if (line == null) return true;
				}

				do
				{
					var blocks = line.Split(',');
					mergeTable.Add(new Address(blocks));
				} while ((line = sr.ReadLine()) != null);

				
				return Merge(mergeTable);
			}
		}

		protected override Address ToItem(DataRow row)
		{
			 var item = new Address
			{
				Id = GetInt32(row, "Id"),
				AnotherId = GetString(row, "AnotherId"),
				PersonId = GetNullableInt32(row, "PersonId"),
				Line1 = GetString(row, "Line1"),
				Line2 = GetString(row, "Line2"),
				Line3 = GetString(row, "Line3"),
				Line4 = GetString(row, "Line4"),
				PostCode = GetString(row, "PostCode"),
				PhoneNumber = GetString(row, "PhoneNumber"),
				COUNTRY_CODE = GetString(row, "COUNTRY_CODE"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<Address> Search(
			Int32? id = null,
			String anotherid = null,
			Int32? personid = null,
			String line1 = null,
			String line2 = null,
			String line3 = null,
			String line4 = null,
			String postcode = null,
			String phonenumber = null,
			String country_code = null)
		{
			var queries = new List<QueryItem>(); 

			if (id.HasValue)
				queries.Add(new QueryItem("Id", id));
			if (!string.IsNullOrEmpty(anotherid))
				queries.Add(new QueryItem("AnotherId", anotherid));
			if (personid.HasValue)
				queries.Add(new QueryItem("PersonId", personid));
			if (!string.IsNullOrEmpty(line1))
				queries.Add(new QueryItem("Line1", line1));
			if (!string.IsNullOrEmpty(line2))
				queries.Add(new QueryItem("Line2", line2));
			if (!string.IsNullOrEmpty(line3))
				queries.Add(new QueryItem("Line3", line3));
			if (!string.IsNullOrEmpty(line4))
				queries.Add(new QueryItem("Line4", line4));
			if (!string.IsNullOrEmpty(postcode))
				queries.Add(new QueryItem("PostCode", postcode));
			if (!string.IsNullOrEmpty(phonenumber))
				queries.Add(new QueryItem("PhoneNumber", phonenumber));
			if (!string.IsNullOrEmpty(country_code))
				queries.Add(new QueryItem("COUNTRY_CODE", country_code));

			return BaseSearch(queries);
		}


		public IEnumerable<Address> FindById(Int32 id)
		{
			return FindById(FindComparison.Equals, id);
		}

		public IEnumerable<Address> FindById(FindComparison comparison, Int32 id)
		{
			return Where("Id", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), id).Results();
		}

		public IEnumerable<Address> FindByAnotherId(String anotherid)
		{
			return FindByAnotherId(FindComparison.Equals, anotherid);
		}

		public IEnumerable<Address> FindByAnotherId(FindComparison comparison, String anotherid)
		{
			return Where("AnotherId", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), anotherid).Results();
		}

		public IEnumerable<Address> FindByPersonId(Int32 personid)
		{
			return FindByPersonId(FindComparison.Equals, personid);
		}

		public IEnumerable<Address> FindByPersonId(FindComparison comparison, Int32 personid)
		{
			return Where("PersonId", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), personid).Results();
		}

		public IEnumerable<Address> FindByLine1(String line1)
		{
			return FindByLine1(FindComparison.Equals, line1);
		}

		public IEnumerable<Address> FindByLine1(FindComparison comparison, String line1)
		{
			return Where("Line1", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), line1).Results();
		}

		public IEnumerable<Address> FindByLine2(String line2)
		{
			return FindByLine2(FindComparison.Equals, line2);
		}

		public IEnumerable<Address> FindByLine2(FindComparison comparison, String line2)
		{
			return Where("Line2", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), line2).Results();
		}

		public IEnumerable<Address> FindByLine3(String line3)
		{
			return FindByLine3(FindComparison.Equals, line3);
		}

		public IEnumerable<Address> FindByLine3(FindComparison comparison, String line3)
		{
			return Where("Line3", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), line3).Results();
		}

		public IEnumerable<Address> FindByLine4(String line4)
		{
			return FindByLine4(FindComparison.Equals, line4);
		}

		public IEnumerable<Address> FindByLine4(FindComparison comparison, String line4)
		{
			return Where("Line4", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), line4).Results();
		}

		public IEnumerable<Address> FindByPostCode(String postcode)
		{
			return FindByPostCode(FindComparison.Equals, postcode);
		}

		public IEnumerable<Address> FindByPostCode(FindComparison comparison, String postcode)
		{
			return Where("PostCode", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), postcode).Results();
		}

		public IEnumerable<Address> FindByPhoneNumber(String phonenumber)
		{
			return FindByPhoneNumber(FindComparison.Equals, phonenumber);
		}

		public IEnumerable<Address> FindByPhoneNumber(FindComparison comparison, String phonenumber)
		{
			return Where("PhoneNumber", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), phonenumber).Results();
		}

		public IEnumerable<Address> FindByCOUNTRY_CODE(String country_code)
		{
			return FindByCOUNTRY_CODE(FindComparison.Equals, country_code);
		}

		public IEnumerable<Address> FindByCOUNTRY_CODE(FindComparison comparison, String country_code)
		{
			return Where("COUNTRY_CODE", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), country_code).Results();
		}
	}
}
