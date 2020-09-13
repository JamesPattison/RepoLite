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
	public partial interface IBinManRepository : IBaseRepository<BinManDto>
	{
		bool DeleteById(int id);
		bool DeleteByData(byte[] data);
		IEnumerable<BinManDto> Search(
			int? id = null,
			byte[] data = null);
		IEnumerable<BinManDto> FindById(int id);
		IEnumerable<BinManDto> FindById(FindComparison comparison, int id);
		IEnumerable<BinManDto> FindByData(byte[] data);
		IEnumerable<BinManDto> FindByData(FindComparison comparison, byte[] data);
	}
	public sealed partial class BinManRepository : BaseRepository<BinManDto>, IBinManRepository
	{
		partial void InitializeExtension();
		public BinManRepository(string connectionString) : this(connectionString, exception => { }) { }
		public BinManRepository(string connectionString, bool useCache, int cacheDurationInSeconds) : this(connectionString, exception => { }, useCache, cacheDurationInSeconds) { }
		public BinManRepository(string connectionString, Action<Exception> logMethod) : this(connectionString, logMethod, false, 0) { }
		public BinManRepository(string connectionString, Action<Exception> logMethod, bool useCache, int cacheDurationInSeconds) : base(connectionString, logMethod,
			BinManDto.Schema, BinManDto.TableName, BinManDto.Columns, useCache, cacheDurationInSeconds)
		{
			InitializeExtension();
		}
		public override bool Create(BinManDto item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.Data);
			if (createdKeys.Count != BinManDto.Columns.Count(x => x.PrimaryKey))
				return false;

			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params BinManDto[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in BinManDto.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.Id, item.Data); 
			}

			if (BulkInsert(dt))
			{
				return true;
			}
			return false;
		}
		public override bool BulkCreate(List<BinManDto> items)
		{
			return BulkCreate(items.ToArray());
		}
		public bool DeleteById(int id)
		{
			if (BaseDelete(new DeleteColumn("Id", id, SqlDbType.Int), out var items))
			{
				return true;
			}
			return false;
		}
		public bool DeleteByData(byte[] data)
		{
			if (BaseDelete(new DeleteColumn("Data", data, SqlDbType.Binary), out var items))
			{
				return true;
			}
			return false;
		}

		public override BinManDto ToItem(DataRow row, bool skipBase)
		{
			var item = new BinManDto
			{
				Id = GetInt32(row, "Id"),
				Data = GetByteArray(row, "Data"),
			};

			item.ResetDirty();
			return item;
		}

		public override TK ToItem<TK>(DataRow row, bool skipBase)
		{
			var item = new TK
			{
				Id = GetInt32(row, "Id"),
				Data = GetByteArray(row, "Data"),
			};

			item.ResetDirty();
			return item as TK;
		}

		public IEnumerable<BinManDto> Search(
			int? id = null,
			byte[] data = null)
		{
			var queries = new List<QueryItem>(); 

			if (id.HasValue)
				queries.Add(new QueryItem("Id", id));
			if (data != null)
				queries.Add(new QueryItem("Data", data));

			return BaseSearch(queries);
		}

		public IEnumerable<BinManDto> FindById(int id)
		{
			return FindById(FindComparison.Equals, id);
		}

		public IEnumerable<BinManDto> FindById(FindComparison comparison, int id)
		{
			var items = Where("Id", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), id).Results();
			return items;
		}

		public IEnumerable<BinManDto> FindByData(byte[] data)
		{
			return FindByData(FindComparison.Equals, data);
		}

		public IEnumerable<BinManDto> FindByData(FindComparison comparison, byte[] data)
		{
			var items = Where("Data", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), data).Results();
			return items;
		}

	}
}
