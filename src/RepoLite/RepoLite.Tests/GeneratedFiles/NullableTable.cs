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
	public partial interface INullableTableRepository : IPkRepository<NullableTable>
	{
		NullableTable Get(Int32 id);
		IEnumerable<NullableTable> Get(List<Int32> ids);
		IEnumerable<NullableTable> Get(params Int32[] ids);
		Int32 GetMaxId();
		bool DeleteById(Int32 id);
		bool DeleteByAge(Int32 age);
		bool DeleteByDoB(DateTime dob);
		bool DeleteBylolVal(Guid lolval);
		IEnumerable<NullableTable> Search(
			Int32? id = null,
			Int32? age = null,
			DateTime? dob = null,
			Guid? lolval = null);
		IEnumerable<NullableTable> FindByAge(Int32 age);
		IEnumerable<NullableTable> FindByAge(FindComparison comparison, Int32 age);
		IEnumerable<NullableTable> FindByDoB(DateTime dob);
		IEnumerable<NullableTable> FindByDoB(FindComparison comparison, DateTime dob);
		IEnumerable<NullableTable> FindBylolVal(Guid lolval);
		IEnumerable<NullableTable> FindBylolVal(FindComparison comparison, Guid lolval);
	}
	public sealed partial class NullableTableRepository : BaseRepository<NullableTable>, INullableTableRepository
	{
		public NullableTableRepository(string connectionString) : this(connectionString, exception => { }) { }
		public NullableTableRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "NullableTable", NullableTable.Columns)
		{
		}

		public NullableTable Get(Int32 id)
		{
			return Where("Id", Comparison.Equals, id).Results().FirstOrDefault();
		}

		public IEnumerable<NullableTable> Get(List<Int32> ids)
		{
			return Get(ids.ToArray());
		}

		public IEnumerable<NullableTable> Get(params Int32[] ids)
		{
			return Where("Id", Comparison.In, ids).Results();
		}

		public Int32 GetMaxId()
		{
			using (var cn = new SqlConnection(ConnectionString))
			{
				using (var cmd = CreateCommand(cn, "SELECT MAX(Id) FROM NullableTable"))
				{
					cn.Open();
					return (Int32)cmd.ExecuteScalar();
				}
			}
		}
		public override bool Create(NullableTable item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.Age, item.DoB, item.lolVal);
			if (createdKeys.Count != NullableTable.Columns.Count(x => x.PrimaryKey))
				return false;

			item.Id = (Int32)createdKeys[nameof(NullableTable.Id)];
			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params NullableTable[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in NullableTable.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.Age, item.DoB, item.lolVal); 
			}

			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<NullableTable> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(NullableTable item)
		{
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.Id, item.Age, item.DoB, item.lolVal);

			if (success)
			item.ResetDirty();

			return success;
		}
		public bool Delete(NullableTable nullabletable)
		{
			if (nullabletable == null)
				return false;

			var deleteColumn = new DeleteColumn("Id", nullabletable.Id, SqlDbType.Int);

			return BaseDelete(deleteColumn);
		}
		public bool Delete(IEnumerable<NullableTable> items)
		{
			if (!items.Any()) return true;
			var deleteValues = new List<object>();
			foreach (var item in items)
			{
				deleteValues.Add(item.Id);
			}

			return BaseDelete("Id", deleteValues);
		}

		public bool Delete(Int32 id)
		{
			return Delete(new NullableTable { Id = id });
		}


		public bool Delete(IEnumerable<Int32> ids)
		{
			return Delete(ids.Select(x => new NullableTable { Id = x }));
		}

		public bool DeleteById(Int32 id)
		{
			return BaseDelete(new DeleteColumn("Id", id, SqlDbType.Int));
		}
		public bool DeleteByAge(Int32 age)
		{
			return BaseDelete(new DeleteColumn("Age", age, SqlDbType.Int));
		}
		public bool DeleteByDoB(DateTime dob)
		{
			return BaseDelete(new DeleteColumn("DoB", dob, SqlDbType.DateTime));
		}
		public bool DeleteBylolVal(Guid lolval)
		{
			return BaseDelete(new DeleteColumn("lolVal", lolval, SqlDbType.UniqueIdentifier));
		}

		public bool Merge(List<NullableTable> items)
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
						Cast<Int32>(blocks[0]),
						Cast<Int32>(blocks[1]), true,
						Cast<DateTime>(blocks[2]), true,
						Cast<Guid>(blocks[3]), true,
					});
				} while ((line = sr.ReadLine()) != null);

				
				return BaseMerge(mergeTable);
			}
		}

		protected override NullableTable ToItem(DataRow row)
		{
			 var item = new NullableTable
			{
				Id = GetInt32(row, "Id"),
				Age = GetNullableInt32(row, "Age"),
				DoB = GetNullableDateTime(row, "DoB"),
				lolVal = GetNullableGuid(row, "lolVal"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<NullableTable> Search(
			Int32? id = null,
			Int32? age = null,
			DateTime? dob = null,
			Guid? lolval = null)
		{
			var queries = new List<QueryItem>(); 

			if (id.HasValue)
				queries.Add(new QueryItem("Id", id));
			if (age.HasValue)
				queries.Add(new QueryItem("Age", age));
			if (dob.HasValue)
				queries.Add(new QueryItem("DoB", dob));
			if (lolval.HasValue)
				queries.Add(new QueryItem("lolVal", lolval));

			return BaseSearch(queries);
		}

		public IEnumerable<NullableTable> FindByAge(Int32 age)
		{
			return FindByAge(FindComparison.Equals, age);
		}

		public IEnumerable<NullableTable> FindByAge(FindComparison comparison, Int32 age)
		{
			return Where("Age", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), age).Results();
		}

		public IEnumerable<NullableTable> FindByDoB(DateTime dob)
		{
			return FindByDoB(FindComparison.Equals, dob);
		}

		public IEnumerable<NullableTable> FindByDoB(FindComparison comparison, DateTime dob)
		{
			return Where("DoB", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), dob).Results();
		}

		public IEnumerable<NullableTable> FindBylolVal(Guid lolval)
		{
			return FindBylolVal(FindComparison.Equals, lolval);
		}

		public IEnumerable<NullableTable> FindBylolVal(FindComparison comparison, Guid lolval)
		{
			return Where("lolVal", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), lolval).Results();
		}
	}
}
