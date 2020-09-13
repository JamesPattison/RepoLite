using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public sealed partial class AddressDto : BaseModel
	{
		public override string EntityName => "Address";
		public static string CacheKey(object identifier) => $"Address_{identifier}";
		protected int _id;
		protected string _anotherid;
		protected int? _personid;
		protected string _line1;
		protected string _line2;
		protected string _line3;
		protected string _line4;
		protected string _postcode;
		protected string _phonenumber;
		protected string _country_code;

		public int Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public string AnotherId
		{
			get => _anotherid;
			set => SetValue(ref _anotherid, value);
		}
		public int? PersonId
		{
			get => _personid;
			set => SetValue(ref _personid, value);
		}
		/// <summary>
		/// Nothing is done with this, it's merely there to hold data IF you wish to populate it
		/// </summary>
		public PersonDto Person { get; set; }
		public string Line1
		{
			get => _line1;
			set => SetValue(ref _line1, value);
		}
		public string Line2
		{
			get => _line2;
			set => SetValue(ref _line2, value);
		}
		public string Line3
		{
			get => _line3;
			set => SetValue(ref _line3, value);
		}
		public string Line4
		{
			get => _line4;
			set => SetValue(ref _line4, value);
		}
		public string PostCode
		{
			get => _postcode;
			set => SetValue(ref _postcode, value);
		}
		public string PhoneNumber
		{
			get => _phonenumber;
			set => SetValue(ref _phonenumber, value);
		}
		public string COUNTRY_CODE
		{
			get => _country_code;
			set => SetValue(ref _country_code, value);
		}
		public AddressDto() { }
		public AddressDto(
			int id,
			string anotherid,
			int personid,
			string line1,
			string line2,
			string line3,
			string line4,
			string postcode,
			string phonenumber,
			string country_code)
		{
			_id = id;
			_anotherid = anotherid;
			_personid = personid;
			_line1 = line1;
			_line2 = line2;
			_line3 = line3;
			_line4 = line4;
			_postcode = postcode;
			_phonenumber = phonenumber;
			_country_code = country_code;
		}
		public override IBaseModel SetValues(DataRow row, string propertyPrefix)
		{
			_id = row.GetValue<int>($"{propertyPrefix}Id") ?? default(int); 
			_anotherid = row.GetText($"{propertyPrefix}AnotherId");
			_personid = row.GetValue<int>($"{propertyPrefix}PersonId"); 
			_line1 = row.GetText($"{propertyPrefix}Line1");
			_line2 = row.GetText($"{propertyPrefix}Line2");
			_line3 = row.GetText($"{propertyPrefix}Line3");
			_line4 = row.GetText($"{propertyPrefix}Line4");
			_postcode = row.GetText($"{propertyPrefix}PostCode");
			_phonenumber = row.GetText($"{propertyPrefix}PhoneNumber");
			_country_code = row.GetText($"{propertyPrefix}COUNTRY_CODE");
			return this;
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (AnotherId == null)
				validationErrors.Add(new ValidationError(nameof(AnotherId), "Value cannot be null"));
			if (!string.IsNullOrEmpty(AnotherId) && AnotherId.Length > 10)
				validationErrors.Add(new ValidationError(nameof(AnotherId), "Max length is 10"));
			if (Line1 == null)
				validationErrors.Add(new ValidationError(nameof(Line1), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Line1) && Line1.Length > 100)
				validationErrors.Add(new ValidationError(nameof(Line1), "Max length is 100"));
			if (!string.IsNullOrEmpty(Line2) && Line2.Length > 100)
				validationErrors.Add(new ValidationError(nameof(Line2), "Max length is 100"));
			if (!string.IsNullOrEmpty(Line3) && Line3.Length > 100)
				validationErrors.Add(new ValidationError(nameof(Line3), "Max length is 100"));
			if (!string.IsNullOrEmpty(Line4) && Line4.Length > 100)
				validationErrors.Add(new ValidationError(nameof(Line4), "Max length is 100"));
			if (PostCode == null)
				validationErrors.Add(new ValidationError(nameof(PostCode), "Value cannot be null"));
			if (!string.IsNullOrEmpty(PostCode) && PostCode.Length > 15)
				validationErrors.Add(new ValidationError(nameof(PostCode), "Max length is 15"));
			if (!string.IsNullOrEmpty(PhoneNumber) && PhoneNumber.Length > 20)
				validationErrors.Add(new ValidationError(nameof(PhoneNumber), "Max length is 20"));
			if (!string.IsNullOrEmpty(COUNTRY_CODE) && COUNTRY_CODE.Length > 2)
				validationErrors.Add(new ValidationError(nameof(COUNTRY_CODE), "Max length is 2"));

			return validationErrors;
		}
		public static string Schema = "dbo";
		public static string TableName = "Address";
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("Id", typeof(int), "[INT]", SqlDbType.Int, false, true, true),
			new ColumnDefinition("AnotherId", typeof(string), "[NVARCHAR](10)", SqlDbType.NVarChar, false, true, false),
			new ColumnDefinition("PersonId", typeof(int), "[INT]", SqlDbType.Int, true, false, false),
			new ColumnDefinition("Line1", typeof(string), "[NVARCHAR](100)", SqlDbType.NVarChar, false, false, false),
			new ColumnDefinition("Line2", typeof(string), "[NVARCHAR](100)", SqlDbType.NVarChar, true, false, false),
			new ColumnDefinition("Line3", typeof(string), "[NVARCHAR](100)", SqlDbType.NVarChar, true, false, false),
			new ColumnDefinition("Line4", typeof(string), "[NVARCHAR](100)", SqlDbType.NVarChar, true, false, false),
			new ColumnDefinition("PostCode", typeof(string), "[NVARCHAR](15)", SqlDbType.NVarChar, false, false, false),
			new ColumnDefinition("PhoneNumber", typeof(string), "[NVARCHAR](20)", SqlDbType.NVarChar, true, false, false),
			new ColumnDefinition("COUNTRY_CODE", typeof(string), "[NVARCHAR](2)", SqlDbType.NVarChar, true, false, false),
		};
	}
}

