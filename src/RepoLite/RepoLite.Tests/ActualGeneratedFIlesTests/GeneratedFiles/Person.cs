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
	public partial interface IPersonRepository : IPkRepository<Person>
	{
		Person Get(Int32 id);
		IEnumerable<Person> Get(List<Int32> ids);
		IEnumerable<Person> Get(params Int32[] ids);

		bool Update(Person item);
		bool Delete(Int32 id);
		bool Delete(IEnumerable<Int32> ids);
	    bool Merge(List<Person> items);

		IEnumerable<Person> Search(
			Int32? id = null,
			String name = null,
			Int32? age = null,
			String nationality = null,
			Boolean? registered = null);

		IEnumerable<Person> FindByName(String name);
		IEnumerable<Person> FindByName(FindComparison comparison, String name);
		IEnumerable<Person> FindByAge(Int32 age);
		IEnumerable<Person> FindByAge(FindComparison comparison, Int32 age);
		IEnumerable<Person> FindByNationality(String nationality);
		IEnumerable<Person> FindByNationality(FindComparison comparison, String nationality);
		IEnumerable<Person> FindByRegistered(Boolean registered);
		IEnumerable<Person> FindByRegistered(FindComparison comparison, Boolean registered);
	}
	public sealed partial class PersonRepository : BaseRepository<Person>, IPersonRepository
	{
		public PersonRepository(string connectionString) : this(connectionString, exception => { }) { }
		public PersonRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "Person", 5)
		{
			Columns.Add(new ColumnDefinition("Id", typeof(System.Int32), "[INT]", false, true, true));
			Columns.Add(new ColumnDefinition("Name", typeof(System.String), "[NVARCHAR](50)", false, false, false));
			Columns.Add(new ColumnDefinition("Age", typeof(System.Int32), "[INT]", false, false, false));
			Columns.Add(new ColumnDefinition("Nationality", typeof(System.String), "[NVARCHAR](50)", false, false, false));
			Columns.Add(new ColumnDefinition("Registered", typeof(System.Boolean), "[BIT]", false, false, false));
		}

		public Person Get(Int32 id)
		{
			return Where("Id", Comparison.Equals, id).Results().FirstOrDefault();
		}

		public IEnumerable<Person> Get(List<Int32> ids)
		{
			return Get(ids.ToArray());
		}

		public IEnumerable<Person> Get(params Int32[] ids)
		{
			return Where("Id", Comparison.In, ids).Results();
		}

		public override bool Create(Person item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.Name, item.Age, item.Nationality, item.Registered);
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
				return false;

			item.Id = (Int32)createdKeys[nameof(Person.Id)];
			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params Person[] items)
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
				dt.Rows.Add(item.Name, item.Age, item.Nationality, item.Registered); 
			}

			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<Person> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(Person item)
		{
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.Id, item.Name, item.Age, item.Nationality, item.Registered);

			if (success)
			item.ResetDirty();

			return success;
		}
		public bool Delete(Person person)
		{
			if (person == null)
				return false;

			var deleteColumn = new DeleteColumn("Id", person.Id);

			return BaseDelete(deleteColumn);
		}
		public bool Delete(IEnumerable<Person> items)
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
			return Delete(new Person { Id = id });
		}


		public bool Delete(IEnumerable<Int32> ids)
		{
			return Delete(ids.Select(x => new Person { Id = x }));
		}


		public bool Merge(List<Person> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.Id,
					item.Name, item.DirtyColumns.Contains("Name"),
					item.Age, item.DirtyColumns.Contains("Age"),
					item.Nationality, item.DirtyColumns.Contains("Nationality"),
					item.Registered, item.DirtyColumns.Contains("Registered")
				});
			}
			return BaseMerge(mergeTable);
		}

		protected override Person ToItem(DataRow row)
		{
			 var item = new Person
			{
				Id = GetInt32(row, "Id"),
				Name = GetString(row, "Name"),
				Age = GetInt32(row, "Age"),
				Nationality = GetString(row, "Nationality"),
				Registered = GetBoolean(row, "Registered"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<Person> Search(
			Int32? id = null,
			String name = null,
			Int32? age = null,
			String nationality = null,
			Boolean? registered = null)
		{
			var queries = new List<QueryItem>(); 

			if (id.HasValue)
				queries.Add(new QueryItem("Id", id));
			if (!string.IsNullOrEmpty(name))
				queries.Add(new QueryItem("Name", name));
			if (age.HasValue)
				queries.Add(new QueryItem("Age", age));
			if (!string.IsNullOrEmpty(nationality))
				queries.Add(new QueryItem("Nationality", nationality));
			if (registered.HasValue)
				queries.Add(new QueryItem("Registered", registered));

			return BaseSearch(queries);
		}

		public IEnumerable<Person> FindByName(String name)
		{
			return FindByName(FindComparison.Equals, name);
		}

		public IEnumerable<Person> FindByName(FindComparison comparison, String name)
		{
			return Where("Name", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), name).Results();
		}

		public IEnumerable<Person> FindByAge(Int32 age)
		{
			return FindByAge(FindComparison.Equals, age);
		}

		public IEnumerable<Person> FindByAge(FindComparison comparison, Int32 age)
		{
			return Where("Age", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), age).Results();
		}

		public IEnumerable<Person> FindByNationality(String nationality)
		{
			return FindByNationality(FindComparison.Equals, nationality);
		}

		public IEnumerable<Person> FindByNationality(FindComparison comparison, String nationality)
		{
			return Where("Nationality", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), nationality).Results();
		}

		public IEnumerable<Person> FindByRegistered(Boolean registered)
		{
			return FindByRegistered(FindComparison.Equals, registered);
		}

		public IEnumerable<Person> FindByRegistered(FindComparison comparison, Boolean registered)
		{
			return Where("Registered", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), registered).Results();
		}
	}
}
