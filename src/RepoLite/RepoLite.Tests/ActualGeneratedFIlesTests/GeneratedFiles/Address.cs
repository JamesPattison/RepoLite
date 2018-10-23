using NS.Base;
using NS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
			String anotherId)
		{
			Id = id;
			AnotherId = anotherId;
		}
	}
	public partial interface IAddressRepository : IBaseRepository<Address>
	{
		Address Get(Int32 id, String anotherId);
		Address Get(AddressKeys compositeId);
		IEnumerable<Address> Get(List<AddressKeys> compositeIds);
		IEnumerable<Address> Get(params AddressKeys[] compositeIds);
		bool Update(Address item);
		bool Delete(Address address);
		bool Delete(Int32 id, String anotherId);
		bool Delete(AddressKeys compositeId);
		bool Delete(IEnumerable<AddressKeys> compositeIds);
		bool Merge(List<Address> items);
		IEnumerable<Address> Search(
			Int32? id = null,
			String anotherId = null,
			Int32? personId = null,
			String line1 = null,
			String line2 = null,
			String line3 = null,
			String line4 = null,
			String postCode = null,
			String phoneNumber = null,
			String cOUNTRY_CODE = null);
		IEnumerable<Address> FindById(Int32 id);
		IEnumerable<Address> FindById(FindComparison comparison, Int32 id);
		IEnumerable<Address> FindByAnotherId(String anotherId);
		IEnumerable<Address> FindByAnotherId(FindComparison comparison, String anotherId);
		IEnumerable<Address> FindByPersonId(Int32 personId);
		IEnumerable<Address> FindByPersonId(FindComparison comparison, Int32 personId);
		IEnumerable<Address> FindByLine1(String line1);
		IEnumerable<Address> FindByLine1(FindComparison comparison, String line1);
		IEnumerable<Address> FindByLine2(String line2);
		IEnumerable<Address> FindByLine2(FindComparison comparison, String line2);
		IEnumerable<Address> FindByLine3(String line3);
		IEnumerable<Address> FindByLine3(FindComparison comparison, String line3);
		IEnumerable<Address> FindByLine4(String line4);
		IEnumerable<Address> FindByLine4(FindComparison comparison, String line4);
		IEnumerable<Address> FindByPostCode(String postCode);
		IEnumerable<Address> FindByPostCode(FindComparison comparison, String postCode);
		IEnumerable<Address> FindByPhoneNumber(String phoneNumber);
		IEnumerable<Address> FindByPhoneNumber(FindComparison comparison, String phoneNumber);
		IEnumerable<Address> FindByCOUNTRY_CODE(String cOUNTRY_CODE);
		IEnumerable<Address> FindByCOUNTRY_CODE(FindComparison comparison, String cOUNTRY_CODE);
	}
	public sealed partial class AddressRepository : BaseRepository<Address>, IAddressRepository
	{
		public AddressRepository(string connectionString) : this(connectionString, exception => { }) { }
		public AddressRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "Address", 10)
		{
			Columns.Add(new ColumnDefinition("Id", typeof(System.Int32), "[INT]", SqlDbType.Int, false, true, true));
			Columns.Add(new ColumnDefinition("AnotherId", typeof(System.String), "[NVARCHAR](10)", SqlDbType.NVarChar, false, true, false));
			Columns.Add(new ColumnDefinition("PersonId", typeof(System.Int32), "[INT]", SqlDbType.Int, false, false, false));
			Columns.Add(new ColumnDefinition("Line1", typeof(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, false, false, false));
			Columns.Add(new ColumnDefinition("Line2", typeof(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, true, false, false));
			Columns.Add(new ColumnDefinition("Line3", typeof(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, true, false, false));
			Columns.Add(new ColumnDefinition("Line4", typeof(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, true, false, false));
			Columns.Add(new ColumnDefinition("PostCode", typeof(System.String), "[NVARCHAR](15)", SqlDbType.NVarChar, false, false, false));
			Columns.Add(new ColumnDefinition("PhoneNumber", typeof(System.String), "[NVARCHAR](20)", SqlDbType.NVarChar, true, false, false));
			Columns.Add(new ColumnDefinition("COUNTRY_CODE", typeof(System.String), "[NVARCHAR](2)", SqlDbType.NVarChar, true, false, false));
		}

		public Address Get(Int32 id, String anotherId)
		{
			return Where("Id", Comparison.Equals, id).And("AnotherId", Comparison.Equals, anotherId).Results().FirstOrDefault();
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
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
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
			foreach (var mergeColumn in Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
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

		public bool Delete(Int32 id, String anotherId)
		{
			return Delete(new Address { Id = id,AnotherId = anotherId});
		}

		public bool Delete(AddressKeys compositeId)
		{
			return Delete(new Address { Id = compositeId.Id,AnotherId = compositeId.AnotherId});
		}

		public bool Delete(IEnumerable<AddressKeys> compositeIds)
		{
			var tempTableName = $"staging{DateTime.Now.Ticks}";
			var dt = new DataTable();
			foreach (var mergeColumn in Columns.Where(x => x.PrimaryKey))
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

		protected override Address ToItem(DataRow row)
		{
			 var item = new Address
			{
				Id = GetInt32(row, "Id"),
				AnotherId = GetString(row, "AnotherId"),
				PersonId = GetInt32(row, "PersonId"),
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
			String anotherId = null,
			Int32? personId = null,
			String line1 = null,
			String line2 = null,
			String line3 = null,
			String line4 = null,
			String postCode = null,
			String phoneNumber = null,
			String cOUNTRY_CODE = null)
		{
			var queries = new List<QueryItem>(); 

			if (id.HasValue)
				queries.Add(new QueryItem("Id", id));
			if (!string.IsNullOrEmpty(anotherId))
				queries.Add(new QueryItem("AnotherId", anotherId));
			if (personId.HasValue)
				queries.Add(new QueryItem("PersonId", personId));
			if (!string.IsNullOrEmpty(line1))
				queries.Add(new QueryItem("Line1", line1));
			if (!string.IsNullOrEmpty(line2))
				queries.Add(new QueryItem("Line2", line2));
			if (!string.IsNullOrEmpty(line3))
				queries.Add(new QueryItem("Line3", line3));
			if (!string.IsNullOrEmpty(line4))
				queries.Add(new QueryItem("Line4", line4));
			if (!string.IsNullOrEmpty(postCode))
				queries.Add(new QueryItem("PostCode", postCode));
			if (!string.IsNullOrEmpty(phoneNumber))
				queries.Add(new QueryItem("PhoneNumber", phoneNumber));
			if (!string.IsNullOrEmpty(cOUNTRY_CODE))
				queries.Add(new QueryItem("COUNTRY_CODE", cOUNTRY_CODE));

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

		public IEnumerable<Address> FindByAnotherId(String anotherId)
		{
			return FindByAnotherId(FindComparison.Equals, anotherId);
		}

		public IEnumerable<Address> FindByAnotherId(FindComparison comparison, String anotherId)
		{
			return Where("AnotherId", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), anotherId).Results();
		}

		public IEnumerable<Address> FindByPersonId(Int32 personId)
		{
			return FindByPersonId(FindComparison.Equals, personId);
		}

		public IEnumerable<Address> FindByPersonId(FindComparison comparison, Int32 personId)
		{
			return Where("PersonId", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), personId).Results();
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

		public IEnumerable<Address> FindByPostCode(String postCode)
		{
			return FindByPostCode(FindComparison.Equals, postCode);
		}

		public IEnumerable<Address> FindByPostCode(FindComparison comparison, String postCode)
		{
			return Where("PostCode", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), postCode).Results();
		}

		public IEnumerable<Address> FindByPhoneNumber(String phoneNumber)
		{
			return FindByPhoneNumber(FindComparison.Equals, phoneNumber);
		}

		public IEnumerable<Address> FindByPhoneNumber(FindComparison comparison, String phoneNumber)
		{
			return Where("PhoneNumber", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), phoneNumber).Results();
		}

		public IEnumerable<Address> FindByCOUNTRY_CODE(String cOUNTRY_CODE)
		{
			return FindByCOUNTRY_CODE(FindComparison.Equals, cOUNTRY_CODE);
		}

		public IEnumerable<Address> FindByCOUNTRY_CODE(FindComparison comparison, String cOUNTRY_CODE)
		{
			return Where("COUNTRY_CODE", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), cOUNTRY_CODE).Results();
		}
	}
}
