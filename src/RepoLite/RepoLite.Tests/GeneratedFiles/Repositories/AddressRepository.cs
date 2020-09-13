using NS.Base;
using NS.Models;
using NS.Models.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;

namespace NS
{
	public class AddressDtoKeys
	{
		public int Id { get; set; }
		public string AnotherId { get; set; }
		public AddressDtoKeys() {}
		public AddressDtoKeys(
			int id,
			string anotherid)
		{
			Id = id;
			AnotherId = anotherid;
		}
	}
	public partial interface IAddressRepository : IBaseRepository<AddressDto>
	{
		AddressDto Get(int id, string anotherid);
		AddressDto Get(AddressDtoKeys compositeId);
		IEnumerable<AddressDto> Get(List<AddressDtoKeys> compositeIds);
		IEnumerable<AddressDto> Get(params AddressDtoKeys[] compositeIds);
		bool DeleteByPersonId(int personid);
		bool DeleteByLine1(string line1);
		bool DeleteByLine2(string line2);
		bool DeleteByLine3(string line3);
		bool DeleteByLine4(string line4);
		bool DeleteByPostCode(string postcode);
		bool DeleteByPhoneNumber(string phonenumber);
		bool DeleteByCOUNTRY_CODE(string country_code);
		bool Update(AddressDto item, bool clearDirty = true);
		bool Delete(AddressDto addressdto);
		bool Delete(int id, string anotherid);
		bool Delete(AddressDtoKeys compositeId);
		bool Delete(IEnumerable<AddressDtoKeys> compositeIds);
		bool Merge(List<AddressDto> items);
		bool Merge(string csvPath);
		IEnumerable<AddressDto> Search(
			int? personid = null,
			string line1 = null,
			string line2 = null,
			string line3 = null,
			string line4 = null,
			string postcode = null,
			string phonenumber = null,
			string country_code = null);
		IEnumerable<AddressDto> FindById(int id);
		IEnumerable<AddressDto> FindById(FindComparison comparison, int id);
		IEnumerable<AddressDto> FindByAnotherId(string anotherid);
		IEnumerable<AddressDto> FindByAnotherId(FindComparison comparison, string anotherid);
		IEnumerable<AddressDto> FindByPersonId(int personid);
		IEnumerable<AddressDto> FindByPersonId(FindComparison comparison, int personid);
		IEnumerable<AddressDto> FindByLine1(string line1);
		IEnumerable<AddressDto> FindByLine1(FindComparison comparison, string line1);
		IEnumerable<AddressDto> FindByLine2(string line2);
		IEnumerable<AddressDto> FindByLine2(FindComparison comparison, string line2);
		IEnumerable<AddressDto> FindByLine3(string line3);
		IEnumerable<AddressDto> FindByLine3(FindComparison comparison, string line3);
		IEnumerable<AddressDto> FindByLine4(string line4);
		IEnumerable<AddressDto> FindByLine4(FindComparison comparison, string line4);
		IEnumerable<AddressDto> FindByPostCode(string postcode);
		IEnumerable<AddressDto> FindByPostCode(FindComparison comparison, string postcode);
		IEnumerable<AddressDto> FindByPhoneNumber(string phonenumber);
		IEnumerable<AddressDto> FindByPhoneNumber(FindComparison comparison, string phonenumber);
		IEnumerable<AddressDto> FindByCOUNTRY_CODE(string country_code);
		IEnumerable<AddressDto> FindByCOUNTRY_CODE(FindComparison comparison, string country_code);
	}
	public sealed partial class AddressRepository : BaseRepository<AddressDto>, IAddressRepository
	{
		partial void InitializeExtension();
		public AddressRepository(string connectionString) : this(connectionString, exception => { }) { }
		public AddressRepository(string connectionString, bool useCache, int cacheDurationInSeconds) : this(connectionString, exception => { }, useCache, cacheDurationInSeconds) { }
		public AddressRepository(string connectionString, Action<Exception> logMethod) : this(connectionString, logMethod, false, 0) { }
		public AddressRepository(string connectionString, Action<Exception> logMethod, bool useCache, int cacheDurationInSeconds) : base(connectionString, logMethod,
			AddressDto.Schema, AddressDto.TableName, AddressDto.Columns, useCache, cacheDurationInSeconds)
		{
			InitializeExtension();
		}

		public AddressDto Get(int id, string anotherid)
		{
			return Where("Id", Comparison.Equals, id).And("AnotherId", Comparison.Equals, anotherid).Results().FirstOrDefault();
		}

		public AddressDto Get(AddressDtoKeys compositeId)
		{
			return Where("Id", Comparison.Equals, compositeId.Id).And("AnotherId", Comparison.Equals, compositeId.AnotherId).Results().FirstOrDefault();
		}

		public IEnumerable<AddressDto> Get(List<AddressDtoKeys> compositeIds)
		{
			return Get(compositeIds.ToArray());
		}

		public IEnumerable<AddressDto> Get(params AddressDtoKeys[] compositeIds)
		{
			var result = Where("Id", Comparison.In, compositeIds.Select(x => x.Id).ToList()).Or("AnotherId", Comparison.In, compositeIds.Select(x => x.AnotherId).ToList()).Results().ToArray();
			var filteredResults = new List<AddressDto>();

			foreach (var compositeKey in compositeIds)
			{
				filteredResults.AddRange(result.Where(x => x.Id == compositeKey.Id && x.AnotherId == compositeKey.AnotherId));
			}
			return filteredResults;
		}

		public override bool Create(AddressDto item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.AnotherId, item.PersonId, item.Line1, item.Line2, 
				item.Line3, item.Line4, item.PostCode, item.PhoneNumber, item.COUNTRY_CODE);
			if (createdKeys.Count != AddressDto.Columns.Count(x => x.PrimaryKey))
				return false;

			item.Id = (int)createdKeys[nameof(AddressDto.Id)];
			item.AnotherId = (string)createdKeys[nameof(AddressDto.AnotherId)];
			item.ResetDirty();

				if (CacheEnabled)
				{
					SaveToCache(item);
				}
			return true;
		}

		public override bool BulkCreate(params AddressDto[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in AddressDto.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.AnotherId, item.PersonId, item.Line1, item.Line2, item.Line3, 
				item.Line4, item.PostCode, item.PhoneNumber, item.COUNTRY_CODE); 
			}

			if (BulkInsert(dt))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						SaveToCache(item);
					}
				}
				return true;
			}
			return false;
		}
		public override bool BulkCreate(List<AddressDto> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(AddressDto item, bool clearDirty = true)
		{
			if (item == null)
				return false;
			if (CacheEnabled)
			{
				RemoveFromCache(item.Id);
			}

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.Id, item.AnotherId, item.PersonId, item.Line1, item.Line2, 
				item.Line3, item.Line4, item.PostCode, item.PhoneNumber, item.COUNTRY_CODE);

			if (success && clearDirty)
				item.ResetDirty();
			if (success && CacheEnabled)
			{
				SaveToCache(item);
			}

			return success;
		}
		public bool Delete(AddressDto addressdto)
		{
			if (addressdto == null)
				return false;

			var deleteColumn = new DeleteColumn("Id", addressdto.Id, SqlDbType.Int);

			return BaseDelete(deleteColumn, out var items);
		}

		public bool Delete(int id, string anotherid)
		{
			return Delete(new AddressDto { Id = id,AnotherId = anotherid});
		}

		public bool Delete(AddressDtoKeys compositeId)
		{
			return Delete(new AddressDto { Id = compositeId.Id,AnotherId = compositeId.AnotherId});
		}

		public bool Delete(IEnumerable<AddressDtoKeys> compositeIds)
		{
			var tempTableName = $"staging{DateTime.Now.Ticks}";
			var dt = new DataTable();
			foreach (var mergeColumn in AddressDto.Columns.Where(x => x.PrimaryKey))
			{
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);
			}
			foreach (var compositeId in compositeIds)
			{
				dt.Rows.Add(compositeId.Id,compositeId.AnotherId);
			}
			CreateStagingTable(tempTableName, AddressDto.Columns, true);
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

		public bool DeleteByPersonId(int personid)
		{
			if (BaseDelete(new DeleteColumn("PersonId", personid, SqlDbType.Int), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByLine1(string line1)
		{
			if (BaseDelete(new DeleteColumn("Line1", line1, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByLine2(string line2)
		{
			if (BaseDelete(new DeleteColumn("Line2", line2, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByLine3(string line3)
		{
			if (BaseDelete(new DeleteColumn("Line3", line3, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByLine4(string line4)
		{
			if (BaseDelete(new DeleteColumn("Line4", line4, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByPostCode(string postcode)
		{
			if (BaseDelete(new DeleteColumn("PostCode", postcode, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByPhoneNumber(string phonenumber)
		{
			if (BaseDelete(new DeleteColumn("PhoneNumber", phonenumber, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByCOUNTRY_CODE(string country_code)
		{
			if (BaseDelete(new DeleteColumn("COUNTRY_CODE", country_code, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}

		public bool Merge(List<AddressDto> items)
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
			var mergeTable = new List<object[]>();
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
					mergeTable.Add(new object[]
					{
						Cast<int>(blocks[0]),
						Cast<string>(blocks[1]),
						Cast<int>(blocks[2]), true,
						Cast<string>(blocks[3]), true,
						Cast<string>(blocks[4]), true,
						Cast<string>(blocks[5]), true,
						Cast<string>(blocks[6]), true,
						Cast<string>(blocks[7]), true,
						Cast<string>(blocks[8]), true,
						Cast<string>(blocks[9]), true,
					});
				} while ((line = sr.ReadLine()) != null);

				
				return BaseMerge(mergeTable);
			}
		}

		public override AddressDto ToItem(DataRow row, bool skipBase)
		{
			var item = new AddressDto
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

		public override TK ToItem<TK>(DataRow row, bool skipBase)
		{
			var item = new TK
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
			return item as TK;
		}

		public IEnumerable<AddressDto> Search(
			int? personid = null,
			string line1 = null,
			string line2 = null,
			string line3 = null,
			string line4 = null,
			string postcode = null,
			string phonenumber = null,
			string country_code = null)
		{
			var queries = new List<QueryItem>(); 

			if (personid.HasValue)
				queries.Add(new QueryItem("PersonId", personid));
			if (line1 != null)
				queries.Add(new QueryItem("Line1", line1));
			if (line2 != null)
				queries.Add(new QueryItem("Line2", line2));
			if (line3 != null)
				queries.Add(new QueryItem("Line3", line3));
			if (line4 != null)
				queries.Add(new QueryItem("Line4", line4));
			if (postcode != null)
				queries.Add(new QueryItem("PostCode", postcode));
			if (phonenumber != null)
				queries.Add(new QueryItem("PhoneNumber", phonenumber));
			if (country_code != null)
				queries.Add(new QueryItem("COUNTRY_CODE", country_code));

			return BaseSearch(queries);
		}


		public IEnumerable<AddressDto> FindById(int id)
		{
			var items = FindById(FindComparison.Equals, id);
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindById(FindComparison comparison, int id)
		{
			return Where("Id", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), id).Results();
		}

		public IEnumerable<AddressDto> FindByAnotherId(string anotherid)
		{
			var items = FindByAnotherId(FindComparison.Equals, anotherid);
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindByAnotherId(FindComparison comparison, string anotherid)
		{
			return Where("AnotherId", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), anotherid).Results();
		}

		public IEnumerable<AddressDto> FindByPersonId(int personid)
		{
			return FindByPersonId(FindComparison.Equals, personid);
		}

		public IEnumerable<AddressDto> FindByPersonId(FindComparison comparison, int personid)
		{
			var items = Where("PersonId", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), personid).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindByLine1(string line1)
		{
			return FindByLine1(FindComparison.Equals, line1);
		}

		public IEnumerable<AddressDto> FindByLine1(FindComparison comparison, string line1)
		{
			var items = Where("Line1", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), line1).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindByLine2(string line2)
		{
			return FindByLine2(FindComparison.Equals, line2);
		}

		public IEnumerable<AddressDto> FindByLine2(FindComparison comparison, string line2)
		{
			var items = Where("Line2", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), line2).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindByLine3(string line3)
		{
			return FindByLine3(FindComparison.Equals, line3);
		}

		public IEnumerable<AddressDto> FindByLine3(FindComparison comparison, string line3)
		{
			var items = Where("Line3", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), line3).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindByLine4(string line4)
		{
			return FindByLine4(FindComparison.Equals, line4);
		}

		public IEnumerable<AddressDto> FindByLine4(FindComparison comparison, string line4)
		{
			var items = Where("Line4", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), line4).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindByPostCode(string postcode)
		{
			return FindByPostCode(FindComparison.Equals, postcode);
		}

		public IEnumerable<AddressDto> FindByPostCode(FindComparison comparison, string postcode)
		{
			var items = Where("PostCode", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), postcode).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindByPhoneNumber(string phonenumber)
		{
			return FindByPhoneNumber(FindComparison.Equals, phonenumber);
		}

		public IEnumerable<AddressDto> FindByPhoneNumber(FindComparison comparison, string phonenumber)
		{
			var items = Where("PhoneNumber", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), phonenumber).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<AddressDto> FindByCOUNTRY_CODE(string country_code)
		{
			return FindByCOUNTRY_CODE(FindComparison.Equals, country_code);
		}

		public IEnumerable<AddressDto> FindByCOUNTRY_CODE(FindComparison comparison, string country_code)
		{
			var items = Where("COUNTRY_CODE", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), country_code).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}
		private void SaveToCache(AddressDto addressdto)
		{
			CacheHelper.SaveToCache(AddressDto.CacheKey(addressdto.Id), addressdto);
		}
		private AddressDto GetFromCache(System.Int32 id)
		{
			return CacheHelper.GetFromCache<AddressDto>(AddressDto.CacheKey(id));
		}
		private void RemoveFromCache(System.Int32 id)
		{
			CacheHelper.RemoveFromCache(AddressDto.CacheKey(id));
		}
		private bool IsInCache(System.Int32 id)
		{
			return CacheHelper.IsInCache(AddressDto.CacheKey(id));
		}

	}
}
