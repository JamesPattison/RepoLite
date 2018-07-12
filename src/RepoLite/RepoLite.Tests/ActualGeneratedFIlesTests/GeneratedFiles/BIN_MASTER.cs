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
	public partial interface IBIN_MASTERRepository : IPkRepository<BIN_MASTER>
	{
		BIN_MASTER Get(Int32 bIN_ID);
		IEnumerable<BIN_MASTER> Get(List<Int32> bIN_IDs);
		IEnumerable<BIN_MASTER> Get(params Int32[] bIN_IDs);

		bool Update(BIN_MASTER item);
		bool Delete(Int32 bIN_ID);
		bool Delete(IEnumerable<Int32> bIN_IDs);
	    bool Merge(List<BIN_MASTER> items);

		IEnumerable<BIN_MASTER> Search(
			Int32? bIN_ID = null,
			Int32? rACK_ID = null,
			Int32? sTATION_ID = null,
			Int32? bIN_BIN_ID = null,
			Int32? wH_ID = null,
			String bIN_NAME = null,
			String bIN_BAR_CODE = null,
			Int32? bIN_PACKING_ORDER = null,
			Int32? mAX_CONTAINERS = null,
			Decimal? mAX_WEIGHT = null,
			Decimal? mAX_VOLUME = null,
			Decimal? mAX_HEIGHT = null,
			Decimal? mAX_DEPTH = null,
			Decimal? mAX_WIDTH = null,
			Boolean? aLLOW_MULTI_STOCK = null,
			Boolean? tRANSITION_BIN = null,
			Boolean? dEFAULT_RECEIPT_BIN = null,
			Boolean? cONTAINER_STORAGE_BIN = null,
			Int32? pICK_SEQUENCE = null,
			Boolean? sTOCK_TAKE = null,
			Boolean? aUTO_ALLOCATE = null,
			Boolean? bIN_FULL = null,
			Boolean? pALLETISATION_BIN = null,
			Int32? zONE_ID = null,
			Int32? pOSTMAN_WALK_HOUSE_NO = null,
			Int32? z_PICK_NO = null,
			Boolean? vIRTUAL_CONTAINERS_ONLY = null,
			Boolean? tRANSITIONAL_BIN = null,
			Boolean? tYPE_ET = null,
			Decimal? iNNER_HEIGHT = null,
			Boolean? iS_TOP_SHELF = null,
			Boolean? tRANSIT_IN = null,
			Boolean? tRANSIT_OUT = null,
			Boolean? wIP_RECEIVING = null,
			Boolean? oN_HOLD = null,
			String rACK_LEVEL = null,
			Int32? rACK_SEQUENCE = null,
			Int32? wIP_RETURN_BIN = null,
			Int32? cHECK_DIGIT = null,
			Int32? softAllocWHArea = null,
			Boolean? inPickingProcess = null,
			Int32? pickingByRacId = null,
			Int32? inPickingInstanceID = null,
			Boolean? usePickFaceBinsForWIP = null,
			Boolean? singleInwardMovementsForWIP = null,
			Boolean? suspenseBin = null);

		IEnumerable<BIN_MASTER> FindByRACK_ID(Int32 rACK_ID);
		IEnumerable<BIN_MASTER> FindByRACK_ID(FindComparison comparison, Int32 rACK_ID);
		IEnumerable<BIN_MASTER> FindBySTATION_ID(Int32 sTATION_ID);
		IEnumerable<BIN_MASTER> FindBySTATION_ID(FindComparison comparison, Int32 sTATION_ID);
		IEnumerable<BIN_MASTER> FindByBIN_BIN_ID(Int32 bIN_BIN_ID);
		IEnumerable<BIN_MASTER> FindByBIN_BIN_ID(FindComparison comparison, Int32 bIN_BIN_ID);
		IEnumerable<BIN_MASTER> FindByWH_ID(Int32 wH_ID);
		IEnumerable<BIN_MASTER> FindByWH_ID(FindComparison comparison, Int32 wH_ID);
		IEnumerable<BIN_MASTER> FindByBIN_NAME(String bIN_NAME);
		IEnumerable<BIN_MASTER> FindByBIN_NAME(FindComparison comparison, String bIN_NAME);
		IEnumerable<BIN_MASTER> FindByBIN_BAR_CODE(String bIN_BAR_CODE);
		IEnumerable<BIN_MASTER> FindByBIN_BAR_CODE(FindComparison comparison, String bIN_BAR_CODE);
		IEnumerable<BIN_MASTER> FindByBIN_PACKING_ORDER(Int32 bIN_PACKING_ORDER);
		IEnumerable<BIN_MASTER> FindByBIN_PACKING_ORDER(FindComparison comparison, Int32 bIN_PACKING_ORDER);
		IEnumerable<BIN_MASTER> FindByMAX_CONTAINERS(Int32 mAX_CONTAINERS);
		IEnumerable<BIN_MASTER> FindByMAX_CONTAINERS(FindComparison comparison, Int32 mAX_CONTAINERS);
		IEnumerable<BIN_MASTER> FindByMAX_WEIGHT(Decimal mAX_WEIGHT);
		IEnumerable<BIN_MASTER> FindByMAX_WEIGHT(FindComparison comparison, Decimal mAX_WEIGHT);
		IEnumerable<BIN_MASTER> FindByMAX_VOLUME(Decimal mAX_VOLUME);
		IEnumerable<BIN_MASTER> FindByMAX_VOLUME(FindComparison comparison, Decimal mAX_VOLUME);
		IEnumerable<BIN_MASTER> FindByMAX_HEIGHT(Decimal mAX_HEIGHT);
		IEnumerable<BIN_MASTER> FindByMAX_HEIGHT(FindComparison comparison, Decimal mAX_HEIGHT);
		IEnumerable<BIN_MASTER> FindByMAX_DEPTH(Decimal mAX_DEPTH);
		IEnumerable<BIN_MASTER> FindByMAX_DEPTH(FindComparison comparison, Decimal mAX_DEPTH);
		IEnumerable<BIN_MASTER> FindByMAX_WIDTH(Decimal mAX_WIDTH);
		IEnumerable<BIN_MASTER> FindByMAX_WIDTH(FindComparison comparison, Decimal mAX_WIDTH);
		IEnumerable<BIN_MASTER> FindByALLOW_MULTI_STOCK(Boolean aLLOW_MULTI_STOCK);
		IEnumerable<BIN_MASTER> FindByALLOW_MULTI_STOCK(FindComparison comparison, Boolean aLLOW_MULTI_STOCK);
		IEnumerable<BIN_MASTER> FindByTRANSITION_BIN(Boolean tRANSITION_BIN);
		IEnumerable<BIN_MASTER> FindByTRANSITION_BIN(FindComparison comparison, Boolean tRANSITION_BIN);
		IEnumerable<BIN_MASTER> FindByDEFAULT_RECEIPT_BIN(Boolean dEFAULT_RECEIPT_BIN);
		IEnumerable<BIN_MASTER> FindByDEFAULT_RECEIPT_BIN(FindComparison comparison, Boolean dEFAULT_RECEIPT_BIN);
		IEnumerable<BIN_MASTER> FindByCONTAINER_STORAGE_BIN(Boolean cONTAINER_STORAGE_BIN);
		IEnumerable<BIN_MASTER> FindByCONTAINER_STORAGE_BIN(FindComparison comparison, Boolean cONTAINER_STORAGE_BIN);
		IEnumerable<BIN_MASTER> FindByPICK_SEQUENCE(Int32 pICK_SEQUENCE);
		IEnumerable<BIN_MASTER> FindByPICK_SEQUENCE(FindComparison comparison, Int32 pICK_SEQUENCE);
		IEnumerable<BIN_MASTER> FindBySTOCK_TAKE(Boolean sTOCK_TAKE);
		IEnumerable<BIN_MASTER> FindBySTOCK_TAKE(FindComparison comparison, Boolean sTOCK_TAKE);
		IEnumerable<BIN_MASTER> FindByAUTO_ALLOCATE(Boolean aUTO_ALLOCATE);
		IEnumerable<BIN_MASTER> FindByAUTO_ALLOCATE(FindComparison comparison, Boolean aUTO_ALLOCATE);
		IEnumerable<BIN_MASTER> FindByBIN_FULL(Boolean bIN_FULL);
		IEnumerable<BIN_MASTER> FindByBIN_FULL(FindComparison comparison, Boolean bIN_FULL);
		IEnumerable<BIN_MASTER> FindByPALLETISATION_BIN(Boolean pALLETISATION_BIN);
		IEnumerable<BIN_MASTER> FindByPALLETISATION_BIN(FindComparison comparison, Boolean pALLETISATION_BIN);
		IEnumerable<BIN_MASTER> FindByZONE_ID(Int32 zONE_ID);
		IEnumerable<BIN_MASTER> FindByZONE_ID(FindComparison comparison, Int32 zONE_ID);
		IEnumerable<BIN_MASTER> FindByPOSTMAN_WALK_HOUSE_NO(Int32 pOSTMAN_WALK_HOUSE_NO);
		IEnumerable<BIN_MASTER> FindByPOSTMAN_WALK_HOUSE_NO(FindComparison comparison, Int32 pOSTMAN_WALK_HOUSE_NO);
		IEnumerable<BIN_MASTER> FindByZ_PICK_NO(Int32 z_PICK_NO);
		IEnumerable<BIN_MASTER> FindByZ_PICK_NO(FindComparison comparison, Int32 z_PICK_NO);
		IEnumerable<BIN_MASTER> FindByVIRTUAL_CONTAINERS_ONLY(Boolean vIRTUAL_CONTAINERS_ONLY);
		IEnumerable<BIN_MASTER> FindByVIRTUAL_CONTAINERS_ONLY(FindComparison comparison, Boolean vIRTUAL_CONTAINERS_ONLY);
		IEnumerable<BIN_MASTER> FindByTRANSITIONAL_BIN(Boolean tRANSITIONAL_BIN);
		IEnumerable<BIN_MASTER> FindByTRANSITIONAL_BIN(FindComparison comparison, Boolean tRANSITIONAL_BIN);
		IEnumerable<BIN_MASTER> FindByTYPE_ET(Boolean tYPE_ET);
		IEnumerable<BIN_MASTER> FindByTYPE_ET(FindComparison comparison, Boolean tYPE_ET);
		IEnumerable<BIN_MASTER> FindByINNER_HEIGHT(Decimal iNNER_HEIGHT);
		IEnumerable<BIN_MASTER> FindByINNER_HEIGHT(FindComparison comparison, Decimal iNNER_HEIGHT);
		IEnumerable<BIN_MASTER> FindByIS_TOP_SHELF(Boolean iS_TOP_SHELF);
		IEnumerable<BIN_MASTER> FindByIS_TOP_SHELF(FindComparison comparison, Boolean iS_TOP_SHELF);
		IEnumerable<BIN_MASTER> FindByTRANSIT_IN(Boolean tRANSIT_IN);
		IEnumerable<BIN_MASTER> FindByTRANSIT_IN(FindComparison comparison, Boolean tRANSIT_IN);
		IEnumerable<BIN_MASTER> FindByTRANSIT_OUT(Boolean tRANSIT_OUT);
		IEnumerable<BIN_MASTER> FindByTRANSIT_OUT(FindComparison comparison, Boolean tRANSIT_OUT);
		IEnumerable<BIN_MASTER> FindByWIP_RECEIVING(Boolean wIP_RECEIVING);
		IEnumerable<BIN_MASTER> FindByWIP_RECEIVING(FindComparison comparison, Boolean wIP_RECEIVING);
		IEnumerable<BIN_MASTER> FindByON_HOLD(Boolean oN_HOLD);
		IEnumerable<BIN_MASTER> FindByON_HOLD(FindComparison comparison, Boolean oN_HOLD);
		IEnumerable<BIN_MASTER> FindByRACK_LEVEL(String rACK_LEVEL);
		IEnumerable<BIN_MASTER> FindByRACK_LEVEL(FindComparison comparison, String rACK_LEVEL);
		IEnumerable<BIN_MASTER> FindByRACK_SEQUENCE(Int32 rACK_SEQUENCE);
		IEnumerable<BIN_MASTER> FindByRACK_SEQUENCE(FindComparison comparison, Int32 rACK_SEQUENCE);
		IEnumerable<BIN_MASTER> FindByWIP_RETURN_BIN(Int32 wIP_RETURN_BIN);
		IEnumerable<BIN_MASTER> FindByWIP_RETURN_BIN(FindComparison comparison, Int32 wIP_RETURN_BIN);
		IEnumerable<BIN_MASTER> FindByCHECK_DIGIT(Int32 cHECK_DIGIT);
		IEnumerable<BIN_MASTER> FindByCHECK_DIGIT(FindComparison comparison, Int32 cHECK_DIGIT);
		IEnumerable<BIN_MASTER> FindBySoftAllocWHArea(Int32 softAllocWHArea);
		IEnumerable<BIN_MASTER> FindBySoftAllocWHArea(FindComparison comparison, Int32 softAllocWHArea);
		IEnumerable<BIN_MASTER> FindByInPickingProcess(Boolean inPickingProcess);
		IEnumerable<BIN_MASTER> FindByInPickingProcess(FindComparison comparison, Boolean inPickingProcess);
		IEnumerable<BIN_MASTER> FindByPickingByRacId(Int32 pickingByRacId);
		IEnumerable<BIN_MASTER> FindByPickingByRacId(FindComparison comparison, Int32 pickingByRacId);
		IEnumerable<BIN_MASTER> FindByInPickingInstanceID(Int32 inPickingInstanceID);
		IEnumerable<BIN_MASTER> FindByInPickingInstanceID(FindComparison comparison, Int32 inPickingInstanceID);
		IEnumerable<BIN_MASTER> FindByUsePickFaceBinsForWIP(Boolean usePickFaceBinsForWIP);
		IEnumerable<BIN_MASTER> FindByUsePickFaceBinsForWIP(FindComparison comparison, Boolean usePickFaceBinsForWIP);
		IEnumerable<BIN_MASTER> FindBySingleInwardMovementsForWIP(Boolean singleInwardMovementsForWIP);
		IEnumerable<BIN_MASTER> FindBySingleInwardMovementsForWIP(FindComparison comparison, Boolean singleInwardMovementsForWIP);
		IEnumerable<BIN_MASTER> FindBySuspenseBin(Boolean suspenseBin);
		IEnumerable<BIN_MASTER> FindBySuspenseBin(FindComparison comparison, Boolean suspenseBin);
	}
	public sealed partial class BIN_MASTERRepository : BaseRepository<BIN_MASTER>, IBIN_MASTERRepository
	{
		public BIN_MASTERRepository(string connectionString) : this(connectionString, exception => { }) { }
		public BIN_MASTERRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "BIN_MASTER", 46)
		{
			Columns.Add(new ColumnDefinition("BIN_ID", typeof(System.Int32), "[INT]", false, true, false));
			Columns.Add(new ColumnDefinition("RACK_ID", typeof(System.Int32), "[INT]", false, false, false));
			Columns.Add(new ColumnDefinition("STATION_ID", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("BIN_BIN_ID", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("WH_ID", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("BIN_NAME", typeof(System.String), "[CHAR](30)", false, false, false));
			Columns.Add(new ColumnDefinition("BIN_BAR_CODE", typeof(System.String), "[CHAR](20)", true, false, false));
			Columns.Add(new ColumnDefinition("BIN_PACKING_ORDER", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("MAX_CONTAINERS", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("MAX_WEIGHT", typeof(System.Decimal), "[DECIMAL](22)", false, false, false));
			Columns.Add(new ColumnDefinition("MAX_VOLUME", typeof(System.Decimal), "[DECIMAL](22)", false, false, false));
			Columns.Add(new ColumnDefinition("MAX_HEIGHT", typeof(System.Decimal), "[DECIMAL](22)", false, false, false));
			Columns.Add(new ColumnDefinition("MAX_DEPTH", typeof(System.Decimal), "[DECIMAL](22)", false, false, false));
			Columns.Add(new ColumnDefinition("MAX_WIDTH", typeof(System.Decimal), "[DECIMAL](22)", false, false, false));
			Columns.Add(new ColumnDefinition("ALLOW_MULTI_STOCK", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("TRANSITION_BIN", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("DEFAULT_RECEIPT_BIN", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("CONTAINER_STORAGE_BIN", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("PICK_SEQUENCE", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("STOCK_TAKE", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("AUTO_ALLOCATE", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("BIN_FULL", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("PALLETISATION_BIN", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("ZONE_ID", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("POSTMAN_WALK_HOUSE_NO", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("Z_PICK_NO", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("VIRTUAL_CONTAINERS_ONLY", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("TRANSITIONAL_BIN", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("TYPE_ET", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("INNER_HEIGHT", typeof(System.Decimal), "[DECIMAL](15)", true, false, false));
			Columns.Add(new ColumnDefinition("IS_TOP_SHELF", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("TRANSIT_IN", typeof(System.Boolean), "[BIT]", true, false, false));
			Columns.Add(new ColumnDefinition("TRANSIT_OUT", typeof(System.Boolean), "[BIT]", true, false, false));
			Columns.Add(new ColumnDefinition("WIP_RECEIVING", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("ON_HOLD", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("RACK_LEVEL", typeof(System.String), "[CHAR](1)", false, false, false));
			Columns.Add(new ColumnDefinition("RACK_SEQUENCE", typeof(System.Int32), "[INT]", false, false, false));
			Columns.Add(new ColumnDefinition("WIP_RETURN_BIN", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("CHECK_DIGIT", typeof(System.Int32), "[INT]", false, false, false));
			Columns.Add(new ColumnDefinition("SoftAllocWHArea", typeof(System.Int32), "[INT]", true, false, false));
			Columns.Add(new ColumnDefinition("InPickingProcess", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("PickingByRacId", typeof(System.Int32), "[INT]", false, false, false));
			Columns.Add(new ColumnDefinition("InPickingInstanceID", typeof(System.Int32), "[INT]", false, false, false));
			Columns.Add(new ColumnDefinition("UsePickFaceBinsForWIP", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("SingleInwardMovementsForWIP", typeof(System.Boolean), "[BIT]", false, false, false));
			Columns.Add(new ColumnDefinition("SuspenseBin", typeof(System.Boolean), "[BIT]", false, false, false));
		}

		public BIN_MASTER Get(Int32 bIN_ID)
		{
			return Where("BIN_ID", Comparison.Equals, bIN_ID).Results().FirstOrDefault();
		}

		public IEnumerable<BIN_MASTER> Get(List<Int32> bIN_IDs)
		{
			return Get(bIN_IDs.ToArray());
		}

		public IEnumerable<BIN_MASTER> Get(params Int32[] bIN_IDs)
		{
			return Where("BIN_ID", Comparison.In, bIN_IDs).Results();
		}

		public override bool Create(BIN_MASTER item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.BIN_ID, item.RACK_ID, item.STATION_ID, item.BIN_BIN_ID, item.WH_ID, 
				item.BIN_NAME, item.BIN_BAR_CODE, item.BIN_PACKING_ORDER, item.MAX_CONTAINERS, item.MAX_WEIGHT, 
				item.MAX_VOLUME, item.MAX_HEIGHT, item.MAX_DEPTH, item.MAX_WIDTH, item.ALLOW_MULTI_STOCK, 
				item.TRANSITION_BIN, item.DEFAULT_RECEIPT_BIN, item.CONTAINER_STORAGE_BIN, item.PICK_SEQUENCE, item.STOCK_TAKE, 
				item.AUTO_ALLOCATE, item.BIN_FULL, item.PALLETISATION_BIN, item.ZONE_ID, item.POSTMAN_WALK_HOUSE_NO, 
				item.Z_PICK_NO, item.VIRTUAL_CONTAINERS_ONLY, item.TRANSITIONAL_BIN, item.TYPE_ET, item.INNER_HEIGHT, 
				item.IS_TOP_SHELF, item.TRANSIT_IN, item.TRANSIT_OUT, item.WIP_RECEIVING, item.ON_HOLD, 
				item.RACK_LEVEL, item.RACK_SEQUENCE, item.WIP_RETURN_BIN, item.CHECK_DIGIT, item.SoftAllocWHArea, 
				item.InPickingProcess, item.PickingByRacId, item.InPickingInstanceID, item.UsePickFaceBinsForWIP, item.SingleInwardMovementsForWIP, 
				item.SuspenseBin);
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
				return false;

			item.BIN_ID = (Int32)createdKeys[nameof(BIN_MASTER.BIN_ID)];
			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params BIN_MASTER[] items)
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
				dt.Rows.Add(item.BIN_ID, item.RACK_ID, item.STATION_ID, item.BIN_BIN_ID, item.WH_ID, 
				item.BIN_NAME, item.BIN_BAR_CODE, item.BIN_PACKING_ORDER, item.MAX_CONTAINERS, item.MAX_WEIGHT, 
				item.MAX_VOLUME, item.MAX_HEIGHT, item.MAX_DEPTH, item.MAX_WIDTH, item.ALLOW_MULTI_STOCK, 
				item.TRANSITION_BIN, item.DEFAULT_RECEIPT_BIN, item.CONTAINER_STORAGE_BIN, item.PICK_SEQUENCE, item.STOCK_TAKE, 
				item.AUTO_ALLOCATE, item.BIN_FULL, item.PALLETISATION_BIN, item.ZONE_ID, item.POSTMAN_WALK_HOUSE_NO, 
				item.Z_PICK_NO, item.VIRTUAL_CONTAINERS_ONLY, item.TRANSITIONAL_BIN, item.TYPE_ET, item.INNER_HEIGHT, 
				item.IS_TOP_SHELF, item.TRANSIT_IN, item.TRANSIT_OUT, item.WIP_RECEIVING, item.ON_HOLD, 
				item.RACK_LEVEL, item.RACK_SEQUENCE, item.WIP_RETURN_BIN, item.CHECK_DIGIT, item.SoftAllocWHArea, 
				item.InPickingProcess, item.PickingByRacId, item.InPickingInstanceID, item.UsePickFaceBinsForWIP, item.SingleInwardMovementsForWIP, 
				item.SuspenseBin); 
			}

			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<BIN_MASTER> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(BIN_MASTER item)
		{
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.BIN_ID, item.RACK_ID, item.STATION_ID, item.BIN_BIN_ID, item.WH_ID, 
				item.BIN_NAME, item.BIN_BAR_CODE, item.BIN_PACKING_ORDER, item.MAX_CONTAINERS, item.MAX_WEIGHT, 
				item.MAX_VOLUME, item.MAX_HEIGHT, item.MAX_DEPTH, item.MAX_WIDTH, item.ALLOW_MULTI_STOCK, 
				item.TRANSITION_BIN, item.DEFAULT_RECEIPT_BIN, item.CONTAINER_STORAGE_BIN, item.PICK_SEQUENCE, item.STOCK_TAKE, 
				item.AUTO_ALLOCATE, item.BIN_FULL, item.PALLETISATION_BIN, item.ZONE_ID, item.POSTMAN_WALK_HOUSE_NO, 
				item.Z_PICK_NO, item.VIRTUAL_CONTAINERS_ONLY, item.TRANSITIONAL_BIN, item.TYPE_ET, item.INNER_HEIGHT, 
				item.IS_TOP_SHELF, item.TRANSIT_IN, item.TRANSIT_OUT, item.WIP_RECEIVING, item.ON_HOLD, 
				item.RACK_LEVEL, item.RACK_SEQUENCE, item.WIP_RETURN_BIN, item.CHECK_DIGIT, item.SoftAllocWHArea, 
				item.InPickingProcess, item.PickingByRacId, item.InPickingInstanceID, item.UsePickFaceBinsForWIP, item.SingleInwardMovementsForWIP, 
				item.SuspenseBin);

			if (success)
			item.ResetDirty();

			return success;
		}
		public bool Delete(BIN_MASTER bIN_MASTER)
		{
			if (bIN_MASTER == null)
				return false;

			var deleteColumn = new DeleteColumn("BIN_ID", bIN_MASTER.BIN_ID);

			return BaseDelete(deleteColumn);
		}
		public bool Delete(IEnumerable<BIN_MASTER> items)
		{
			if (!items.Any()) return true;
			var deleteValues = new List<object>();
			foreach (var item in items)
			{
				deleteValues.Add(item.BIN_ID);
			}

			return BaseDelete("BIN_ID", deleteValues);
		}

		public bool Delete(Int32 bIN_ID)
		{
			return Delete(new BIN_MASTER { BIN_ID = bIN_ID });
		}


		public bool Delete(IEnumerable<Int32> bIN_IDs)
		{
			return Delete(bIN_IDs.Select(x => new BIN_MASTER { BIN_ID = x }));
		}


		public bool Merge(List<BIN_MASTER> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.BIN_ID,
					item.RACK_ID, item.DirtyColumns.Contains("RACK_ID"),
					item.STATION_ID, item.DirtyColumns.Contains("STATION_ID"),
					item.BIN_BIN_ID, item.DirtyColumns.Contains("BIN_BIN_ID"),
					item.WH_ID, item.DirtyColumns.Contains("WH_ID"),
					item.BIN_NAME, item.DirtyColumns.Contains("BIN_NAME"),
					item.BIN_BAR_CODE, item.DirtyColumns.Contains("BIN_BAR_CODE"),
					item.BIN_PACKING_ORDER, item.DirtyColumns.Contains("BIN_PACKING_ORDER"),
					item.MAX_CONTAINERS, item.DirtyColumns.Contains("MAX_CONTAINERS"),
					item.MAX_WEIGHT, item.DirtyColumns.Contains("MAX_WEIGHT"),
					item.MAX_VOLUME, item.DirtyColumns.Contains("MAX_VOLUME"),
					item.MAX_HEIGHT, item.DirtyColumns.Contains("MAX_HEIGHT"),
					item.MAX_DEPTH, item.DirtyColumns.Contains("MAX_DEPTH"),
					item.MAX_WIDTH, item.DirtyColumns.Contains("MAX_WIDTH"),
					item.ALLOW_MULTI_STOCK, item.DirtyColumns.Contains("ALLOW_MULTI_STOCK"),
					item.TRANSITION_BIN, item.DirtyColumns.Contains("TRANSITION_BIN"),
					item.DEFAULT_RECEIPT_BIN, item.DirtyColumns.Contains("DEFAULT_RECEIPT_BIN"),
					item.CONTAINER_STORAGE_BIN, item.DirtyColumns.Contains("CONTAINER_STORAGE_BIN"),
					item.PICK_SEQUENCE, item.DirtyColumns.Contains("PICK_SEQUENCE"),
					item.STOCK_TAKE, item.DirtyColumns.Contains("STOCK_TAKE"),
					item.AUTO_ALLOCATE, item.DirtyColumns.Contains("AUTO_ALLOCATE"),
					item.BIN_FULL, item.DirtyColumns.Contains("BIN_FULL"),
					item.PALLETISATION_BIN, item.DirtyColumns.Contains("PALLETISATION_BIN"),
					item.ZONE_ID, item.DirtyColumns.Contains("ZONE_ID"),
					item.POSTMAN_WALK_HOUSE_NO, item.DirtyColumns.Contains("POSTMAN_WALK_HOUSE_NO"),
					item.Z_PICK_NO, item.DirtyColumns.Contains("Z_PICK_NO"),
					item.VIRTUAL_CONTAINERS_ONLY, item.DirtyColumns.Contains("VIRTUAL_CONTAINERS_ONLY"),
					item.TRANSITIONAL_BIN, item.DirtyColumns.Contains("TRANSITIONAL_BIN"),
					item.TYPE_ET, item.DirtyColumns.Contains("TYPE_ET"),
					item.INNER_HEIGHT, item.DirtyColumns.Contains("INNER_HEIGHT"),
					item.IS_TOP_SHELF, item.DirtyColumns.Contains("IS_TOP_SHELF"),
					item.TRANSIT_IN, item.DirtyColumns.Contains("TRANSIT_IN"),
					item.TRANSIT_OUT, item.DirtyColumns.Contains("TRANSIT_OUT"),
					item.WIP_RECEIVING, item.DirtyColumns.Contains("WIP_RECEIVING"),
					item.ON_HOLD, item.DirtyColumns.Contains("ON_HOLD"),
					item.RACK_LEVEL, item.DirtyColumns.Contains("RACK_LEVEL"),
					item.RACK_SEQUENCE, item.DirtyColumns.Contains("RACK_SEQUENCE"),
					item.WIP_RETURN_BIN, item.DirtyColumns.Contains("WIP_RETURN_BIN"),
					item.CHECK_DIGIT, item.DirtyColumns.Contains("CHECK_DIGIT"),
					item.SoftAllocWHArea, item.DirtyColumns.Contains("SoftAllocWHArea"),
					item.InPickingProcess, item.DirtyColumns.Contains("InPickingProcess"),
					item.PickingByRacId, item.DirtyColumns.Contains("PickingByRacId"),
					item.InPickingInstanceID, item.DirtyColumns.Contains("InPickingInstanceID"),
					item.UsePickFaceBinsForWIP, item.DirtyColumns.Contains("UsePickFaceBinsForWIP"),
					item.SingleInwardMovementsForWIP, item.DirtyColumns.Contains("SingleInwardMovementsForWIP"),
					item.SuspenseBin, item.DirtyColumns.Contains("SuspenseBin")
				});
			}
			return BaseMerge(mergeTable);
		}

		protected override BIN_MASTER ToItem(DataRow row)
		{
			 var item = new BIN_MASTER
			{
				BIN_ID = GetInt32(row, "BIN_ID"),
				RACK_ID = GetInt32(row, "RACK_ID"),
				STATION_ID = GetNullableInt32(row, "STATION_ID"),
				BIN_BIN_ID = GetNullableInt32(row, "BIN_BIN_ID"),
				WH_ID = GetNullableInt32(row, "WH_ID"),
				BIN_NAME = GetString(row, "BIN_NAME"),
				BIN_BAR_CODE = GetString(row, "BIN_BAR_CODE"),
				BIN_PACKING_ORDER = GetNullableInt32(row, "BIN_PACKING_ORDER"),
				MAX_CONTAINERS = GetNullableInt32(row, "MAX_CONTAINERS"),
				MAX_WEIGHT = GetDecimal(row, "MAX_WEIGHT"),
				MAX_VOLUME = GetDecimal(row, "MAX_VOLUME"),
				MAX_HEIGHT = GetDecimal(row, "MAX_HEIGHT"),
				MAX_DEPTH = GetDecimal(row, "MAX_DEPTH"),
				MAX_WIDTH = GetDecimal(row, "MAX_WIDTH"),
				ALLOW_MULTI_STOCK = GetBoolean(row, "ALLOW_MULTI_STOCK"),
				TRANSITION_BIN = GetBoolean(row, "TRANSITION_BIN"),
				DEFAULT_RECEIPT_BIN = GetBoolean(row, "DEFAULT_RECEIPT_BIN"),
				CONTAINER_STORAGE_BIN = GetBoolean(row, "CONTAINER_STORAGE_BIN"),
				PICK_SEQUENCE = GetNullableInt32(row, "PICK_SEQUENCE"),
				STOCK_TAKE = GetBoolean(row, "STOCK_TAKE"),
				AUTO_ALLOCATE = GetBoolean(row, "AUTO_ALLOCATE"),
				BIN_FULL = GetBoolean(row, "BIN_FULL"),
				PALLETISATION_BIN = GetBoolean(row, "PALLETISATION_BIN"),
				ZONE_ID = GetNullableInt32(row, "ZONE_ID"),
				POSTMAN_WALK_HOUSE_NO = GetNullableInt32(row, "POSTMAN_WALK_HOUSE_NO"),
				Z_PICK_NO = GetNullableInt32(row, "Z_PICK_NO"),
				VIRTUAL_CONTAINERS_ONLY = GetBoolean(row, "VIRTUAL_CONTAINERS_ONLY"),
				TRANSITIONAL_BIN = GetBoolean(row, "TRANSITIONAL_BIN"),
				TYPE_ET = GetBoolean(row, "TYPE_ET"),
				INNER_HEIGHT = GetNullableDecimal(row, "INNER_HEIGHT"),
				IS_TOP_SHELF = GetBoolean(row, "IS_TOP_SHELF"),
				TRANSIT_IN = GetNullableBoolean(row, "TRANSIT_IN"),
				TRANSIT_OUT = GetNullableBoolean(row, "TRANSIT_OUT"),
				WIP_RECEIVING = GetBoolean(row, "WIP_RECEIVING"),
				ON_HOLD = GetBoolean(row, "ON_HOLD"),
				RACK_LEVEL = GetString(row, "RACK_LEVEL"),
				RACK_SEQUENCE = GetInt32(row, "RACK_SEQUENCE"),
				WIP_RETURN_BIN = GetNullableInt32(row, "WIP_RETURN_BIN"),
				CHECK_DIGIT = GetInt32(row, "CHECK_DIGIT"),
				SoftAllocWHArea = GetNullableInt32(row, "SoftAllocWHArea"),
				InPickingProcess = GetBoolean(row, "InPickingProcess"),
				PickingByRacId = GetInt32(row, "PickingByRacId"),
				InPickingInstanceID = GetInt32(row, "InPickingInstanceID"),
				UsePickFaceBinsForWIP = GetBoolean(row, "UsePickFaceBinsForWIP"),
				SingleInwardMovementsForWIP = GetBoolean(row, "SingleInwardMovementsForWIP"),
				SuspenseBin = GetBoolean(row, "SuspenseBin"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<BIN_MASTER> Search(
			Int32? bIN_ID = null,
			Int32? rACK_ID = null,
			Int32? sTATION_ID = null,
			Int32? bIN_BIN_ID = null,
			Int32? wH_ID = null,
			String bIN_NAME = null,
			String bIN_BAR_CODE = null,
			Int32? bIN_PACKING_ORDER = null,
			Int32? mAX_CONTAINERS = null,
			Decimal? mAX_WEIGHT = null,
			Decimal? mAX_VOLUME = null,
			Decimal? mAX_HEIGHT = null,
			Decimal? mAX_DEPTH = null,
			Decimal? mAX_WIDTH = null,
			Boolean? aLLOW_MULTI_STOCK = null,
			Boolean? tRANSITION_BIN = null,
			Boolean? dEFAULT_RECEIPT_BIN = null,
			Boolean? cONTAINER_STORAGE_BIN = null,
			Int32? pICK_SEQUENCE = null,
			Boolean? sTOCK_TAKE = null,
			Boolean? aUTO_ALLOCATE = null,
			Boolean? bIN_FULL = null,
			Boolean? pALLETISATION_BIN = null,
			Int32? zONE_ID = null,
			Int32? pOSTMAN_WALK_HOUSE_NO = null,
			Int32? z_PICK_NO = null,
			Boolean? vIRTUAL_CONTAINERS_ONLY = null,
			Boolean? tRANSITIONAL_BIN = null,
			Boolean? tYPE_ET = null,
			Decimal? iNNER_HEIGHT = null,
			Boolean? iS_TOP_SHELF = null,
			Boolean? tRANSIT_IN = null,
			Boolean? tRANSIT_OUT = null,
			Boolean? wIP_RECEIVING = null,
			Boolean? oN_HOLD = null,
			String rACK_LEVEL = null,
			Int32? rACK_SEQUENCE = null,
			Int32? wIP_RETURN_BIN = null,
			Int32? cHECK_DIGIT = null,
			Int32? softAllocWHArea = null,
			Boolean? inPickingProcess = null,
			Int32? pickingByRacId = null,
			Int32? inPickingInstanceID = null,
			Boolean? usePickFaceBinsForWIP = null,
			Boolean? singleInwardMovementsForWIP = null,
			Boolean? suspenseBin = null)
		{
			var queries = new List<QueryItem>(); 

			if (bIN_ID.HasValue)
				queries.Add(new QueryItem("BIN_ID", bIN_ID));
			if (rACK_ID.HasValue)
				queries.Add(new QueryItem("RACK_ID", rACK_ID));
			if (sTATION_ID.HasValue)
				queries.Add(new QueryItem("STATION_ID", sTATION_ID));
			if (bIN_BIN_ID.HasValue)
				queries.Add(new QueryItem("BIN_BIN_ID", bIN_BIN_ID));
			if (wH_ID.HasValue)
				queries.Add(new QueryItem("WH_ID", wH_ID));
			if (!string.IsNullOrEmpty(bIN_NAME))
				queries.Add(new QueryItem("BIN_NAME", bIN_NAME));
			if (!string.IsNullOrEmpty(bIN_BAR_CODE))
				queries.Add(new QueryItem("BIN_BAR_CODE", bIN_BAR_CODE));
			if (bIN_PACKING_ORDER.HasValue)
				queries.Add(new QueryItem("BIN_PACKING_ORDER", bIN_PACKING_ORDER));
			if (mAX_CONTAINERS.HasValue)
				queries.Add(new QueryItem("MAX_CONTAINERS", mAX_CONTAINERS));
			if (mAX_WEIGHT.HasValue)
				queries.Add(new QueryItem("MAX_WEIGHT", mAX_WEIGHT));
			if (mAX_VOLUME.HasValue)
				queries.Add(new QueryItem("MAX_VOLUME", mAX_VOLUME));
			if (mAX_HEIGHT.HasValue)
				queries.Add(new QueryItem("MAX_HEIGHT", mAX_HEIGHT));
			if (mAX_DEPTH.HasValue)
				queries.Add(new QueryItem("MAX_DEPTH", mAX_DEPTH));
			if (mAX_WIDTH.HasValue)
				queries.Add(new QueryItem("MAX_WIDTH", mAX_WIDTH));
			if (aLLOW_MULTI_STOCK.HasValue)
				queries.Add(new QueryItem("ALLOW_MULTI_STOCK", aLLOW_MULTI_STOCK));
			if (tRANSITION_BIN.HasValue)
				queries.Add(new QueryItem("TRANSITION_BIN", tRANSITION_BIN));
			if (dEFAULT_RECEIPT_BIN.HasValue)
				queries.Add(new QueryItem("DEFAULT_RECEIPT_BIN", dEFAULT_RECEIPT_BIN));
			if (cONTAINER_STORAGE_BIN.HasValue)
				queries.Add(new QueryItem("CONTAINER_STORAGE_BIN", cONTAINER_STORAGE_BIN));
			if (pICK_SEQUENCE.HasValue)
				queries.Add(new QueryItem("PICK_SEQUENCE", pICK_SEQUENCE));
			if (sTOCK_TAKE.HasValue)
				queries.Add(new QueryItem("STOCK_TAKE", sTOCK_TAKE));
			if (aUTO_ALLOCATE.HasValue)
				queries.Add(new QueryItem("AUTO_ALLOCATE", aUTO_ALLOCATE));
			if (bIN_FULL.HasValue)
				queries.Add(new QueryItem("BIN_FULL", bIN_FULL));
			if (pALLETISATION_BIN.HasValue)
				queries.Add(new QueryItem("PALLETISATION_BIN", pALLETISATION_BIN));
			if (zONE_ID.HasValue)
				queries.Add(new QueryItem("ZONE_ID", zONE_ID));
			if (pOSTMAN_WALK_HOUSE_NO.HasValue)
				queries.Add(new QueryItem("POSTMAN_WALK_HOUSE_NO", pOSTMAN_WALK_HOUSE_NO));
			if (z_PICK_NO.HasValue)
				queries.Add(new QueryItem("Z_PICK_NO", z_PICK_NO));
			if (vIRTUAL_CONTAINERS_ONLY.HasValue)
				queries.Add(new QueryItem("VIRTUAL_CONTAINERS_ONLY", vIRTUAL_CONTAINERS_ONLY));
			if (tRANSITIONAL_BIN.HasValue)
				queries.Add(new QueryItem("TRANSITIONAL_BIN", tRANSITIONAL_BIN));
			if (tYPE_ET.HasValue)
				queries.Add(new QueryItem("TYPE_ET", tYPE_ET));
			if (iNNER_HEIGHT.HasValue)
				queries.Add(new QueryItem("INNER_HEIGHT", iNNER_HEIGHT));
			if (iS_TOP_SHELF.HasValue)
				queries.Add(new QueryItem("IS_TOP_SHELF", iS_TOP_SHELF));
			if (tRANSIT_IN.HasValue)
				queries.Add(new QueryItem("TRANSIT_IN", tRANSIT_IN));
			if (tRANSIT_OUT.HasValue)
				queries.Add(new QueryItem("TRANSIT_OUT", tRANSIT_OUT));
			if (wIP_RECEIVING.HasValue)
				queries.Add(new QueryItem("WIP_RECEIVING", wIP_RECEIVING));
			if (oN_HOLD.HasValue)
				queries.Add(new QueryItem("ON_HOLD", oN_HOLD));
			if (!string.IsNullOrEmpty(rACK_LEVEL))
				queries.Add(new QueryItem("RACK_LEVEL", rACK_LEVEL));
			if (rACK_SEQUENCE.HasValue)
				queries.Add(new QueryItem("RACK_SEQUENCE", rACK_SEQUENCE));
			if (wIP_RETURN_BIN.HasValue)
				queries.Add(new QueryItem("WIP_RETURN_BIN", wIP_RETURN_BIN));
			if (cHECK_DIGIT.HasValue)
				queries.Add(new QueryItem("CHECK_DIGIT", cHECK_DIGIT));
			if (softAllocWHArea.HasValue)
				queries.Add(new QueryItem("SoftAllocWHArea", softAllocWHArea));
			if (inPickingProcess.HasValue)
				queries.Add(new QueryItem("InPickingProcess", inPickingProcess));
			if (pickingByRacId.HasValue)
				queries.Add(new QueryItem("PickingByRacId", pickingByRacId));
			if (inPickingInstanceID.HasValue)
				queries.Add(new QueryItem("InPickingInstanceID", inPickingInstanceID));
			if (usePickFaceBinsForWIP.HasValue)
				queries.Add(new QueryItem("UsePickFaceBinsForWIP", usePickFaceBinsForWIP));
			if (singleInwardMovementsForWIP.HasValue)
				queries.Add(new QueryItem("SingleInwardMovementsForWIP", singleInwardMovementsForWIP));
			if (suspenseBin.HasValue)
				queries.Add(new QueryItem("SuspenseBin", suspenseBin));

			return BaseSearch(queries);
		}

		public IEnumerable<BIN_MASTER> FindByRACK_ID(Int32 rACK_ID)
		{
			return FindByRACK_ID(FindComparison.Equals, rACK_ID);
		}

		public IEnumerable<BIN_MASTER> FindByRACK_ID(FindComparison comparison, Int32 rACK_ID)
		{
			return Where("RACK_ID", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), rACK_ID).Results();
		}

		public IEnumerable<BIN_MASTER> FindBySTATION_ID(Int32 sTATION_ID)
		{
			return FindBySTATION_ID(FindComparison.Equals, sTATION_ID);
		}

		public IEnumerable<BIN_MASTER> FindBySTATION_ID(FindComparison comparison, Int32 sTATION_ID)
		{
			return Where("STATION_ID", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), sTATION_ID).Results();
		}

		public IEnumerable<BIN_MASTER> FindByBIN_BIN_ID(Int32 bIN_BIN_ID)
		{
			return FindByBIN_BIN_ID(FindComparison.Equals, bIN_BIN_ID);
		}

		public IEnumerable<BIN_MASTER> FindByBIN_BIN_ID(FindComparison comparison, Int32 bIN_BIN_ID)
		{
			return Where("BIN_BIN_ID", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), bIN_BIN_ID).Results();
		}

		public IEnumerable<BIN_MASTER> FindByWH_ID(Int32 wH_ID)
		{
			return FindByWH_ID(FindComparison.Equals, wH_ID);
		}

		public IEnumerable<BIN_MASTER> FindByWH_ID(FindComparison comparison, Int32 wH_ID)
		{
			return Where("WH_ID", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), wH_ID).Results();
		}

		public IEnumerable<BIN_MASTER> FindByBIN_NAME(String bIN_NAME)
		{
			return FindByBIN_NAME(FindComparison.Equals, bIN_NAME);
		}

		public IEnumerable<BIN_MASTER> FindByBIN_NAME(FindComparison comparison, String bIN_NAME)
		{
			return Where("BIN_NAME", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), bIN_NAME).Results();
		}

		public IEnumerable<BIN_MASTER> FindByBIN_BAR_CODE(String bIN_BAR_CODE)
		{
			return FindByBIN_BAR_CODE(FindComparison.Equals, bIN_BAR_CODE);
		}

		public IEnumerable<BIN_MASTER> FindByBIN_BAR_CODE(FindComparison comparison, String bIN_BAR_CODE)
		{
			return Where("BIN_BAR_CODE", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), bIN_BAR_CODE).Results();
		}

		public IEnumerable<BIN_MASTER> FindByBIN_PACKING_ORDER(Int32 bIN_PACKING_ORDER)
		{
			return FindByBIN_PACKING_ORDER(FindComparison.Equals, bIN_PACKING_ORDER);
		}

		public IEnumerable<BIN_MASTER> FindByBIN_PACKING_ORDER(FindComparison comparison, Int32 bIN_PACKING_ORDER)
		{
			return Where("BIN_PACKING_ORDER", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), bIN_PACKING_ORDER).Results();
		}

		public IEnumerable<BIN_MASTER> FindByMAX_CONTAINERS(Int32 mAX_CONTAINERS)
		{
			return FindByMAX_CONTAINERS(FindComparison.Equals, mAX_CONTAINERS);
		}

		public IEnumerable<BIN_MASTER> FindByMAX_CONTAINERS(FindComparison comparison, Int32 mAX_CONTAINERS)
		{
			return Where("MAX_CONTAINERS", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), mAX_CONTAINERS).Results();
		}

		public IEnumerable<BIN_MASTER> FindByMAX_WEIGHT(Decimal mAX_WEIGHT)
		{
			return FindByMAX_WEIGHT(FindComparison.Equals, mAX_WEIGHT);
		}

		public IEnumerable<BIN_MASTER> FindByMAX_WEIGHT(FindComparison comparison, Decimal mAX_WEIGHT)
		{
			return Where("MAX_WEIGHT", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), mAX_WEIGHT).Results();
		}

		public IEnumerable<BIN_MASTER> FindByMAX_VOLUME(Decimal mAX_VOLUME)
		{
			return FindByMAX_VOLUME(FindComparison.Equals, mAX_VOLUME);
		}

		public IEnumerable<BIN_MASTER> FindByMAX_VOLUME(FindComparison comparison, Decimal mAX_VOLUME)
		{
			return Where("MAX_VOLUME", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), mAX_VOLUME).Results();
		}

		public IEnumerable<BIN_MASTER> FindByMAX_HEIGHT(Decimal mAX_HEIGHT)
		{
			return FindByMAX_HEIGHT(FindComparison.Equals, mAX_HEIGHT);
		}

		public IEnumerable<BIN_MASTER> FindByMAX_HEIGHT(FindComparison comparison, Decimal mAX_HEIGHT)
		{
			return Where("MAX_HEIGHT", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), mAX_HEIGHT).Results();
		}

		public IEnumerable<BIN_MASTER> FindByMAX_DEPTH(Decimal mAX_DEPTH)
		{
			return FindByMAX_DEPTH(FindComparison.Equals, mAX_DEPTH);
		}

		public IEnumerable<BIN_MASTER> FindByMAX_DEPTH(FindComparison comparison, Decimal mAX_DEPTH)
		{
			return Where("MAX_DEPTH", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), mAX_DEPTH).Results();
		}

		public IEnumerable<BIN_MASTER> FindByMAX_WIDTH(Decimal mAX_WIDTH)
		{
			return FindByMAX_WIDTH(FindComparison.Equals, mAX_WIDTH);
		}

		public IEnumerable<BIN_MASTER> FindByMAX_WIDTH(FindComparison comparison, Decimal mAX_WIDTH)
		{
			return Where("MAX_WIDTH", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), mAX_WIDTH).Results();
		}

		public IEnumerable<BIN_MASTER> FindByALLOW_MULTI_STOCK(Boolean aLLOW_MULTI_STOCK)
		{
			return FindByALLOW_MULTI_STOCK(FindComparison.Equals, aLLOW_MULTI_STOCK);
		}

		public IEnumerable<BIN_MASTER> FindByALLOW_MULTI_STOCK(FindComparison comparison, Boolean aLLOW_MULTI_STOCK)
		{
			return Where("ALLOW_MULTI_STOCK", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), aLLOW_MULTI_STOCK).Results();
		}

		public IEnumerable<BIN_MASTER> FindByTRANSITION_BIN(Boolean tRANSITION_BIN)
		{
			return FindByTRANSITION_BIN(FindComparison.Equals, tRANSITION_BIN);
		}

		public IEnumerable<BIN_MASTER> FindByTRANSITION_BIN(FindComparison comparison, Boolean tRANSITION_BIN)
		{
			return Where("TRANSITION_BIN", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), tRANSITION_BIN).Results();
		}

		public IEnumerable<BIN_MASTER> FindByDEFAULT_RECEIPT_BIN(Boolean dEFAULT_RECEIPT_BIN)
		{
			return FindByDEFAULT_RECEIPT_BIN(FindComparison.Equals, dEFAULT_RECEIPT_BIN);
		}

		public IEnumerable<BIN_MASTER> FindByDEFAULT_RECEIPT_BIN(FindComparison comparison, Boolean dEFAULT_RECEIPT_BIN)
		{
			return Where("DEFAULT_RECEIPT_BIN", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), dEFAULT_RECEIPT_BIN).Results();
		}

		public IEnumerable<BIN_MASTER> FindByCONTAINER_STORAGE_BIN(Boolean cONTAINER_STORAGE_BIN)
		{
			return FindByCONTAINER_STORAGE_BIN(FindComparison.Equals, cONTAINER_STORAGE_BIN);
		}

		public IEnumerable<BIN_MASTER> FindByCONTAINER_STORAGE_BIN(FindComparison comparison, Boolean cONTAINER_STORAGE_BIN)
		{
			return Where("CONTAINER_STORAGE_BIN", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), cONTAINER_STORAGE_BIN).Results();
		}

		public IEnumerable<BIN_MASTER> FindByPICK_SEQUENCE(Int32 pICK_SEQUENCE)
		{
			return FindByPICK_SEQUENCE(FindComparison.Equals, pICK_SEQUENCE);
		}

		public IEnumerable<BIN_MASTER> FindByPICK_SEQUENCE(FindComparison comparison, Int32 pICK_SEQUENCE)
		{
			return Where("PICK_SEQUENCE", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), pICK_SEQUENCE).Results();
		}

		public IEnumerable<BIN_MASTER> FindBySTOCK_TAKE(Boolean sTOCK_TAKE)
		{
			return FindBySTOCK_TAKE(FindComparison.Equals, sTOCK_TAKE);
		}

		public IEnumerable<BIN_MASTER> FindBySTOCK_TAKE(FindComparison comparison, Boolean sTOCK_TAKE)
		{
			return Where("STOCK_TAKE", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), sTOCK_TAKE).Results();
		}

		public IEnumerable<BIN_MASTER> FindByAUTO_ALLOCATE(Boolean aUTO_ALLOCATE)
		{
			return FindByAUTO_ALLOCATE(FindComparison.Equals, aUTO_ALLOCATE);
		}

		public IEnumerable<BIN_MASTER> FindByAUTO_ALLOCATE(FindComparison comparison, Boolean aUTO_ALLOCATE)
		{
			return Where("AUTO_ALLOCATE", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), aUTO_ALLOCATE).Results();
		}

		public IEnumerable<BIN_MASTER> FindByBIN_FULL(Boolean bIN_FULL)
		{
			return FindByBIN_FULL(FindComparison.Equals, bIN_FULL);
		}

		public IEnumerable<BIN_MASTER> FindByBIN_FULL(FindComparison comparison, Boolean bIN_FULL)
		{
			return Where("BIN_FULL", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), bIN_FULL).Results();
		}

		public IEnumerable<BIN_MASTER> FindByPALLETISATION_BIN(Boolean pALLETISATION_BIN)
		{
			return FindByPALLETISATION_BIN(FindComparison.Equals, pALLETISATION_BIN);
		}

		public IEnumerable<BIN_MASTER> FindByPALLETISATION_BIN(FindComparison comparison, Boolean pALLETISATION_BIN)
		{
			return Where("PALLETISATION_BIN", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), pALLETISATION_BIN).Results();
		}

		public IEnumerable<BIN_MASTER> FindByZONE_ID(Int32 zONE_ID)
		{
			return FindByZONE_ID(FindComparison.Equals, zONE_ID);
		}

		public IEnumerable<BIN_MASTER> FindByZONE_ID(FindComparison comparison, Int32 zONE_ID)
		{
			return Where("ZONE_ID", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), zONE_ID).Results();
		}

		public IEnumerable<BIN_MASTER> FindByPOSTMAN_WALK_HOUSE_NO(Int32 pOSTMAN_WALK_HOUSE_NO)
		{
			return FindByPOSTMAN_WALK_HOUSE_NO(FindComparison.Equals, pOSTMAN_WALK_HOUSE_NO);
		}

		public IEnumerable<BIN_MASTER> FindByPOSTMAN_WALK_HOUSE_NO(FindComparison comparison, Int32 pOSTMAN_WALK_HOUSE_NO)
		{
			return Where("POSTMAN_WALK_HOUSE_NO", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), pOSTMAN_WALK_HOUSE_NO).Results();
		}

		public IEnumerable<BIN_MASTER> FindByZ_PICK_NO(Int32 z_PICK_NO)
		{
			return FindByZ_PICK_NO(FindComparison.Equals, z_PICK_NO);
		}

		public IEnumerable<BIN_MASTER> FindByZ_PICK_NO(FindComparison comparison, Int32 z_PICK_NO)
		{
			return Where("Z_PICK_NO", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), z_PICK_NO).Results();
		}

		public IEnumerable<BIN_MASTER> FindByVIRTUAL_CONTAINERS_ONLY(Boolean vIRTUAL_CONTAINERS_ONLY)
		{
			return FindByVIRTUAL_CONTAINERS_ONLY(FindComparison.Equals, vIRTUAL_CONTAINERS_ONLY);
		}

		public IEnumerable<BIN_MASTER> FindByVIRTUAL_CONTAINERS_ONLY(FindComparison comparison, Boolean vIRTUAL_CONTAINERS_ONLY)
		{
			return Where("VIRTUAL_CONTAINERS_ONLY", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), vIRTUAL_CONTAINERS_ONLY).Results();
		}

		public IEnumerable<BIN_MASTER> FindByTRANSITIONAL_BIN(Boolean tRANSITIONAL_BIN)
		{
			return FindByTRANSITIONAL_BIN(FindComparison.Equals, tRANSITIONAL_BIN);
		}

		public IEnumerable<BIN_MASTER> FindByTRANSITIONAL_BIN(FindComparison comparison, Boolean tRANSITIONAL_BIN)
		{
			return Where("TRANSITIONAL_BIN", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), tRANSITIONAL_BIN).Results();
		}

		public IEnumerable<BIN_MASTER> FindByTYPE_ET(Boolean tYPE_ET)
		{
			return FindByTYPE_ET(FindComparison.Equals, tYPE_ET);
		}

		public IEnumerable<BIN_MASTER> FindByTYPE_ET(FindComparison comparison, Boolean tYPE_ET)
		{
			return Where("TYPE_ET", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), tYPE_ET).Results();
		}

		public IEnumerable<BIN_MASTER> FindByINNER_HEIGHT(Decimal iNNER_HEIGHT)
		{
			return FindByINNER_HEIGHT(FindComparison.Equals, iNNER_HEIGHT);
		}

		public IEnumerable<BIN_MASTER> FindByINNER_HEIGHT(FindComparison comparison, Decimal iNNER_HEIGHT)
		{
			return Where("INNER_HEIGHT", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), iNNER_HEIGHT).Results();
		}

		public IEnumerable<BIN_MASTER> FindByIS_TOP_SHELF(Boolean iS_TOP_SHELF)
		{
			return FindByIS_TOP_SHELF(FindComparison.Equals, iS_TOP_SHELF);
		}

		public IEnumerable<BIN_MASTER> FindByIS_TOP_SHELF(FindComparison comparison, Boolean iS_TOP_SHELF)
		{
			return Where("IS_TOP_SHELF", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), iS_TOP_SHELF).Results();
		}

		public IEnumerable<BIN_MASTER> FindByTRANSIT_IN(Boolean tRANSIT_IN)
		{
			return FindByTRANSIT_IN(FindComparison.Equals, tRANSIT_IN);
		}

		public IEnumerable<BIN_MASTER> FindByTRANSIT_IN(FindComparison comparison, Boolean tRANSIT_IN)
		{
			return Where("TRANSIT_IN", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), tRANSIT_IN).Results();
		}

		public IEnumerable<BIN_MASTER> FindByTRANSIT_OUT(Boolean tRANSIT_OUT)
		{
			return FindByTRANSIT_OUT(FindComparison.Equals, tRANSIT_OUT);
		}

		public IEnumerable<BIN_MASTER> FindByTRANSIT_OUT(FindComparison comparison, Boolean tRANSIT_OUT)
		{
			return Where("TRANSIT_OUT", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), tRANSIT_OUT).Results();
		}

		public IEnumerable<BIN_MASTER> FindByWIP_RECEIVING(Boolean wIP_RECEIVING)
		{
			return FindByWIP_RECEIVING(FindComparison.Equals, wIP_RECEIVING);
		}

		public IEnumerable<BIN_MASTER> FindByWIP_RECEIVING(FindComparison comparison, Boolean wIP_RECEIVING)
		{
			return Where("WIP_RECEIVING", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), wIP_RECEIVING).Results();
		}

		public IEnumerable<BIN_MASTER> FindByON_HOLD(Boolean oN_HOLD)
		{
			return FindByON_HOLD(FindComparison.Equals, oN_HOLD);
		}

		public IEnumerable<BIN_MASTER> FindByON_HOLD(FindComparison comparison, Boolean oN_HOLD)
		{
			return Where("ON_HOLD", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), oN_HOLD).Results();
		}

		public IEnumerable<BIN_MASTER> FindByRACK_LEVEL(String rACK_LEVEL)
		{
			return FindByRACK_LEVEL(FindComparison.Equals, rACK_LEVEL);
		}

		public IEnumerable<BIN_MASTER> FindByRACK_LEVEL(FindComparison comparison, String rACK_LEVEL)
		{
			return Where("RACK_LEVEL", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), rACK_LEVEL).Results();
		}

		public IEnumerable<BIN_MASTER> FindByRACK_SEQUENCE(Int32 rACK_SEQUENCE)
		{
			return FindByRACK_SEQUENCE(FindComparison.Equals, rACK_SEQUENCE);
		}

		public IEnumerable<BIN_MASTER> FindByRACK_SEQUENCE(FindComparison comparison, Int32 rACK_SEQUENCE)
		{
			return Where("RACK_SEQUENCE", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), rACK_SEQUENCE).Results();
		}

		public IEnumerable<BIN_MASTER> FindByWIP_RETURN_BIN(Int32 wIP_RETURN_BIN)
		{
			return FindByWIP_RETURN_BIN(FindComparison.Equals, wIP_RETURN_BIN);
		}

		public IEnumerable<BIN_MASTER> FindByWIP_RETURN_BIN(FindComparison comparison, Int32 wIP_RETURN_BIN)
		{
			return Where("WIP_RETURN_BIN", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), wIP_RETURN_BIN).Results();
		}

		public IEnumerable<BIN_MASTER> FindByCHECK_DIGIT(Int32 cHECK_DIGIT)
		{
			return FindByCHECK_DIGIT(FindComparison.Equals, cHECK_DIGIT);
		}

		public IEnumerable<BIN_MASTER> FindByCHECK_DIGIT(FindComparison comparison, Int32 cHECK_DIGIT)
		{
			return Where("CHECK_DIGIT", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), cHECK_DIGIT).Results();
		}

		public IEnumerable<BIN_MASTER> FindBySoftAllocWHArea(Int32 softAllocWHArea)
		{
			return FindBySoftAllocWHArea(FindComparison.Equals, softAllocWHArea);
		}

		public IEnumerable<BIN_MASTER> FindBySoftAllocWHArea(FindComparison comparison, Int32 softAllocWHArea)
		{
			return Where("SoftAllocWHArea", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), softAllocWHArea).Results();
		}

		public IEnumerable<BIN_MASTER> FindByInPickingProcess(Boolean inPickingProcess)
		{
			return FindByInPickingProcess(FindComparison.Equals, inPickingProcess);
		}

		public IEnumerable<BIN_MASTER> FindByInPickingProcess(FindComparison comparison, Boolean inPickingProcess)
		{
			return Where("InPickingProcess", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), inPickingProcess).Results();
		}

		public IEnumerable<BIN_MASTER> FindByPickingByRacId(Int32 pickingByRacId)
		{
			return FindByPickingByRacId(FindComparison.Equals, pickingByRacId);
		}

		public IEnumerable<BIN_MASTER> FindByPickingByRacId(FindComparison comparison, Int32 pickingByRacId)
		{
			return Where("PickingByRacId", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), pickingByRacId).Results();
		}

		public IEnumerable<BIN_MASTER> FindByInPickingInstanceID(Int32 inPickingInstanceID)
		{
			return FindByInPickingInstanceID(FindComparison.Equals, inPickingInstanceID);
		}

		public IEnumerable<BIN_MASTER> FindByInPickingInstanceID(FindComparison comparison, Int32 inPickingInstanceID)
		{
			return Where("InPickingInstanceID", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), inPickingInstanceID).Results();
		}

		public IEnumerable<BIN_MASTER> FindByUsePickFaceBinsForWIP(Boolean usePickFaceBinsForWIP)
		{
			return FindByUsePickFaceBinsForWIP(FindComparison.Equals, usePickFaceBinsForWIP);
		}

		public IEnumerable<BIN_MASTER> FindByUsePickFaceBinsForWIP(FindComparison comparison, Boolean usePickFaceBinsForWIP)
		{
			return Where("UsePickFaceBinsForWIP", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), usePickFaceBinsForWIP).Results();
		}

		public IEnumerable<BIN_MASTER> FindBySingleInwardMovementsForWIP(Boolean singleInwardMovementsForWIP)
		{
			return FindBySingleInwardMovementsForWIP(FindComparison.Equals, singleInwardMovementsForWIP);
		}

		public IEnumerable<BIN_MASTER> FindBySingleInwardMovementsForWIP(FindComparison comparison, Boolean singleInwardMovementsForWIP)
		{
			return Where("SingleInwardMovementsForWIP", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), singleInwardMovementsForWIP).Results();
		}

		public IEnumerable<BIN_MASTER> FindBySuspenseBin(Boolean suspenseBin)
		{
			return FindBySuspenseBin(FindComparison.Equals, suspenseBin);
		}

		public IEnumerable<BIN_MASTER> FindBySuspenseBin(FindComparison comparison, Boolean suspenseBin)
		{
			return Where("SuspenseBin", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), suspenseBin).Results();
		}
	}
}
