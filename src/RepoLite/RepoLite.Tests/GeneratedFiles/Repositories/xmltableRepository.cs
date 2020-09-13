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
	public partial interface IxmltableRepository : IBaseRepository<xmltableDto>
	{
		bool DeleteByname(string name);
		bool DeleteBydata(XmlDocument data);
		IEnumerable<xmltableDto> Search(
			string name = null,
			String data = null);
		IEnumerable<xmltableDto> FindByname(string name);
		IEnumerable<xmltableDto> FindByname(FindComparison comparison, string name);
		IEnumerable<xmltableDto> FindBydata(String data);
		IEnumerable<xmltableDto> FindBydata(FindComparison comparison, String data);
	}
	public sealed partial class xmltableRepository : BaseRepository<xmltableDto>, IxmltableRepository
	{
		partial void InitializeExtension();
		public xmltableRepository(string connectionString) : this(connectionString, exception => { }) { }
		public xmltableRepository(string connectionString, bool useCache, int cacheDurationInSeconds) : this(connectionString, exception => { }, useCache, cacheDurationInSeconds) { }
		public xmltableRepository(string connectionString, Action<Exception> logMethod) : this(connectionString, logMethod, false, 0) { }
		public xmltableRepository(string connectionString, Action<Exception> logMethod, bool useCache, int cacheDurationInSeconds) : base(connectionString, logMethod,
			xmltableDto.Schema, xmltableDto.TableName, xmltableDto.Columns, useCache, cacheDurationInSeconds)
		{
			InitializeExtension();
		}
		public override bool Create(xmltableDto item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.name, item.data);
			if (createdKeys.Count != xmltableDto.Columns.Count(x => x.PrimaryKey))
				return false;

			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params xmltableDto[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in xmltableDto.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.name, item.data); 
			}

			if (BulkInsert(dt))
			{
				return true;
			}
			return false;
		}
		public override bool BulkCreate(List<xmltableDto> items)
		{
			return BulkCreate(items.ToArray());
		}
		public bool DeleteByname(string name)
		{
			if (BaseDelete(new DeleteColumn("name", name, SqlDbType.VarChar), out var items))
			{
				return true;
			}
			return false;
		}
		public bool DeleteBydata(XmlDocument data)
		{
			if (BaseDelete(new DeleteColumn("data", data, SqlDbType.Xml), out var items))
			{
				return true;
			}
			return false;
		}

		public override xmltableDto ToItem(DataRow row, bool skipBase)
		{
			var item = new xmltableDto
			{
				name = GetString(row, "name"),
				data = GetXmlDocument(row, "data"),
			};

			item.ResetDirty();
			return item;
		}

		public override TK ToItem<TK>(DataRow row, bool skipBase)
		{
			var item = new TK
			{
				name = GetString(row, "name"),
				data = GetXmlDocument(row, "data"),
			};

			item.ResetDirty();
			return item as TK;
		}

		public IEnumerable<xmltableDto> Search(
			string name = null,
			String data = null)
		{
			var queries = new List<QueryItem>(); 

			if (name != null)
				queries.Add(new QueryItem("name", name));
			if (data != null)
				queries.Add(new QueryItem("data", data, typeof(XmlDocument)));

			return BaseSearch(queries);
		}

		public IEnumerable<xmltableDto> FindByname(string name)
		{
			return FindByname(FindComparison.Equals, name);
		}

		public IEnumerable<xmltableDto> FindByname(FindComparison comparison, string name)
		{
			var items = Where("name", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), name).Results();
			return items;
		}

		public IEnumerable<xmltableDto> FindBydata(String data)
		{
			return FindBydata(FindComparison.Equals, data);
		}

		public IEnumerable<xmltableDto> FindBydata(FindComparison comparison, String data)
		{
			return Where("data", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), data, typeof(XmlDocument)).Results();
		}

	}
}
