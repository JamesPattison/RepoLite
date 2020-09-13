using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public sealed partial class PersonDto : BaseModel
	{
		public override string EntityName => "Person";
		public static string CacheKey(object identifier) => $"Person_{identifier}";
		protected int _id;
		protected string _name;
		protected int _age;
		protected string _nationality;
		protected bool _registered;

		public int Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public string Name
		{
			get => _name;
			set => SetValue(ref _name, value);
		}
		public int Age
		{
			get => _age;
			set => SetValue(ref _age, value);
		}
		public string Nationality
		{
			get => _nationality;
			set => SetValue(ref _nationality, value);
		}
		public bool Registered
		{
			get => _registered;
			set => SetValue(ref _registered, value);
		}
		public PersonDto() { }
		public PersonDto(
			int id,
			string name,
			int age,
			string nationality,
			bool registered)
		{
			_id = id;
			_name = name;
			_age = age;
			_nationality = nationality;
			_registered = registered;
		}
		public override IBaseModel SetValues(DataRow row, string propertyPrefix)
		{
			_id = row.GetValue<int>($"{propertyPrefix}Id") ?? default(int); 
			_name = row.GetText($"{propertyPrefix}Name");
			_age = row.GetValue<int>($"{propertyPrefix}Age") ?? default(int); 
			_nationality = row.GetText($"{propertyPrefix}Nationality");
			_registered = row.GetValue<bool>($"{propertyPrefix}Registered") ?? default(bool); 
			return this;
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (Name == null)
				validationErrors.Add(new ValidationError(nameof(Name), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Name) && Name.Length > 50)
				validationErrors.Add(new ValidationError(nameof(Name), "Max length is 50"));
			if (Nationality == null)
				validationErrors.Add(new ValidationError(nameof(Nationality), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Nationality) && Nationality.Length > 50)
				validationErrors.Add(new ValidationError(nameof(Nationality), "Max length is 50"));

			return validationErrors;
		}
		public static string Schema = "dbo";
		public static string TableName = "Person";
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("Id", typeof(int), "[INT]", SqlDbType.Int, false, true, true),
			new ColumnDefinition("Name", typeof(string), "[NVARCHAR](50)", SqlDbType.NVarChar, false, false, false),
			new ColumnDefinition("Age", typeof(int), "[INT]", SqlDbType.Int, false, false, false),
			new ColumnDefinition("Nationality", typeof(string), "[NVARCHAR](50)", SqlDbType.NVarChar, false, false, false),
			new ColumnDefinition("Registered", typeof(bool), "[BIT]", SqlDbType.Bit, false, false, false),
		};
	}
}

