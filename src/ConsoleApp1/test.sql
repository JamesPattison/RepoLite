--Setup data
create type CompanyCarType as table(
	Make nvarchar(256) not null,
	Model nvarchar(256) not null,
	OwnerName nvarchar(256) not null
)
create table Company(
	Id bigint primary key,
	Name nvarchar(256) not null,
	ExployeeCount bigint not null
)
create table Owner(
	Id bigint primary key,
	CompanyId bigint,
	FirstName nvarchar(256),
	LastName nvarchar(256),
	DateOfBirth Datetime,

	constraint owner_fk foreign key(CompanyId) references Company(Id)
)
create table Cars(
	Id bigint primary key,
	OwnerId Bigint not null,
	Make nvarchar(256) not null,
	Model nvarchar(256) not null,
	
	constraint car_owner_fk foreign key (OwnerId) references Owner(Id)
)

insert Company values(1, 'Microsoft', 1000)
insert Company values(2, 'Apple', 2000)

insert Owner values(1, 1, 'John', 'Cox', '1976-06-06')
insert Owner values(2, 1, 'Cobie', 'Pugh', '1972-02-17')
insert Owner values(3, 1, 'Leyton', 'Bull', '1973-09-21')
insert Owner values(4, 1, 'Sebastien', 'Doyle', '1976-10-12')
insert Owner values(5, 2, 'Kush', 'Lancaster', '1984-03-09')
insert Owner values(6, 2, 'Taran', 'Bradford', '1988-02-15')
insert Owner values(7, 2, 'Karishma', 'Crosby', '1995-05-08')
insert Owner values(8, 2, 'Petra', 'Donnelly', '2002-05-09')

insert Cars values (1, 1, 'Suzuki', 'Swift')
insert Cars values (2, 2, 'Mitsubishi', 'Mirage')
insert Cars values (3, 3, 'Peugeot', '108')
insert Cars values (4, 4, 'Fiat', '500')
insert Cars values (5, 7, 'Toyota', 'Aygo')
insert Cars values (6, 8, 'Jeep', 'Compass')
go

-- Takes params, no return
create procedure CreateOwner (
	@companyId bigint,
	@firstName nvarchar(256),
	@lastName varchar(512),
	@dateOfBirth datetime)
as
	
	-- lol select max - idc
	insert Owner values((select MAX(Id)+1 FROM Owner), @companyId, @firstName, @lastName, @dateOfBirth)

GO


-- Takes params, one table return
create procedure FindOwnersCarByName (
	@firstName nvarchar(256),
	@lastName varchar(512))
as
	
	select c.* from owner o
	join Cars c
		on o.Id = c.OwnerId
	where o.FirstName = @firstName and o.LastName = @lastName

GO

-- Takes params, multiple table return
create procedure GetCompanyInfo (
	@companyId bigint)
as
	
	--owners
	select * from Owner o where o.CompanyId = @companyId

	--cars
	select c.* from owner o
	join Cars c
		on o.Id = c.OwnerId
	where o.CompanyId = @companyId

GO

-- Takes custom type
create procedure CreateCars (
	@cars CompanyCarType READONLY)
as
	--worst proc ever lol
	DECLARE @make NVARCHAR(256), @model NVARCHAR(256), @ownerName NVARCHAR(256), @ownerId bigint;  

	DECLARE car_cursor CURSOR FOR   
	SELECT Make, Model, OwnerName
	FROM @cars;  
  
	OPEN car_cursor  
  
	FETCH NEXT FROM car_cursor   
	INTO @make, @model, @ownerName 
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
  
		set @ownerId = (select Id from Owner where FirstName = @ownerName)
		if (@ownerId is not null) begin
			insert Cars values ((select max(id)+1 from Cars), @ownerId, @make, @model)
		end

		FETCH NEXT FROM car_cursor   
		INTO @make, @model, @ownerName  
	END   
	CLOSE car_cursor;  
	DEALLOCATE car_cursor;  

GO

-- Takes custom type, returns table
create procedure CreateCarsAndReturn (
	@cars CompanyCarType READONLY)
as
	--worst proc ever lol
	DECLARE @make NVARCHAR(256), @model NVARCHAR(256), @ownerName NVARCHAR(256), @ownerId bigint;  
	DECLARE @startCarId bigint = (select max(id) from Cars)

	DECLARE car_cursor CURSOR FOR   
	SELECT Make, Model, OwnerName
	FROM @cars;  
  
	OPEN car_cursor  
  
	FETCH NEXT FROM car_cursor   
	INTO @make, @model, @ownerName 
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  

		set @ownerId = (select Id from Owner where FirstName = @ownerName)
		if (@ownerId is not null) begin
			insert Cars values ((select max(id)+1 from Cars), @ownerId, @make, @model)
		end

		FETCH NEXT FROM car_cursor   
		INTO @make, @model, @ownerName  
	END   
	CLOSE car_cursor;  
	DEALLOCATE car_cursor;  

	select * from Cars where Id > @startCarId
GO

-- No params, no return
create procedure Ping
as
	
GO

-- No params, one table return
create procedure GetAllCars
as

	select * from Cars

GO


-- No params, multiple table return
create procedure GetDatabase
as

	select * from Company
	select * from Owner
	select * from Cars

GO