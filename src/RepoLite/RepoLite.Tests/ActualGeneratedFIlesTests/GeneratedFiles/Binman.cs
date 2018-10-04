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
	public partial interface IBinManRepository : IBaseRepository<BinMan>
	{
		bool DeleteById(Int32 id);
		bool DeleteByData(Byte[] data);
		IEnumerable<BinMan> Search(
			Int32? id = null,
			Byte[] data = null);

		IEnumerable<BinMan> FindById(Int32 id);
		IEnumerable<BinMan> FindById(FindComparison comparison, Int32 id);
		IEnumerable<BinMan> FindByData(Byte[] data);
		IEnumerable<BinMan> FindByData(FindComparison comparison, Byte[] data);
	}
	public sealed partial class BinManRepository : BaseRepository<BinMan>, IBinManRepository
	{
		public BinManRepository(string connectionString) : this(connectionString, exception => { }) { }
		public BinManRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "BinMan", 2)
		{
			Columns.Add(new ColumnDefinition("Id", typeof(System.Int32), "[INT]", SqlDbType.Int, false, false, false));
			Columns.Add(new ColumnDefinition("Data", typeof(System.Byte[]), "[BINARY](8)", SqlDbType.Binary, false, false, false));
		}
		public override bool Create(BinMan item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.Data);
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
				return false;

			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params BinMan[] items)
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
				dt.Rows.Add(item.Id, item.Data); 
			}

			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<BinMan> items)
		{
			return BulkCreate(items.ToArray());
		}
		public bool DeleteById(Int32 id)
		{
			return BaseDelete(new DeleteColumn("Id", id, SqlDbType.Int));
		}
		public bool DeleteByData(Byte[] data)
		{
			return BaseDelete(new DeleteColumn("Data", data, SqlDbType.Binary));
		}

		public bool Merge(List<BinMan> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.Id, item.DirtyColumns.Contains("Id"),
					item.Data, item.DirtyColumns.Contains("Data")
				});
			}
			return BaseMerge(mergeTable);
		}

		protected override BinMan ToItem(DataRow row)
		{
			 var item = new BinMan
			{
				Id = GetInt32(row, "Id"),
				Data = GetByteArray(row, "Data"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<BinMan> Search(
			Int32? id = null,
			Byte[] data = null)
		{
			var queries = new List<QueryItem>(); 

			if (id.HasValue)
				queries.Add(new QueryItem("Id", id));
			if (data.Any())
				queries.Add(new QueryItem("Data", data));

			return BaseSearch(queries);
		}

		public IEnumerable<BinMan> FindById(Int32 id)
		{
			return FindById(FindComparison.Equals, id);
		}

		public IEnumerable<BinMan> FindById(FindComparison comparison, Int32 id)
		{
			return Where("Id", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), id).Results();
		}

		public IEnumerable<BinMan> FindByData(Byte[] data)
		{
			return FindByData(FindComparison.Equals, data);
		}

		public IEnumerable<BinMan> FindByData(FindComparison comparison, Byte[] data)
		{
			return Where("Data", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), data).Results();
		}
	}
}
