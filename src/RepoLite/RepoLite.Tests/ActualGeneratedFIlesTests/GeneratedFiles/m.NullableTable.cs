using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public partial class NullableTable : BaseModel
	{
		private Int32 _id;
		private Int32? _age;
		private DateTime? _doB;
		private Guid? _lolVal;

		public virtual Int32 Id
		{
			get { return _id; }
			set { SetValue(ref _id, value, "Id"); }
		}
		public virtual Int32? Age
		{
			get { return _age; }
			set { SetValue(ref _age, value, "Age"); }
		}
		public virtual DateTime? DoB
		{
			get { return _doB; }
			set { SetValue(ref _doB, value, "DoB"); }
		}
		public virtual Guid? LolVal
		{
			get { return _lolVal; }
			set { SetValue(ref _lolVal, value, "LolVal"); }
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (DoB == DateTime.MinValue)
			validationErrors.Add(new ValidationError("DoB", "Value cannot be default."));
			if (LolVal == Guid.Empty)
			validationErrors.Add(new ValidationError("LolVal", "Value cannot be default."));

			return validationErrors;
		}
	}
}

