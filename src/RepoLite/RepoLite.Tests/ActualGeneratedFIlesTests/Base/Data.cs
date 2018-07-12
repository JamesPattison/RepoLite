using NS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace RepoLite.Tests.ActualGeneratedFIlesTests.Base
{
    internal class Data
    {
        public static void DropAndCreateDatabase()
        {
            RunSql(@"DROP TABLE IF EXISTS [dbo].[Address]");
            RunSql(@"DROP TABLE IF EXISTS [dbo].[Person]");
            RunSql(@"DROP TABLE IF EXISTS [dbo].[Event]");
            RunSql(@"DROP TABLE IF EXISTS [dbo].[xmltable]");
            RunSql(@"DROP TABLE IF EXISTS [dbo].[NullableTable]");

            RunSql(@"CREATE TABLE [dbo].[Person](
	                    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	                    [Name] [nvarchar](50) NOT NULL,
	                    [Age] [int] NOT NULL,
	                    [Nationality] [nvarchar](50) NOT NULL,
	                    [Registered] [bit] NOT NULL
                    )");

            RunSql(@"CREATE TABLE [dbo].[Address](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [AnotherId] [nvarchar](10) NOT NULL,
	                    [PersonId] [int] NOT NULL,
	                    [Line1] [nvarchar](100) NOT NULL,
	                    [Line2] [nvarchar](100) NULL,
	                    [Line3] [nvarchar](100) NULL,
	                    [Line4] [nvarchar](100) NULL,
	                    [PostCode] [nvarchar](15) NOT NULL,
	                    [PhoneNumber] [nvarchar](20) NULL,
	                    [COUNTRY_CODE] [nvarchar](2) NULL,

                        CONSTRAINT [fk_a_p] FOREIGN KEY([PersonId]) REFERENCES [dbo].[Person] ([Id]),
                        PRIMARY KEY
                        (
	                        [Id] ASC,
	                        [AnotherId] ASC                    
                        )
                    )");

            RunSql(@"CREATE TABLE [dbo].[Event](
	                    [EventId] [nvarchar](20) NOT NULL PRIMARY KEY,
	                    [EventName] [nvarchar](100) NOT NULL
                    )");

            RunSql(@"CREATE TABLE [dbo].[xmltable] ([name] VARCHAR(12) not null, [data] XML not null)");

            RunSql(@"CREATE TABLE [dbo].[NullableTable](
            	        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            	        [Age] INT NULL,
            	        [DoB] DATETIME NULL,
            	        [lolVal] UNIQUEIDENTIFIER NULL
                    )");

            RunSql(@"CREATE TABLE [AWM].[BIN_MASTER](
	                        [BIN_ID] [int] NOT NULL,
	                        [RACK_ID] [int] NOT NULL,
	                        [STATION_ID] [int] NULL,
	                        [BIN_BIN_ID] [int] NULL,
	                        [WH_ID] [int] NULL,
	                        [BIN_NAME] [char](30) NOT NULL,
	                        [BIN_BAR_CODE] [char](20) NULL,
	                        [BIN_PACKING_ORDER] [int] NULL,
	                        [MAX_CONTAINERS] [int] NULL,
	                        [MAX_WEIGHT] [decimal](28, 6) NOT NULL,
	                        [MAX_VOLUME] [decimal](28, 6) NOT NULL,
	                        [MAX_HEIGHT] [decimal](28, 6) NOT NULL,
	                        [MAX_DEPTH] [decimal](28, 6) NOT NULL,
	                        [MAX_WIDTH] [decimal](28, 6) NOT NULL,
	                        [ALLOW_MULTI_STOCK] [bit] NOT NULL,
	                        [TRANSITION_BIN] [bit] NOT NULL,
	                        [DEFAULT_RECEIPT_BIN] [bit] NOT NULL,
	                        [CONTAINER_STORAGE_BIN] [bit] NOT NULL,
	                        [PICK_SEQUENCE] [int] NULL,
	                        [STOCK_TAKE] [bit] NOT NULL,
	                        [AUTO_ALLOCATE] [bit] NOT NULL,
	                        [BIN_FULL] [bit] NOT NULL,
	                        [PALLETISATION_BIN] [bit] NOT NULL,
	                        [ZONE_ID] [int] NULL,
	                        [POSTMAN_WALK_HOUSE_NO] [int] NULL,
	                        [Z_PICK_NO] [int] NULL,
	                        [VIRTUAL_CONTAINERS_ONLY] [bit] NOT NULL,
	                        [TRANSITIONAL_BIN] [bit] NOT NULL,
	                        [TYPE_ET] [bit] NOT NULL,
	                        [INNER_HEIGHT] [decimal](18, 3) NULL,
	                        [IS_TOP_SHELF] [bit] NOT NULL,
	                        [TRANSIT_IN] [bit] NULL,
	                        [TRANSIT_OUT] [bit] NULL,
	                        [WIP_RECEIVING] [bit] NOT NULL,
	                        [ON_HOLD] [bit] NOT NULL,
	                        [RACK_LEVEL] [char](1) NOT NULL,
	                        [RACK_SEQUENCE] [int] NOT NULL,
	                        [WIP_RETURN_BIN] [int] NULL,
	                        [CHECK_DIGIT] [int] NOT NULL,
	                        [SoftAllocWHArea] [int] NULL,
	                        [InPickingProcess] [bit] NOT NULL,
	                        [PickingByRacId] [int] NOT NULL,
	                        [InPickingInstanceID] [int] NOT NULL,
	                        [UsePickFaceBinsForWIP] [bit] NOT NULL,
	                        [SingleInwardMovementsForWIP] [bit] NOT NULL,
	                        [SuspenseBin] [bit] NOT NULL,
                         CONSTRAINT [PK_BIN_MASTER] PRIMARY KEY CLUSTERED 
                        (
	                        [BIN_ID] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY]");

            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Jim Pattison', 31, 'British', 1)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Rebecca Pattison', 30, 'British', 0)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Raffaela Tinker', 65, 'Dutch', 1)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Sara Vroom', 44, 'French', 1)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Josse Ewart', 41, 'French', 0)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Theutrich Hynes', 35, 'German', 0)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Yasmeen Orbán', 12, 'Indian', 0)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Nicostratus Meissner', 87, 'Greek', 1)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Linda Heffernan', 51, 'British', 1)");
            RunSql("INSERT [dbo].[Person] ([Name],[Age],[Nationality],[Registered]) VALUES ('Serenity Salucci', 17, 'Italian', 0)");


            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('77',4,'01515 Considine Lodge','New Marjorie','Borders','United States Minor Outlying Islands','45200-5387','01002 832512','FR')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('3',4,'91643 Cormier Bridge','Parisiantown','Bedfordshire','Bahamas','01763-4987','01728 542265','LC')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('14',3,'12043 Roob Inlet','North Shaniya','Bedfordshire','Monaco','10326-2100','01605 696163','AE')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('2',9,'1113 Feil Lock','Port Thad','Cambridgeshire','Albania','37396','01671 734615','LK')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('97',1,'8989 Raynor Lake','New Lavernamouth','Avon','Norfolk Island','25632-5757','01805 968151','TW')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('54',7,'5516 Reilly Shoal','North Johnny','Bedfordshire','Uruguay','05472-9584','01833 406229','MY')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('1',8,'264 Yasmin Mountains','West Talia','Berkshire','Monaco','56468-6584','01101 018567','MF')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('12',7,'94940 Eudora Drive','North Merle','Borders','Kiribati','34763-0932','01568 719948','CN')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('47',7,'9331 Zieme Skyway','Gwendolynland','Cambridgeshire','Kazakhstan','43015','01351 315683','FJ')");
            RunSql("INSERT [dbo].[Address] ([AnotherId],[PersonId],[Line1],[Line2],[Line3],[Line4],[PostCode],[PhoneNumber],[COUNTRY_CODE]) VALUES ('47',6,'52137 Hills Walk','Lake Nattown','Avon','Nigeria','80392-8348','01235 987526','DJ')");

            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_01','Car Thief 1')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_02','Duel (Next-gen Only)')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_03','Monkey Mosaic (Next-gen Only)')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_04','Sea Plane (Next-gen only)')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_05','ATM Robberies.')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_06','Bike Thief City 1.')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_07','Bike Thief City 2.')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_08','Bus Tour.')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_09','Construction Accident.')");
            RunSql("INSERT [dbo].[Event] ([EventId], [EventName]) VALUES ('EVT_10','Sports Bike Thief')");

            RunSql("INSERT [dbo].[xmltable] VALUES ('XML1','<xml>Value</xml>')");
            RunSql("INSERT [dbo].[xmltable] VALUES ('XML2', '<xml>Another Value</xml>')");
            RunSql("INSERT [dbo].[xmltable] VALUES ('XML3', '<xml>Yet Another Value</xml>')");
            RunSql("INSERT [dbo].[xmltable] VALUES ('XML4', '<xml><nest>Nested!</nest></xml>')");
            RunSql("INSERT [dbo].[xmltable] VALUES ('XML5', '<xml><nest><nest>Nested Further!</nest></nest></xml>')");

            RunSql("INSERT [dbo].[NullableTable] (Age, DoB, lolVal) VALUES(31, '2018-02-19 09:55:16.057', '84F00717-BFDB-49ED-A116-02578C4D2513')");
            RunSql("INSERT [dbo].[NullableTable] (Age, DoB, lolVal) VALUES(27, '2016-11-12', '847ED511-C462-4478-BD98-02625D5F6BCF')");
            RunSql("INSERT [dbo].[NullableTable] (Age, DoB, lolVal) VALUES(2, '2017-05-01 21:21:21', '63C5DD71-060F-4DEE-81A4-5203277EEED8')");

            RunSql("INSERT INTO [AWM].[BIN_MASTER] ([BIN_ID],[RACK_ID],[STATION_ID],[BIN_NAME],[MAX_WEIGHT],[MAX_VOLUME],[MAX_HEIGHT],[MAX_DEPTH],[MAX_WIDTH],[ALLOW_MULTI_STOCK],[TRANSITION_BIN],[DEFAULT_RECEIPT_BIN],[CONTAINER_STORAGE_BIN],[STOCK_TAKE],[AUTO_ALLOCATE],[BIN_FULL],[PALLETISATION_BIN],[VIRTUAL_CONTAINERS_ONLY],[TRANSITIONAL_BIN],[TYPE_ET],[IS_TOP_SHELF],[TRANSIT_IN],[TRANSIT_OUT],[WIP_RECEIVING],[ON_HOLD],[RACK_LEVEL],[RACK_SEQUENCE],[CHECK_DIGIT],[SoftAllocWHArea],[InPickingProcess],[PickingByRacId],[InPickingInstanceID],[UsePickFaceBinsForWIP],[SingleInwardMovementsForWIP],[SuspenseBin]) VALUES ('1', '1', '1', '1', '1','1', '1', '1', '1', '1','1', '1', '1', '1', '1','1', '1', '1', '1', '1','1', '1', '1', '1', '1','1', '1', '1', '1', '1','1', '1', '1', '1', '1')");
        }

        private static void RunSql(string sql)
        {
            using (var cn = new SqlConnection("Data Source=.;Initial Catalog=RepoDev;Integrated Security=true;"))
            {
                using (var cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = CommandType.Text,
                    CommandText = sql
                })
                {
                    cn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                    cn.Close();
                }
            }
        }

        public static List<Address> Addresses => new List<Address>
        {
            new Address{Id = 1, AnotherId = "77",PersonId = 4,Line1 = "01515 Considine Lodge",Line2 = "New Marjorie",Line3 = "Borders",Line4 = "United States Minor Outlying Islands",PostCode = "45200-5387",PhoneNumber = "01002 832512",COUNTRY_CODE = "FR"},
            new Address{Id = 2, AnotherId = "3",PersonId = 4,Line1 = "91643 Cormier Bridge",Line2 = "Parisiantown",Line3 = "Bedfordshire",Line4 = "Bahamas",PostCode = "01763-4987",PhoneNumber = "01728 542265",COUNTRY_CODE = "LC"},
            new Address{Id = 3, AnotherId = "14",PersonId = 3,Line1 = "12043 Roob Inlet",Line2 = "North Shaniya",Line3 = "Bedfordshire",Line4 = "Monaco",PostCode = "10326-2100",PhoneNumber = "01605 696163",COUNTRY_CODE = "AE"},
            new Address{Id = 4, AnotherId = "2",PersonId = 9,Line1 = "1113 Feil Lock",Line2 = "Port Thad",Line3 = "Cambridgeshire",Line4 = "Albania",PostCode = "37396",PhoneNumber = "01671 734615",COUNTRY_CODE = "LK"},
            new Address{Id = 5, AnotherId = "97",PersonId = 1,Line1 = "8989 Raynor Lake",Line2 = "New Lavernamouth",Line3 = "Avon",Line4 = "Norfolk Island",PostCode = "25632-5757",PhoneNumber = "01805 968151",COUNTRY_CODE = "TW"},
            new Address{Id = 6, AnotherId = "54",PersonId = 7,Line1 = "5516 Reilly Shoal",Line2 = "North Johnny",Line3 = "Bedfordshire",Line4 = "Uruguay",PostCode = "05472-9584",PhoneNumber = "01833 406229",COUNTRY_CODE = "MY"},
            new Address{Id = 7, AnotherId = "1",PersonId = 8,Line1 = "264 Yasmin Mountains",Line2 = "West Talia",Line3 = "Berkshire",Line4 = "Monaco",PostCode = "56468-6584",PhoneNumber = "01101 018567",COUNTRY_CODE = "MF"},
            new Address{Id = 8, AnotherId = "12",PersonId = 7,Line1 = "94940 Eudora Drive",Line2 = "North Merle",Line3 = "Borders",Line4 = "Kiribati",PostCode = "34763-0932",PhoneNumber = "01568 719948",COUNTRY_CODE = "CN"},
            new Address{Id = 9, AnotherId = "47",PersonId = 7,Line1 = "9331 Zieme Skyway",Line2 = "Gwendolynland",Line3 = "Cambridgeshire",Line4 = "Kazakhstan",PostCode = "43015",PhoneNumber = "01351 315683",COUNTRY_CODE = "FJ"},
            new Address{Id = 10, AnotherId = "47",PersonId = 6,Line1 = "52137 Hills Walk",Line2 = "Lake Nattown",Line3 = "Avon",Line4 = "Nigeria",PostCode = "80392-8348",PhoneNumber = "01235 987526",COUNTRY_CODE = "DJ"}
        };
    }
}
