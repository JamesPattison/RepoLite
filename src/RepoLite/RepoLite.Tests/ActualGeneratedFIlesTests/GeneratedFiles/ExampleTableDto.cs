using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NS.Models
{
	[Table("ExampleTable", Schema="dbo")]
	public partial class ExampleTable : BaseModel
	{
		private Int32 _priKey;

		[Key]
		public virtual Int32 PriKey
		{
			get => _priKey;
			set => SetValue(ref _priKey, value);
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();


			return validationErrors;
		}
	}
}

