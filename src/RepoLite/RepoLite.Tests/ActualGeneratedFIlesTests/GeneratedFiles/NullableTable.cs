using NS.Base;
using NS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using Dapper;

namespace NS
{
	public partial interface INullableTableRepository : IPkRepository<NullableTable>
	{
		NullableTable Get(Int32 id);
		IEnumerable<NullableTable> Get(List<Int32> ids);
		IEnumerable<NullableTable> Get(params Int32[] ids);

		bool Update(NullableTable item);
		bool Delete(Int32 id);
		bool Delete(IEnumerable<Int32> ids);
		bool Merge(List<NullableTable> items);

		IEnumerable<NullableTable> Search(
			Int32? id = null,
			Int32? age = null,
			DateTime? doB = null,
			Guid? lolVal = null);

		IEnumerable<NullableTable> FindByAge(Int32 age);
		IEnumerable<NullableTable> FindByAge(FindComparison comparison, Int32 age);
		IEnumerable<NullableTable> FindByDoB(DateTime doB);
		IEnumerable<NullableTable> FindByDoB(FindComparison comparison, DateTime doB);
		IEnumerable<NullableTable> FindByLolVal(Guid lolVal);
		IEnumerable<NullableTable> FindByLolVal(FindComparison comparison, Guid lolVal);
	}
	public sealed partial class NullableTableRepository : BaseRepository<NullableTable>, INullableTableRepository
	{
		public NullableTableRepository(string connectionString) : this(connectionString, exception => { }) { }
		public NullableTableRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "NullableTable", 4)
		{
			Columns.Add(new ColumnDefinition("Id", typeof(System.Int32), "[INT]", false, true, true));
			Columns.Add(new ColumnDefinition("Age", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("DoB", typeof(System.DateTime), "[DATETIME]", true, false, false));
			Columns.Add(new ColumnDefinition("lolVal", typeof(System.Guid), "[UNIQUEIDENTIFIER]", true, false, false));
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

		public override bool Create(NullableTable item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.Age, item.DoB, item.LolVal);
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
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
			foreach (var mergeColumn in Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.Age, item.DoB, item.LolVal); 
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
				item.Id, item.Age, item.DoB, item.LolVal);

			if (success)
			item.ResetDirty();

			return success;
		}
		public bool Delete(NullableTable nullableTable)
		{
			if (nullableTable == null)
				return false;

			var deleteColumn = new DeleteColumn("Id", nullableTable.Id);

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
					item.LolVal, item.DirtyColumns.Contains("lolVal")
				});
			}
			return BaseMerge(mergeTable);
		}

		protected override NullableTable ToItem(DataRow row)
		{
			 var item = new NullableTable
			{
				Id = GetInt32(row, "Id"),
				Age = GetNullableInt32(row, "Age"),
				DoB = GetNullableDateTime(row, "DoB"),
				LolVal = GetNullableGuid(row, "lolVal"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<NullableTable> Search(
			Int32? id = null,
			Int32? age = null,
			DateTime? doB = null,
			Guid? lolVal = null)
		{
			var queries = new List<QueryItem>(); 

			if (id.HasValue)
				queries.Add(new QueryItem("Id", id));
			if (age.HasValue)
				queries.Add(new QueryItem("Age", age));
			if (doB.HasValue)
				queries.Add(new QueryItem("DoB", doB));
			if (lolVal.HasValue)
				queries.Add(new QueryItem("lolVal", lolVal));

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

		public IEnumerable<NullableTable> FindByDoB(DateTime doB)
		{
			return FindByDoB(FindComparison.Equals, doB);
		}

		public IEnumerable<NullableTable> FindByDoB(FindComparison comparison, DateTime doB)
		{
			return Where("DoB", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), doB).Results();
		}

		public IEnumerable<NullableTable> FindByLolVal(Guid lolVal)
		{
			return FindByLolVal(FindComparison.Equals, lolVal);
		}

		public IEnumerable<NullableTable> FindByLolVal(FindComparison comparison, Guid lolVal)
		{
			return Where("lolVal", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), lolVal).Results();
		}
	}
}
