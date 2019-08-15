using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Base;
using NS.Models.Base;

namespace NS.Models
{
	public partial class Person : BaseModel
	{
		public override string EntityName => "Person";
		private Int32 _id;
		private String _name;
		private Int32 _age;
		private String _nationality;
		private Boolean _registered;

		public virtual Int32 Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public virtual String Name
		{
			get => _name;
			set => SetValue(ref _name, value);
		}
		public virtual Int32 Age
		{
			get => _age;
			set => SetValue(ref _age, value);
		}
		public virtual String Nationality
		{
			get => _nationality;
			set => SetValue(ref _nationality, value);
		}
		public virtual Boolean Registered
		{
			get => _registered;
			set => SetValue(ref _registered, value);
		}
		public override IBaseModel SetValues(DataRow row, string propertyPrefix)
		{
			_id = row.GetValue<Int32>($"{propertyPrefix}Id") ?? default(Int32); 
			_name = row.GetText($"{propertyPrefix}Name");
			_age = row.GetValue<Int32>($"{propertyPrefix}Age") ?? default(Int32); 
			_nationality = row.GetText($"{propertyPrefix}Nationality");
			_registered = row.GetValue<Boolean>($"{propertyPrefix}Registered") ?? default(Boolean); 
			return this;
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (string.IsNullOrEmpty(Name))
				validationErrors.Add(new ValidationError(nameof(Name), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Name) && Name.Length > 50)
				validationErrors.Add(new ValidationError(nameof(Name), "Max length is 50"));
			if (string.IsNullOrEmpty(Nationality))
				validationErrors.Add(new ValidationError(nameof(Nationality), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Nationality) && Nationality.Length > 50)
				validationErrors.Add(new ValidationError(nameof(Nationality), "Max length is 50"));

			return validationErrors;
		}
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("Id", typeof(System.Int32), "[INT]", SqlDbType.Int, false, true, true),
			new ColumnDefinition("Name", typeof(System.String), "[NVARCHAR](50)", SqlDbType.NVarChar, false, false, false),
			new ColumnDefinition("Age", typeof(System.Int32), "[INT]", SqlDbType.Int, false, false, false),
			new ColumnDefinition("Nationality", typeof(System.String), "[NVARCHAR](50)", SqlDbType.NVarChar, false, false, false),
			new ColumnDefinition("Registered", typeof(System.Boolean), "[BIT]", SqlDbType.Bit, false, false, false),
		};
	}
}

