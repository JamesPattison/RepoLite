using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NS.Models
{
	[Table("BIN_MASTER", Schema="dbo")]
	public partial class BIN_MASTER : BaseModel
	{
		private Int32 _bIN_ID;
		private Int32 _rACK_ID;
		private Int32? _sTATION_ID;
		private Int32? _bIN_BIN_ID;
		private Int32? _wH_ID;
		private String _bIN_NAME;
		private String _bIN_BAR_CODE;
		private Int32? _bIN_PACKING_ORDER;
		private Int32? _mAX_CONTAINERS;
		private Decimal _mAX_WEIGHT;
		private Decimal _mAX_VOLUME;
		private Decimal _mAX_HEIGHT;
		private Decimal _mAX_DEPTH;
		private Decimal _mAX_WIDTH;
		private Boolean _aLLOW_MULTI_STOCK;
		private Boolean _tRANSITION_BIN;
		private Boolean _dEFAULT_RECEIPT_BIN;
		private Boolean _cONTAINER_STORAGE_BIN;
		private Int32? _pICK_SEQUENCE;
		private Boolean _sTOCK_TAKE;
		private Boolean _aUTO_ALLOCATE;
		private Boolean _bIN_FULL;
		private Boolean _pALLETISATION_BIN;
		private Int32? _zONE_ID;
		private Int32? _pOSTMAN_WALK_HOUSE_NO;
		private Int32? _z_PICK_NO;
		private Boolean _vIRTUAL_CONTAINERS_ONLY;
		private Boolean _tRANSITIONAL_BIN;
		private Boolean _tYPE_ET;
		private Decimal? _iNNER_HEIGHT;
		private Boolean _iS_TOP_SHELF;
		private Boolean? _tRANSIT_IN;
		private Boolean? _tRANSIT_OUT;
		private Boolean _wIP_RECEIVING;
		private Boolean _oN_HOLD;
		private String _rACK_LEVEL;
		private Int32 _rACK_SEQUENCE;
		private Int32? _wIP_RETURN_BIN;
		private Int32 _cHECK_DIGIT;
		private Int32? _softAllocWHArea;
		private Boolean _inPickingProcess;
		private Int32 _pickingByRacId;
		private Int32 _inPickingInstanceID;
		private Boolean _usePickFaceBinsForWIP;
		private Boolean _singleInwardMovementsForWIP;
		private Boolean _suspenseBin;

		[Key]
		public virtual Int32 BIN_ID
		{
			get => _bIN_ID;
			set => SetValue(ref _bIN_ID, value);
		}
		public virtual Int32 RACK_ID
		{
			get => _rACK_ID;
			set => SetValue(ref _rACK_ID, value);
		}
		public virtual Int32? STATION_ID
		{
			get => _sTATION_ID;
			set => SetValue(ref _sTATION_ID, value);
		}
		public virtual Int32? BIN_BIN_ID
		{
			get => _bIN_BIN_ID;
			set => SetValue(ref _bIN_BIN_ID, value);
		}
		public virtual Int32? WH_ID
		{
			get => _wH_ID;
			set => SetValue(ref _wH_ID, value);
		}
		public virtual String BIN_NAME
		{
			get => _bIN_NAME;
			set => SetValue(ref _bIN_NAME, value);
		}
		public virtual String BIN_BAR_CODE
		{
			get => _bIN_BAR_CODE;
			set => SetValue(ref _bIN_BAR_CODE, value);
		}
		public virtual Int32? BIN_PACKING_ORDER
		{
			get => _bIN_PACKING_ORDER;
			set => SetValue(ref _bIN_PACKING_ORDER, value);
		}
		public virtual Int32? MAX_CONTAINERS
		{
			get => _mAX_CONTAINERS;
			set => SetValue(ref _mAX_CONTAINERS, value);
		}
		public virtual Decimal MAX_WEIGHT
		{
			get => _mAX_WEIGHT;
			set => SetValue(ref _mAX_WEIGHT, value);
		}
		public virtual Decimal MAX_VOLUME
		{
			get => _mAX_VOLUME;
			set => SetValue(ref _mAX_VOLUME, value);
		}
		public virtual Decimal MAX_HEIGHT
		{
			get => _mAX_HEIGHT;
			set => SetValue(ref _mAX_HEIGHT, value);
		}
		public virtual Decimal MAX_DEPTH
		{
			get => _mAX_DEPTH;
			set => SetValue(ref _mAX_DEPTH, value);
		}
		public virtual Decimal MAX_WIDTH
		{
			get => _mAX_WIDTH;
			set => SetValue(ref _mAX_WIDTH, value);
		}
		public virtual Boolean ALLOW_MULTI_STOCK
		{
			get => _aLLOW_MULTI_STOCK;
			set => SetValue(ref _aLLOW_MULTI_STOCK, value);
		}
		public virtual Boolean TRANSITION_BIN
		{
			get => _tRANSITION_BIN;
			set => SetValue(ref _tRANSITION_BIN, value);
		}
		public virtual Boolean DEFAULT_RECEIPT_BIN
		{
			get => _dEFAULT_RECEIPT_BIN;
			set => SetValue(ref _dEFAULT_RECEIPT_BIN, value);
		}
		public virtual Boolean CONTAINER_STORAGE_BIN
		{
			get => _cONTAINER_STORAGE_BIN;
			set => SetValue(ref _cONTAINER_STORAGE_BIN, value);
		}
		public virtual Int32? PICK_SEQUENCE
		{
			get => _pICK_SEQUENCE;
			set => SetValue(ref _pICK_SEQUENCE, value);
		}
		public virtual Boolean STOCK_TAKE
		{
			get => _sTOCK_TAKE;
			set => SetValue(ref _sTOCK_TAKE, value);
		}
		public virtual Boolean AUTO_ALLOCATE
		{
			get => _aUTO_ALLOCATE;
			set => SetValue(ref _aUTO_ALLOCATE, value);
		}
		public virtual Boolean BIN_FULL
		{
			get => _bIN_FULL;
			set => SetValue(ref _bIN_FULL, value);
		}
		public virtual Boolean PALLETISATION_BIN
		{
			get => _pALLETISATION_BIN;
			set => SetValue(ref _pALLETISATION_BIN, value);
		}
		public virtual Int32? ZONE_ID
		{
			get => _zONE_ID;
			set => SetValue(ref _zONE_ID, value);
		}
		public virtual Int32? POSTMAN_WALK_HOUSE_NO
		{
			get => _pOSTMAN_WALK_HOUSE_NO;
			set => SetValue(ref _pOSTMAN_WALK_HOUSE_NO, value);
		}
		public virtual Int32? Z_PICK_NO
		{
			get => _z_PICK_NO;
			set => SetValue(ref _z_PICK_NO, value);
		}
		public virtual Boolean VIRTUAL_CONTAINERS_ONLY
		{
			get => _vIRTUAL_CONTAINERS_ONLY;
			set => SetValue(ref _vIRTUAL_CONTAINERS_ONLY, value);
		}
		public virtual Boolean TRANSITIONAL_BIN
		{
			get => _tRANSITIONAL_BIN;
			set => SetValue(ref _tRANSITIONAL_BIN, value);
		}
		public virtual Boolean TYPE_ET
		{
			get => _tYPE_ET;
			set => SetValue(ref _tYPE_ET, value);
		}
		public virtual Decimal? INNER_HEIGHT
		{
			get => _iNNER_HEIGHT;
			set => SetValue(ref _iNNER_HEIGHT, value);
		}
		public virtual Boolean IS_TOP_SHELF
		{
			get => _iS_TOP_SHELF;
			set => SetValue(ref _iS_TOP_SHELF, value);
		}
		public virtual Boolean? TRANSIT_IN
		{
			get => _tRANSIT_IN;
			set => SetValue(ref _tRANSIT_IN, value);
		}
		public virtual Boolean? TRANSIT_OUT
		{
			get => _tRANSIT_OUT;
			set => SetValue(ref _tRANSIT_OUT, value);
		}
		public virtual Boolean WIP_RECEIVING
		{
			get => _wIP_RECEIVING;
			set => SetValue(ref _wIP_RECEIVING, value);
		}
		public virtual Boolean ON_HOLD
		{
			get => _oN_HOLD;
			set => SetValue(ref _oN_HOLD, value);
		}
		public virtual String RACK_LEVEL
		{
			get => _rACK_LEVEL;
			set => SetValue(ref _rACK_LEVEL, value);
		}
		public virtual Int32 RACK_SEQUENCE
		{
			get => _rACK_SEQUENCE;
			set => SetValue(ref _rACK_SEQUENCE, value);
		}
		public virtual Int32? WIP_RETURN_BIN
		{
			get => _wIP_RETURN_BIN;
			set => SetValue(ref _wIP_RETURN_BIN, value);
		}
		public virtual Int32 CHECK_DIGIT
		{
			get => _cHECK_DIGIT;
			set => SetValue(ref _cHECK_DIGIT, value);
		}
		public virtual Int32? SoftAllocWHArea
		{
			get => _softAllocWHArea;
			set => SetValue(ref _softAllocWHArea, value);
		}
		public virtual Boolean InPickingProcess
		{
			get => _inPickingProcess;
			set => SetValue(ref _inPickingProcess, value);
		}
		public virtual Int32 PickingByRacId
		{
			get => _pickingByRacId;
			set => SetValue(ref _pickingByRacId, value);
		}
		public virtual Int32 InPickingInstanceID
		{
			get => _inPickingInstanceID;
			set => SetValue(ref _inPickingInstanceID, value);
		}
		public virtual Boolean UsePickFaceBinsForWIP
		{
			get => _usePickFaceBinsForWIP;
			set => SetValue(ref _usePickFaceBinsForWIP, value);
		}
		public virtual Boolean SingleInwardMovementsForWIP
		{
			get => _singleInwardMovementsForWIP;
			set => SetValue(ref _singleInwardMovementsForWIP, value);
		}
		public virtual Boolean SuspenseBin
		{
			get => _suspenseBin;
			set => SetValue(ref _suspenseBin, value);
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (string.IsNullOrEmpty(BIN_NAME))
				validationErrors.Add(new ValidationError(nameof(BIN_NAME), "Value cannot be null"));
			if (!string.IsNullOrEmpty(BIN_NAME) && BIN_NAME.Length > 30)
				validationErrors.Add(new ValidationError(nameof(BIN_NAME), "Max length is 30"));
			if (!string.IsNullOrEmpty(BIN_BAR_CODE) && BIN_BAR_CODE.Length > 20)
				validationErrors.Add(new ValidationError(nameof(BIN_BAR_CODE), "Max length is 20"));
			if (Math.Floor(MAX_WEIGHT) > 1864712049423024127)
				validationErrors.Add(new ValidationError(nameof(MAX_WEIGHT), "Value cannot exceed 1864712049423024127"));
			if (GetDecimalPlaces(MAX_WEIGHT) > 6)
				validationErrors.Add(new ValidationError(nameof(MAX_WEIGHT), "Value cannot have more than 6 decimal places"));
			if (Math.Floor(MAX_VOLUME) > 1864712049423024127)
				validationErrors.Add(new ValidationError(nameof(MAX_VOLUME), "Value cannot exceed 1864712049423024127"));
			if (GetDecimalPlaces(MAX_VOLUME) > 6)
				validationErrors.Add(new ValidationError(nameof(MAX_VOLUME), "Value cannot have more than 6 decimal places"));
			if (Math.Floor(MAX_HEIGHT) > 1864712049423024127)
				validationErrors.Add(new ValidationError(nameof(MAX_HEIGHT), "Value cannot exceed 1864712049423024127"));
			if (GetDecimalPlaces(MAX_HEIGHT) > 6)
				validationErrors.Add(new ValidationError(nameof(MAX_HEIGHT), "Value cannot have more than 6 decimal places"));
			if (Math.Floor(MAX_DEPTH) > 1864712049423024127)
				validationErrors.Add(new ValidationError(nameof(MAX_DEPTH), "Value cannot exceed 1864712049423024127"));
			if (GetDecimalPlaces(MAX_DEPTH) > 6)
				validationErrors.Add(new ValidationError(nameof(MAX_DEPTH), "Value cannot have more than 6 decimal places"));
			if (Math.Floor(MAX_WIDTH) > 1864712049423024127)
				validationErrors.Add(new ValidationError(nameof(MAX_WIDTH), "Value cannot exceed 1864712049423024127"));
			if (GetDecimalPlaces(MAX_WIDTH) > 6)
				validationErrors.Add(new ValidationError(nameof(MAX_WIDTH), "Value cannot have more than 6 decimal places"));
			if (INNER_HEIGHT.HasValue && Math.Floor(INNER_HEIGHT.Value) > 999999999999999)
				validationErrors.Add(new ValidationError(nameof(INNER_HEIGHT), "Value cannot exceed 999999999999999"));
			if (INNER_HEIGHT.HasValue && GetDecimalPlaces(INNER_HEIGHT.Value) > 3)
				validationErrors.Add(new ValidationError(nameof(INNER_HEIGHT), "Value cannot have more than 3 decimal places"));
			if (string.IsNullOrEmpty(RACK_LEVEL))
				validationErrors.Add(new ValidationError(nameof(RACK_LEVEL), "Value cannot be null"));
			if (!string.IsNullOrEmpty(RACK_LEVEL) && RACK_LEVEL.Length > 1)
				validationErrors.Add(new ValidationError(nameof(RACK_LEVEL), "Max length is 1"));

			return validationErrors;
		}
	}
}

