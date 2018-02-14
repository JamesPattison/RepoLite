using NS.Base;
using NS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace NS
{
	public partial interface IXmltableRepository : IBaseRepository<Xmltable>
	{
		IEnumerable<Xmltable> Search(
			String name = null,
			String data = null);

		IEnumerable<Xmltable> FindByName(String name);
		IEnumerable<Xmltable> FindByName(FindComparison comparison, String name);
		IEnumerable<Xmltable> FindByData(String data);
		IEnumerable<Xmltable> FindByData(FindComparison comparison, String data);
	}
	public sealed partial class XmltableRepository : BaseRepository<Xmltable>, IXmltableRepository
	{
		public XmltableRepository(string connectionString) : this(connectionString, exception => { }) { }
		public XmltableRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "xmltable", 2)
		{
			Columns.Add(new ColumnDefinition("name", "[VARCHAR](12)", false, false, false));
			Columns.Add(new ColumnDefinition("data", "[XML]", false, false, false));
		}
		public override bool Create(Xmltable item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Name, item.Data);
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
				return false;
			
			item.ResetDirty();
			
			return true;
		}
			
		public override bool BulkCreate(params Xmltable[] items)
		{
			if (!items.Any())
				return false;
			
			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);
			
			var dt = new DataTable();
			foreach (var mergeColumn in Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName);
			
			foreach (var item in items)
			{
				dt.Rows.Add(item.Name, item.Data); 
			}
			
			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<Xmltable> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Merge(List<Xmltable> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.Name, item.DirtyColumns.Contains("name"),
					item.Data, item.DirtyColumns.Contains("data")
				});
			}
			return BaseMerge(mergeTable);
		}

		protected override Xmltable ToItem(DataRow row)
		{
			 var item = new Xmltable
			{
				Name = GetString(row, "name"),
				Data = GetXmlDocument(row, "data"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<Xmltable> Search(
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

		public IEnumerable<Xmltable> FindByName(String name)
		{
			return FindByName(FindComparison.Equals, name);
		}

		public IEnumerable<Xmltable> FindByName(FindComparison comparison, String name)
		{
			return Where("name", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), name).Results();
		}

		public IEnumerable<Xmltable> FindByData(String data)
		{
			return FindByData(FindComparison.Equals, data);
		}

		public IEnumerable<Xmltable> FindByData(FindComparison comparison, String data)
		{
			return Where("data", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), data, typeof(XmlDocument)).Results();
		}
	}
}
