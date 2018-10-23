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
	public partial interface IxmltableRepository : IBaseRepository<xmltable>
	{
		bool DeleteByname(String name);
		bool DeleteBydata(XmlDocument data);
		IEnumerable<xmltable> Search(
			String name = null,
			String data = null);
		IEnumerable<xmltable> FindByname(String name);
		IEnumerable<xmltable> FindByname(FindComparison comparison, String name);
		IEnumerable<xmltable> FindBydata(String data);
		IEnumerable<xmltable> FindBydata(FindComparison comparison, String data);
	}
	public sealed partial class xmltableRepository : BaseRepository<xmltable>, IxmltableRepository
	{
		public xmltableRepository(string connectionString) : this(connectionString, exception => { }) { }
		public xmltableRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "xmltable", 2)
		{
			Columns.Add(new ColumnDefinition("name", typeof(System.String), "[VARCHAR](12)", SqlDbType.VarChar, false, false, false));
			Columns.Add(new ColumnDefinition("data", typeof(System.Xml.XmlDocument), "[XML]", SqlDbType.Xml, false, false, false));
		}
		public override bool Create(xmltable item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.name, item.data);
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
				return false;

			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params xmltable[] items)
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
				dt.Rows.Add(item.name, item.data); 
			}

			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<xmltable> items)
		{
			return BulkCreate(items.ToArray());
		}
		public bool DeleteByname(String name)
		{
			return BaseDelete(new DeleteColumn("name", name, SqlDbType.VarChar));
		}
		public bool DeleteBydata(XmlDocument data)
		{
			return BaseDelete(new DeleteColumn("data", data, SqlDbType.Xml));
		}

		protected override xmltable ToItem(DataRow row)
		{
			 var item = new xmltable
			{
				name = GetString(row, "name"),
				data = GetXmlDocument(row, "data"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<xmltable> Search(
			String name = null,
			String data = null)
		{
			var queries = new List<QueryItem>(); 

			if (!string.IsNullOrEmpty(name))
				queries.Add(new QueryItem("name", name));
			if (data != null)
				queries.Add(new QueryItem("data", data, typeof(XmlDocument)));

			return BaseSearch(queries);
		}

		public IEnumerable<xmltable> FindByname(String name)
		{
			return FindByname(FindComparison.Equals, name);
		}

		public IEnumerable<xmltable> FindByname(FindComparison comparison, String name)
		{
			return Where("name", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), name).Results();
		}

		public IEnumerable<xmltable> FindBydata(String data)
		{
			return FindBydata(FindComparison.Equals, data);
		}

		public IEnumerable<xmltable> FindBydata(FindComparison comparison, String data)
		{
			return Where("data", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), data, typeof(XmlDocument)).Results();
		}
	}
}
