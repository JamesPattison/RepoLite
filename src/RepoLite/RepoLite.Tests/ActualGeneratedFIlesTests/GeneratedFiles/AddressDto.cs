using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public partial class Address : BaseModel
	{
		public override string EntityName => "Address";
		private Int32 _id;
		private String _anotherid;
		private Int32 _personid;
		private String _line1;
		private String _line2;
		private String _line3;
		private String _line4;
		private String _postcode;
		private String _phonenumber;
		private String _country_code;

		public virtual Int32 Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public virtual String AnotherId
		{
			get => _anotherid;
			set => SetValue(ref _anotherid, value);
		}
		public virtual Int32 PersonId
		{
			get => _personid;
			set => SetValue(ref _personid, value);
		}
		public virtual String Line1
		{
			get => _line1;
			set => SetValue(ref _line1, value);
		}
		public virtual String Line2
		{
			get => _line2;
			set => SetValue(ref _line2, value);
		}
		public virtual String Line3
		{
			get => _line3;
			set => SetValue(ref _line3, value);
		}
		public virtual String Line4
		{
			get => _line4;
			set => SetValue(ref _line4, value);
		}
		public virtual String PostCode
		{
			get => _postcode;
			set => SetValue(ref _postcode, value);
		}
		public virtual String PhoneNumber
		{
			get => _phonenumber;
			set => SetValue(ref _phonenumber, value);
		}
		public virtual String COUNTRY_CODE
		{
			get => _country_code;
			set => SetValue(ref _country_code, value);
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (string.IsNullOrEmpty(AnotherId))
				validationErrors.Add(new ValidationError(nameof(AnotherId), "Value cannot be null"));
			if (!string.IsNullOrEmpty(AnotherId) && AnotherId.Length > 10)
				validationErrors.Add(new ValidationError(nameof(AnotherId), "Max length is 10"));
			if (string.IsNullOrEmpty(Line1))
				validationErrors.Add(new ValidationError(nameof(Line1), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Line1) && Line1.Length > 100)
				validationErrors.Add(new ValidationError(nameof(Line1), "Max length is 100"));
			if (!string.IsNullOrEmpty(Line2) && Line2.Length > 100)
				validationErrors.Add(new ValidationError(nameof(Line2), "Max length is 100"));
			if (!string.IsNullOrEmpty(Line3) && Line3.Length > 100)
				validationErrors.Add(new ValidationError(nameof(Line3), "Max length is 100"));
			if (!string.IsNullOrEmpty(Line4) && Line4.Length > 100)
				validationErrors.Add(new ValidationError(nameof(Line4), "Max length is 100"));
			if (string.IsNullOrEmpty(PostCode))
				validationErrors.Add(new ValidationError(nameof(PostCode), "Value cannot be null"));
			if (!string.IsNullOrEmpty(PostCode) && PostCode.Length > 15)
				validationErrors.Add(new ValidationError(nameof(PostCode), "Max length is 15"));
			if (!string.IsNullOrEmpty(PhoneNumber) && PhoneNumber.Length > 20)
				validationErrors.Add(new ValidationError(nameof(PhoneNumber), "Max length is 20"));
			if (!string.IsNullOrEmpty(COUNTRY_CODE) && COUNTRY_CODE.Length > 2)
				validationErrors.Add(new ValidationError(nameof(COUNTRY_CODE), "Max length is 2"));

			return validationErrors;
		}
	}
}

