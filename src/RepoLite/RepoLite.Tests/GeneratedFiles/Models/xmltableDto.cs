using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public sealed partial class xmltableDto : BaseModel
	{
		public override string EntityName => "xmltable";
		public static string CacheKey(object identifier) => $"xmltable_{identifier}";
		protected string _name;
		protected XmlDocument _data;

		public string name
		{
			get => _name;
			set => SetValue(ref _name, value);
		}
		public XmlDocument data
		{
			get => _data;
			set => SetValue(ref _data, value);
		}
		public xmltableDto() { }
		public xmltableDto(
			string name,
			XmlDocument data)
		{
			_name = name;
			_data = data;
		}
		public override IBaseModel SetValues(DataRow row, string propertyPrefix)
		{
			_name = row.GetText($"{propertyPrefix}name");
			_data = new XmlDocument{InnerXml = row.GetText($"{propertyPrefix}data")};
			return this;
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (name == null)
				validationErrors.Add(new ValidationError(nameof(name), "Value cannot be null"));
			if (!string.IsNullOrEmpty(name) && name.Length > 12)
				validationErrors.Add(new ValidationError(nameof(name), "Max length is 12"));

			return validationErrors;
		}
		public static string Schema = "dbo";
		public static string TableName = "xmltable";
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("name", typeof(string), "[VARCHAR](12)", SqlDbType.VarChar, false, false, false),
			new ColumnDefinition("data", typeof(XmlDocument), "[XML]", SqlDbType.Xml, false, false, false),
		};
	}
}

