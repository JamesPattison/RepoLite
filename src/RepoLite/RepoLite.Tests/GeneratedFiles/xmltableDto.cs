using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Base;
using NS.Models.Base;

namespace NS.Models
{
	public sealed partial class xmltable : BaseModel
	{
		public override string EntityName => "xmltable";
		private String _name;
		private XmlDocument _data;

		public String name
		{
			get => _name;
			set => SetValue(ref _name, value);
		}
		public XmlDocument data
		{
			get => _data;
			set => SetValue(ref _data, value);
		}
		public xmltable() { }
		public xmltable(
			String name,
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
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("name", typeof(System.String), "[VARCHAR](12)", SqlDbType.VarChar, false, false, false),
			new ColumnDefinition("data", typeof(System.Xml.XmlDocument), "[XML]", SqlDbType.Xml, false, false, false),
		};
	}
}

