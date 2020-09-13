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
	public partial interface INullableTableRepository : IPkRepository<NullableTableDto>
	{
		NullableTableDto Get(int id);
		IEnumerable<NullableTableDto> Get(List<int> ids);
		IEnumerable<NullableTableDto> Get(params int[] ids);
		int GetMaxId();
		bool Delete(int id);
		bool Delete(IEnumerable<int> ids);
		bool DeleteByAge(int age);
		bool DeleteByDoB(DateTime dob);
		bool DeleteBylolVal(Guid lolval);
		IEnumerable<NullableTableDto> Search(
			int? age = null,
			DateTime? dob = null,
			Guid? lolval = null);
		IEnumerable<NullableTableDto> FindByAge(int age);
		IEnumerable<NullableTableDto> FindByAge(FindComparison comparison, int age);
		IEnumerable<NullableTableDto> FindByDoB(DateTime dob);
		IEnumerable<NullableTableDto> FindByDoB(FindComparison comparison, DateTime dob);
		IEnumerable<NullableTableDto> FindBylolVal(Guid lolval);
		IEnumerable<NullableTableDto> FindBylolVal(FindComparison comparison, Guid lolval);
	}
	public sealed partial class NullableTableRepository : BaseRepository<NullableTableDto>, INullableTableRepository
	{
		partial void InitializeExtension();
		public NullableTableRepository(string connectionString) : this(connectionString, exception => { }) { }
		public NullableTableRepository(string connectionString, bool useCache, int cacheDurationInSeconds) : this(connectionString, exception => { }, useCache, cacheDurationInSeconds) { }
		public NullableTableRepository(string connectionString, Action<Exception> logMethod) : this(connectionString, logMethod, false, 0) { }
		public NullableTableRepository(string connectionString, Action<Exception> logMethod, bool useCache, int cacheDurationInSeconds) : base(connectionString, logMethod,
			NullableTableDto.Schema, NullableTableDto.TableName, NullableTableDto.Columns, useCache, cacheDurationInSeconds)
		{
			InitializeExtension();
		}

		public NullableTableDto Get(int id)
		{
			return Where("Id", Comparison.Equals, id).Results().FirstOrDefault();
		}

		public IEnumerable<NullableTableDto> Get(List<int> ids)
		{
			return Get(ids.ToArray());
		}

		public IEnumerable<NullableTableDto> Get(params int[] ids)
		{
			return Where("Id", Comparison.In, ids).Results();
		}

		public int GetMaxId()
		{
			using (var cn = new SqlConnection(ConnectionString))
			{
				using (var cmd = CreateCommand(cn, "SELECT ISNULL(MAX(Id), 0) FROM NullableTable"))
				{
					cn.Open();
					return (int)cmd.ExecuteScalar();
				}
			}
		}
		public override bool Create(NullableTableDto item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.Age, item.DoB, item.lolVal);
			if (createdKeys.Count != NullableTableDto.Columns.Count(x => x.PrimaryKey))
				return false;

			item.Id = (int)createdKeys[nameof(NullableTableDto.Id)];
			item.ResetDirty();

				if (CacheEnabled)
				{
					SaveToCache(item);
				}
			return true;
		}

		public override bool BulkCreate(params NullableTableDto[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in NullableTableDto.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.Age, item.DoB, item.lolVal); 
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
		public override bool BulkCreate(List<NullableTableDto> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(NullableTableDto item, bool clearDirty = true)
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
				item.Id, item.Age, item.DoB, item.lolVal);

			if (success && clearDirty)
				item.ResetDirty();
			if (success && CacheEnabled)
			{
				SaveToCache(item);
			}

			return success;
		}
		public bool Delete(NullableTableDto nullabletabledto)
		{
			if (nullabletabledto == null)
				return false;

			var deleteColumn = new DeleteColumn("Id", nullabletabledto.Id, SqlDbType.Int);

			return BaseDelete(deleteColumn, out var items);
		}
		public bool Delete(IEnumerable<NullableTableDto> items)
		{
			if (!items.Any()) return true;
			var deleteValues = new List<object>();
			foreach (var item in items)
			{
				deleteValues.Add(item.Id);
			}

			return BaseDelete("Id", deleteValues);
		}

		public bool Delete(int id)
		{
			return Delete(new NullableTableDto { Id = id });
		}


		public bool Delete(IEnumerable<int> ids)
		{
			if (!ids.Any()) return true;
			var deleteValues = new List<object>();
			deleteValues.AddRange(ids.Cast<object>()); 
			return BaseDelete("Id", deleteValues);
		}

		public bool DeleteByAge(int age)
		{
			if (BaseDelete(new DeleteColumn("Age", age, SqlDbType.Int), out var items))
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
		public bool DeleteByDoB(DateTime dob)
		{
			if (BaseDelete(new DeleteColumn("DoB", dob, SqlDbType.DateTime), out var items))
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
		public bool DeleteBylolVal(Guid lolval)
		{
			if (BaseDelete(new DeleteColumn("lolVal", lolval, SqlDbType.UniqueIdentifier), out var items))
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

		public bool Merge(List<NullableTableDto> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.Id,
					item.Age, item.DirtyColumns.Contains("Age"),
					item.DoB, item.DirtyColumns.Contains("DoB"),
					item.lolVal, item.DirtyColumns.Contains("lolVal")
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
						Cast<int>(blocks[1]), true,
						Cast<DateTime>(blocks[2]), true,
						Cast<Guid>(blocks[3]), true,
					});
				} while ((line = sr.ReadLine()) != null);

				
				return BaseMerge(mergeTable);
			}
		}

		public override NullableTableDto ToItem(DataRow row, bool skipBase)
		{
			var item = new NullableTableDto
			{
				Id = GetInt32(row, "Id"),
				Age = GetNullableInt32(row, "Age"),
				DoB = GetNullableDateTime(row, "DoB"),
				lolVal = GetNullableGuid(row, "lolVal"),
			};

			item.ResetDirty();
			return item;
		}

		public override TK ToItem<TK>(DataRow row, bool skipBase)
		{
			var item = new TK
			{
				Id = GetInt32(row, "Id"),
				Age = GetNullableInt32(row, "Age"),
				DoB = GetNullableDateTime(row, "DoB"),
				lolVal = GetNullableGuid(row, "lolVal"),
			};

			item.ResetDirty();
			return item as TK;
		}

		public IEnumerable<NullableTableDto> Search(
			int? age = null,
			DateTime? dob = null,
			Guid? lolval = null)
		{
			var queries = new List<QueryItem>(); 

			if (age.HasValue)
				queries.Add(new QueryItem("Age", age));
			if (dob.HasValue)
				queries.Add(new QueryItem("DoB", dob));
			if (lolval.HasValue)
				queries.Add(new QueryItem("lolVal", lolval));

			return BaseSearch(queries);
		}

		public IEnumerable<NullableTableDto> FindByAge(int age)
		{
			return FindByAge(FindComparison.Equals, age);
		}

		public IEnumerable<NullableTableDto> FindByAge(FindComparison comparison, int age)
		{
			var items = Where("Age", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), age).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<NullableTableDto> FindByDoB(DateTime dob)
		{
			return FindByDoB(FindComparison.Equals, dob);
		}

		public IEnumerable<NullableTableDto> FindByDoB(FindComparison comparison, DateTime dob)
		{
			var items = Where("DoB", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), dob).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<NullableTableDto> FindBylolVal(Guid lolval)
		{
			return FindBylolVal(FindComparison.Equals, lolval);
		}

		public IEnumerable<NullableTableDto> FindBylolVal(FindComparison comparison, Guid lolval)
		{
			var items = Where("lolVal", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), lolval).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}
		private void SaveToCache(NullableTableDto nullabletabledto)
		{
			CacheHelper.SaveToCache(NullableTableDto.CacheKey(nullabletabledto.Id), nullabletabledto);
		}
		private NullableTableDto GetFromCache(System.Int32 id)
		{
			return CacheHelper.GetFromCache<NullableTableDto>(NullableTableDto.CacheKey(id));
		}
		private void RemoveFromCache(System.Int32 id)
		{
			CacheHelper.RemoveFromCache(NullableTableDto.CacheKey(id));
		}
		private bool IsInCache(System.Int32 id)
		{
			return CacheHelper.IsInCache(NullableTableDto.CacheKey(id));
		}

	}
}
