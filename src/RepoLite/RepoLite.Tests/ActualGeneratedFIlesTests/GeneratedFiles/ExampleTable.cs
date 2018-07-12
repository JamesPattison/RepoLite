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
	public partial interface IExampleTableRepository : IPkRepository<ExampleTable>
	{
		ExampleTable Get(Int32 priKey);
		IEnumerable<ExampleTable> Get(List<Int32> priKeys);
		IEnumerable<ExampleTable> Get(params Int32[] priKeys);

		bool Update(ExampleTable item);
		bool Delete(Int32 priKey);
		bool Delete(IEnumerable<Int32> priKeys);
	    bool Merge(List<ExampleTable> items);

		IEnumerable<ExampleTable> Search(
			Int32? priKey = null);

	}
	public sealed partial class ExampleTableRepository : BaseRepository<ExampleTable>, IExampleTableRepository
	{
		public ExampleTableRepository(string connectionString) : this(connectionString, exception => { }) { }
		public ExampleTableRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "ExampleTable", 1)
		{
			Columns.Add(new ColumnDefinition("PriKey", typeof(System.Int32), "[INT]", false, true, false));
		}

		public ExampleTable Get(Int32 priKey)
		{
			return Where("PriKey", Comparison.Equals, priKey).Results().FirstOrDefault();
		}

		public IEnumerable<ExampleTable> Get(List<Int32> priKeys)
		{
			return Get(priKeys.ToArray());
		}

		public IEnumerable<ExampleTable> Get(params Int32[] priKeys)
		{
			return Where("PriKey", Comparison.In, priKeys).Results();
		}

		public override bool Create(ExampleTable item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.PriKey);
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
				return false;

			item.PriKey = (Int32)createdKeys[nameof(ExampleTable.PriKey)];
			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params ExampleTable[] items)
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
				dt.Rows.Add(item.PriKey); 
			}

			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<ExampleTable> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(ExampleTable item)
		{
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.PriKey);

			if (success)
			item.ResetDirty();

			return success;
		}
		public bool Delete(ExampleTable exampleTable)
		{
			if (exampleTable == null)
				return false;

			var deleteColumn = new DeleteColumn("PriKey", exampleTable.PriKey);

			return BaseDelete(deleteColumn);
		}
		public bool Delete(IEnumerable<ExampleTable> items)
		{
			if (!items.Any()) return true;
			var deleteValues = new List<object>();
			foreach (var item in items)
			{
				deleteValues.Add(item.PriKey);
			}

			return BaseDelete("PriKey", deleteValues);
		}

		public bool Delete(Int32 priKey)
		{
			return Delete(new ExampleTable { PriKey = priKey });
		}


		public bool Delete(IEnumerable<Int32> priKeys)
		{
			return Delete(priKeys.Select(x => new ExampleTable { PriKey = x }));
		}


		public bool Merge(List<ExampleTable> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.PriKey
				});
			}
			return BaseMerge(mergeTable);
		}

		protected override ExampleTable ToItem(DataRow row)
		{
			 var item = new ExampleTable
			{
				PriKey = GetInt32(row, "PriKey"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<ExampleTable> Search(
			Int32? priKey = null)
		{
			var queries = new List<QueryItem>(); 

			if (priKey.HasValue)
				queries.Add(new QueryItem("PriKey", priKey));

			return BaseSearch(queries);
		}
	}
}
