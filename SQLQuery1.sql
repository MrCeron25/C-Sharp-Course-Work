/*
use [master];
IF EXISTS (SELECT * FROM SYS.DATABASES WHERE NAME='course_work')
BEGIN
	DROP DATABASE course_work;
END
CREATE DATABASE course_work;
*/

use course_work;
GO
IF EXISTS (SELECT * FROM sysobjects WHERE name = 'GetRandTime' AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
   DROP FUNCTION GetRandTime;
END;
GO

GO
CREATE FUNCTION GetRandTime(
	@rand float,
	@startTime time = '00:30:00',
	@endTime time = '02:00:00'
) RETURNS time
AS
BEGIN
	-- Get the number of seconds between these two times
	-- (eg. there are 5400 seconds between 1 AM and 2.30 AM)
	DECLARE @maxSeconds int = DATEDIFF(ss, @startTime, @endTime);
	-- Get a random number of seconds between 0 and the number of 
	-- seconds between @startTime and @endTime (@maxSeconds)
	DECLARE @randomSeconds int = (@maxSeconds + 1) * @rand; 
	-- Add the random number of seconds to @startTime and return that random time of day
	return convert(Time, DateAdd(second, @randomSeconds, @startTime));
END;
GO

drop table if exists tickets;
drop table if exists passengers;
drop table if exists flights;
drop table if exists archive_flights;
drop table if exists airplane;
drop table if exists cities;
drop table if exists country;
drop table if exists [system];

drop table if exists [system];
create table [system](
	[user_id] bigint identity primary key,
	[login] nvarchar(255) not null,
	[password] nvarchar(255) not null,
	[is_admin] bit not null,
	[id_passenger] bigint unique
);

drop table if exists country;
create table country(
	id bigint identity primary key,
	[name] nvarchar(255) not null unique,
);

drop table if exists cities;
create table cities(
	id bigint identity primary key,
	[name] nvarchar(255) not null unique,
	[country_id] bigint,
	FOREIGN KEY ([country_id]) REFERENCES country (id)
);

CREATE INDEX idx_name ON cities ([name]);--для быстрого поиска

drop table if exists airplane;
create table airplane(
	id bigint identity primary key,
	[model] nvarchar(255) not null unique,
	number_of_seats int not null,
);

-- рейсы
drop table if exists flights;
create table flights(
	id bigint identity primary key,
	[flight_name] nvarchar(10) not null,
	[departure_city] bigint,
	[arrival_city] bigint,
	airplane_id bigint not null,
	[departure_date] datetime not null, --default cast(cast(RAND()*100000 as int) as datetime),
		--DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 3650), '2000-01-01'), --DATEADD(day, (ABS(CHECKSUM(NEWID())) % 65530), 0), --DATEADD(DAY,ABS(CHECKSUM(NEWID())) % ( 1 + DATEDIFF(DAY,CURRENT_TIMESTAMP,DATEADD(year,1,CURRENT_TIMESTAMP))),CURRENT_TIMESTAMP), -- для рандомных данных
	[travel_time] time not null,
	[arrival_date] AS DATEADD(MINUTE, DATEDIFF(MINUTE, 0, [travel_time]), [departure_date]),
	[price] float not null,
	FOREIGN KEY (airplane_id) REFERENCES airplane (id),
	FOREIGN KEY ([departure_city]) REFERENCES cities (id),
	FOREIGN KEY ([arrival_city]) REFERENCES cities (id)
);

-- архив
drop table if exists archive_flights;
create table archive_flights(
	id bigint identity primary key,
	[flight_name] nvarchar(10) not null,
	[departure_city] bigint,
	[arrival_city] bigint,
	airplane_id bigint not null,
	[departure_date] datetime not null, 
	[travel_time] time not null,
	[arrival_date] AS DATEADD(MINUTE, DATEDIFF(MINUTE, 0, [travel_time]), [departure_date]),
	[price] float not null,
	FOREIGN KEY (airplane_id) REFERENCES airplane (id),
	FOREIGN KEY ([departure_city]) REFERENCES cities (id),
	FOREIGN KEY ([arrival_city]) REFERENCES cities (id)
);

/*
select 
	convert(datetime,'2022-06-16 01:10:28',20),
	convert(date,'2000-02-14'),
	ABS(SUM(DATEDIFF(SECOND, convert(datetime,'2022-06-16 01:10:28',20), convert(date,'2000-02-14')))),
	dateadd(second, SUM(DATEDIFF(SECOND, convert(datetime,'2022-06-16 01:10:28',20), convert(date,'2000-02-14'))), 0),
	DATEADD(ms, 121.25 * 1000, 0);


select convert(datetime,'2022-06-16 01:10:28',20) [date],
	   convert(time,'00:30:00') [time],
	   LTRIM(DATEDIFF(MINUTE, 0, convert(time,'00:30:00'))) [min],
	   DATEADD(MINUTE, DATEDIFF(MINUTE, 0, convert(time,'00:30:00')),  convert(datetime,'2022-06-16 01:10:28',20)) [date + time];


select dateadd(SS,DATEDIFF(second, 600, convert(datetime,'2022-06-16 01:10:28',20)),GETDATE()),
	   DATEDIFF(SS, GETDATE(), GETDATE()) [sec];

select DATEDIFF(SS, CONVERT(date, GETDATE()), GETDATE()),
	   convert(datetime,DATEDIFF(SS, CONVERT(date, GETDATE()), GETDATE())),cast(cast(RAND()*100000 as int) as datetime);
*/


drop table if exists passengers;
create table passengers(
	id bigint identity primary key,
	[name] nvarchar(255) not null,
	[surname] nvarchar(255) not null,
	sex char(1),
	date_of_birth date not null default getdate(),
	passport_series int not null,
	passport_id int not null,
	FOREIGN KEY (id) REFERENCES [system] ([id_passenger])
);

drop table if exists tickets;
create table tickets(
	id bigint identity primary key,
	[flight_id] bigint not null,
	seat_number bigint not null,
	id_passenger bigint not null,
	FOREIGN KEY ([flight_id]) REFERENCES flights (id),
	FOREIGN KEY (id_passenger) REFERENCES passengers (id)
);

/*
-- Триггер для уникальности каждого билета на рейс
GO
CREATE TRIGGER tickets_check ON tickets
AFTER INSERT, UPDATE
AS 
BEGIN
	--select * from inserted;
	
	if ((select count(*) from tickets ti where ti.seat_number = (select seat_number from INSERTED)) > 0) BEGIN
		RAISERROR ('Билет уже занят.', 16, 1);  
	END;
	
END;
GO
*/

insert into [system]([login],[password],[is_admin],id_passenger) values
('user1', 'XKDBTUH5ERCVDBH', 1, 1),
('user2', 'ZRCK91MIKZJFL3X', 1, 2),
('user3', '2DI804OKODJB3LI', 1, 3),
('user4', '1', 1, 4),
('user5', '1', 0, 5),
('user6', 'G2OYD411QO77I02', 0, 6),
('user7', 'CB9B09ZKWLWKOH7', 0, 7),
('user8', 'VV576US7ZYLN8C3', 0, 8),
('user9', '6C4HRWLEG2E178P', 0, 9),
('user10', 'HTOSVWZGTWNYBZO', 0, 10),
('user11', '68U736DS360HQG5', 0, 11),
('user12', '07TJT3F4P15OYY6', 0, 12),
('user13', 'FA3TVS5YE6CAOII', 0, 13),
('user14', 'VBHH3T2SU172HPM', 0, 14),
('user15', '0CO9AM77J6U9A2H', 0, 15),
('user16', 'UWAROVJDC79258H', 0, 16),
('user17', 'FL4VF73B2DPOFNF', 0, 17),
('user18', 'TQIKROUMJLDGPLK', 0, 18),
('user19', '8NGJUNL5Z6F6B8W', 0, 19),
('user20', 'W4HMZN2DQ1N3GMQ', 0, 20);

insert into passengers([name],[surname],sex,date_of_birth,passport_id,passport_series) values
('Ярослав', 'Фуфаев', 'м', '1990-03-01', 2265, 665038),
('Акимов', 'Аким', 'м', '1960-02-17', 6186, 788845),
('Пронин', 'Василий', 'м', '1975-07-20', 1489, 686287),
('Петров', 'Пётр', 'м', '1982-01-02', 3325, 281050),
('Петренко', 'Пётр', 'м', '1982-01-02', 1602, 943241),
('Петренко', 'Иван', 'м', '1982-01-02', 7857, 617316),
('Дубова', 'Галина', 'ж', '1960-02-14', 4240, 831619),
('Бабичев', 'Евгений', 'м', '1960-06-12', 2576, 742885),
('Нагаев', 'Дмитрий', 'м', '1985-09-20', 5113, 787175),
('Семеновна', 'Ольга', 'ж', '1960-06-12', 7083, 202561),
('Сидоров', 'Павел', 'м', '1960-02-07', 3656, 540713),
('Овдиенко', 'Евгений', 'м', '1980-02-02', 6267, 875685),
('Кузнецов', 'Василий', 'м', '1950-01-01', 2487, 309681),
('Ильенко', 'Александр', 'м', '1982-01-02', 6078, 772914),
('Ильенко', 'Дмитрий', 'м', '1982-01-02', 1219, 943560),
('Никитка', 'Мышанская', 'ж', '2020-03-01', 6650, 971222),
('Иван', 'Курилин', 'м', '1945-01-02', 3254, 864699),
('Никитка', 'Мышанская', 'ж', '2020-03-01', 6333, 921761),
('Мария', 'Игнатьевна', 'ж', '2003-03-06', 4991, 968461),
('Иван', 'Курилин', 'м', '1945-01-02', 4555, 870041);

insert into country([name]) values
('Россия'),
('Китай'),
('Япония'),
('США'),
('Германия');

insert into cities([name],country_id) values
('Москва',1),
('Калининград',1),
('Пекин',2),
('Тяньцзинь',2),
('Айсай',3),
('Самара',1),
('Нью-Йорк',4),
('Берлин',5),
('Дайсен',3),
('Чунцин',2),
('Мюнхен',5),
('Шанхай',2),
('Нижний Новгород',1),
('Гаосюн',2),
('Тайбэй',2),
('Асахи',3),
('Астрахань',1),
('Детройт',4),
('Чикаго',4),
('Гамбург',5),
('Санкт-Петербург',1);

insert into airplane([model], number_of_seats) values
('Boeing 717', 124),
('Boeing 737',162),
('Boeing 747-400',660),
('Boeing 757-200',200),
('Boeing 757-300',300),
('Boeing-777-300',450),
('Boeing-777-200',300),
('Airbus A310',220),
('Airbus A350-900',310),
('Airbus A350-1000',350);


--Declare @rand float = RAND(convert(varbinary, newId()));
--select @rand;
--select dbo.GetRandTime(@rand,default,default);

--select convert(datetime,'2023-10-11 20:33:27.226432',120);

insert into flights([departure_city], [arrival_city], airplane_id, flight_name, travel_time,price,departure_date) values
(1, 6, 1,'SU 1602',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),70000,convert(datetime,'2023-03-05 15:04:30',20)),
(1, 6, 3,'SU 1602',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),65000,convert(datetime,'2024-01-09 07:34:29',20)),
(1, 21, 1,'SU 6016',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),13000,convert(datetime,'2023-10-26 05:35:33',120)),
(4, 10, 1,'SU 8043',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),100000,convert(datetime,'2024-01-22 06:52:49',120)),
(2, 20, 2,'SU 9997',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),80000,convert(datetime,'2022-06-25 02:27:56',120)),
(1, 19, 4,'SU 6205',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),90000,convert(datetime,'2024-08-13 22:43:46',120)),
(7, 20, 3,'SU 5813',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),76000,convert(datetime,'2025-01-08 12:08:42',120)),
(6, 17, 5,'SU 6859',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),56000,convert(datetime,'2022-08-04 01:37:21',120)),
(2, 5, 6,'SU 9642',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),45000,convert(datetime,'2023-10-23 04:23:49',120)),
(16, 4, 7,'SU 6252',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),32000,convert(datetime,'2022-05-09 00:01:16',120)),
(1, 20, 9,'SU 8999',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),77000,convert(datetime,'2023-01-22 04:36:17',120)),
(17, 9, 10,'SU 3146',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),66000,convert(datetime,'2023-12-07 04:24:49',120)),
(14, 3, 8,'SU 1357',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),84000,convert(datetime,'2022-11-08 14:14:22',120)),
(9, 5, 2,'SU 6498',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),5000,convert(datetime,'2022-01-01 14:14:22',120)),
(6, 4, 7,'SU 1931',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),2000,convert(datetime,'2022-01-01 14:14:22',120)),
(7, 8, 3,'SU 6943',dbo.GetRandTime(RAND(convert(varbinary, newId())),default,default),66000,convert(datetime,'2022-05-07 15:00:00',120));

insert into tickets(flight_id, seat_number,id_passenger) values
(1,1, 10),
(1,14, 20),
(1,15, 6),
(1,20, 4),
(1,7, 1),
(1,18, 13),
(1,16, 16),
(1,19, 6),
(1,17, 12),
(1,6, 19),
(2,9, 12),
(2,16, 7),
(2,11, 13),
(2,6, 18),
(2,18, 10),
(2,17, 17),
(2,20, 12),
(2,3, 5),
(2,19, 6),
(2,13, 5),
(3,1, 9),
(3,2, 14),
(3,10, 7),
(3,17, 19),
(3,8, 18),
(3,12, 17),
(3,20, 4),
(3,13, 18),
(3,3, 18),
(3,4, 15),
(4,19, 18),
(4,14, 1),
(4,1, 13),
(4,12, 8),
(4,3, 1),
(4,16, 10),
(4,15, 14),
(4,7, 2),
(4,9, 15),
(4,18, 7),
(5,19, 10),
(5,18, 7),
(5,2, 16),
(5,7, 1),
(5,6, 20),
(5,11, 15),
(5,12, 3),
(5,20, 10),
(5,1, 13),
(5,4, 14),
(6,12, 4),
(6,15, 14),
(6,13, 13),
(6,11, 16),
(6,14, 11),
(6,3, 16),
(6,5, 1),
(6,4, 16),
(6,10, 18),
(6,18, 18),
(7,9, 6),
(7,17, 15),
(7,8, 6),
(7,4, 4),
(7,13, 3),
(7,7, 18),
(7,20, 13),
(7,10, 14),
(7,2, 6),
(7,15, 7),
(8,2, 6),
(8,5, 13),
(8,18, 8),
(8,16, 16),
(8,3, 4),
(8,19, 18),
(8,10, 3),
(8,1, 8),
(8,6, 3),
(8,7, 3),
(9,11, 18),
(9,3, 6),
(9,18, 5),
(9,15, 7),
(9,12, 17),
(9,16, 9),
(9,13, 19),
(9,14, 9),
(9,9, 10),
(9,17, 12),
(10,11, 7),
(10,8, 14),
(10,15, 3),
(10,12, 15),
(10,7, 9),
(10,2, 6),
(10,5, 20),
(10,20, 8),
(10,3, 2),
(10,13, 3),
(14,11, 7),
(14,8, 14),
(14,15, 3)
--(15,11, 7)
;

--------------
-- PROC 1
USE course_work;

GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'UserRegistration' AND type = 'P')
BEGIN
   DROP PROCEDURE UserRegistration;
END;
GO

GO
CREATE PROCEDURE UserRegistration(
	@name nvarchar(255),
	@surname nvarchar(255),
	@sex nvarchar(1),
	@date_of_birth date,
	@passport_id int,
	@passport_series int,
	@login nvarchar(255),
	@password nvarchar(255)
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;
		declare @id_passenger int;
		set @id_passenger = (SELECT IDENT_CURRENT('[dbo].[passengers]')) + 1;

		insert into [system]([login],[password],[is_admin],id_passenger)
		values (@login, @password, 0, @id_passenger);

		insert into passengers([name],[surname],sex,date_of_birth,passport_id,passport_series)
		values (@name,@surname, @sex, @date_of_birth, @passport_id, @passport_series);

        COMMIT;
		SELECT 1,
			   'Completed successfully!' [Completed successfully];
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_LINE() ErrorLine,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO
---
GO
IF EXISTS (SELECT * FROM sysobjects WHERE name = 'get_list_of_flights_with_dep__date' AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
   DROP FUNCTION get_list_of_flights_with_dep__date;
END;
GO

GO
CREATE FUNCTION get_list_of_flights_with_dep__date(
	@from_city nvarchar(255),
	@to_city nvarchar(255),
	@start_date datetime
) RETURNS TABLE 
AS
RETURN
	select 
		fl.id,
		format(fl.departure_date,'dd.MM.yyyy HH:mm') [Время отправления],
		format(fl.arrival_date,'dd.MM.yyyy HH:mm') [Время прибытия],
		fl.flight_name [Рейс],
		format(convert(datetime,fl.travel_time,8),'HH:mm') [Время в пути],
		fl.price [Цена],
		(select name from cities where id = fl.departure_city) AS [Город вылета],
		(select name from cities where id = fl.arrival_city) AS [Город прибытия],
		air.number_of_seats - (select count(*) from tickets ti where ti.flight_id = fl.id) [свободных мест]
	from flights fl
	join airplane air on 
		air.id = fl.airplane_id
	where 
		lower((select name from cities where id = fl.departure_city)) like lower(concat('%',@from_city,'%')) and 
		lower((select name from cities where id = fl.arrival_city)) like lower(concat('%',@to_city,'%')) AND
		(fl.departure_date >= @start_date) AND 
		(air.number_of_seats - (select count(*) from tickets ti where ti.flight_id = fl.id)) > 0;
GO

/*
	select 
		format(fl.departure_date, 'HH:mm') [Время отправления],
		format(fl.arrival_date, 'HH:mm')  [Время прибытия],
		fl.flight_name [Рейс],
		format(convert(datetime,fl.travel_time,8),'HH:mm') [Время в пути],
		fl.price [Цена],
		(select name from cities where id = fl.departure_city) AS [Город вылета],
		(select name from cities where id = fl.arrival_city) AS [Город прибытия]
	from flights fl
	where 
		lower((select name from cities where id = fl.departure_city)) like lower(concat('%',@from_city,'%')) and 
		lower((select name from cities where id = fl.arrival_city)) like lower(concat('%',@to_city,'%'))
*/

--select * from dbo.get_list_of_flights_with_dep__date('мос','с') [tbl];

----------------------

GO
IF EXISTS (SELECT * FROM sysobjects WHERE name = 'get_list_of_flights_with_dep_and_arr_date' AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
   DROP FUNCTION get_list_of_flights_with_dep_and_arr_date;
END;
GO

GO
CREATE FUNCTION get_list_of_flights_with_dep_and_arr_date(
	@from_city nvarchar(255),
	@to_city nvarchar(255),
	@start_date datetime,
	@end_date datetime
) RETURNS TABLE 
AS
RETURN
	select 
		fl.id,
		format(fl.departure_date,'dd.MM.yyyy HH:mm') [Время отправления],
		format(fl.arrival_date,'dd.MM.yyyy HH:mm') [Время прибытия],
		fl.flight_name [Рейс],
		format(convert(datetime,fl.travel_time,8),'HH:mm') [Время в пути],
		fl.price [Цена],
		(select name from cities where id = fl.departure_city) AS [Город вылета],
		(select name from cities where id = fl.arrival_city) AS [Город прибытия],
		air.number_of_seats - (select count(*) from tickets ti where ti.flight_id = fl.id) [свободных мест]
	from flights fl
	join airplane air on 
		air.id = fl.airplane_id
	where 
		lower((select name from cities where id = fl.departure_city)) like lower(concat('%',@from_city,'%')) AND 
		lower((select name from cities where id = fl.arrival_city)) like lower(concat('%',@to_city,'%')) AND
		(fl.departure_date >= @start_date) AND 
		(fl.arrival_date <= @end_date) AND 
		(air.number_of_seats - (select count(*) from tickets ti where ti.flight_id = fl.id)) > 0;
GO
---
GO
IF EXISTS (SELECT * FROM sysobjects WHERE name = 'get_occupied_seats' AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
   DROP FUNCTION get_occupied_seats;
END;
GO

GO
CREATE FUNCTION get_occupied_seats(
	@flight_id bigint
) RETURNS TABLE 
AS
RETURN
	select 
		ti.seat_number
	from flights fl
	join tickets ti on 
		ti.flight_id = fl.id
	where
		fl.id = @flight_id
GO
---
GO
IF EXISTS (SELECT * FROM sysobjects WHERE name = 'get_number_of_seats' AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
   DROP FUNCTION get_number_of_seats;
END;
GO

GO
CREATE FUNCTION get_number_of_seats(
	@flight_id bigint
) RETURNS bigint 
AS
BEGIN
	declare @res as int;
	set @res = (select 
					number_of_seats
				from flights fl
				join airplane air on 
					air.id= fl.airplane_id
				where
					fl.id = @flight_id);
	return @res;
END;
GO
---
GO
IF EXISTS (SELECT * FROM sysobjects WHERE name = 'get_user_tickets' AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
   DROP FUNCTION get_user_tickets;
END;
GO

GO
CREATE FUNCTION get_user_tickets(
	@id_passenger bigint
) RETURNS TABLE 
AS
RETURN
	select 
		ti.id [№ билета],
		fl.id [№ рейса],
		ti.seat_number [№ места],
		format(fl.departure_date,'dd.MM.yyyy HH:mm') [Время отправления],
		format(fl.arrival_date,'dd.MM.yyyy HH:mm') [Время прибытия],
		fl.flight_name [Рейс],
		format(convert(datetime,fl.travel_time,8),'HH:mm') [Время в пути],
		fl.price [Цена],
		(select name from cities where id = fl.departure_city) AS [Город вылета],
		(select name from cities where id = fl.arrival_city) AS [Город прибытия]
	from tickets ti
	join flights fl on 
		ti.flight_id = fl.id
	join airplane air on 
		air.id = fl.airplane_id
	where ti.id_passenger = @id_passenger;
GO
--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'DeleteCountry' AND type = 'P')
BEGIN
   DROP PROCEDURE DeleteCountry;
END;
GO

GO
CREATE PROCEDURE DeleteCountry(
	@CountryId bigint
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;

		update cities
		set country_id = null
		where cities.country_id = @CountryId

		delete from country 
		where id = @CountryId

        COMMIT;
		SELECT 1;
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO
--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'UpdateCountry' AND type = 'P')
BEGIN
   DROP PROCEDURE UpdateCountry;
END;
GO

GO
CREATE PROCEDURE UpdateCountry(
	@CountryName nvarchar(255),
	@NewCountryName nvarchar(255)
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;

		declare @CountryId bigint;
		set @CountryId = (select id from country 
						  where [name] = @CountryName);

		update country
        set [name] = @NewCountryName
        where id = @CountryId;

        COMMIT;
		SELECT 1;
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO
--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'DeleteCities' AND type = 'P')
BEGIN
   DROP PROCEDURE DeleteCities;
END;
GO

GO
CREATE PROCEDURE DeleteCities(
	@CityId bigint
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;

		update flights
		set departure_city = null
		where departure_city = @CityId

		update flights
		set arrival_city = null
		where arrival_city = @CityId

		delete from cities 
		where id = @CityId

        COMMIT;
		SELECT 1;
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO
--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'UpdateAirplane' AND type = 'P')
BEGIN
   DROP PROCEDURE UpdateAirplane;
END;
GO

GO
CREATE PROCEDURE UpdateAirplane(
	@AirplaneName nvarchar(255),
	@NewAirplaneName nvarchar(255)
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;
		declare @AirplaneId bigint;
		set @AirplaneId = (select id from airplane 
						  where [model] = @AirplaneName);

		update airplane
        set [model] = @NewAirplaneName
        where id = @AirplaneId;

        COMMIT;
		SELECT 1;
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO
--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'GetPassengerList' AND type = 'P')
BEGIN
   DROP PROCEDURE GetPassengerList;
END;
GO

GO
CREATE PROCEDURE GetPassengerList(
	@FlightName nvarchar(255),
	@Date DATE
) AS
BEGIN
	select 
		pas.[name] [Имя],
		pas.surname [Фамилия],
		pas.sex [Пол],
	    pas.date_of_birth [Дата рождения],
		pas.passport_id [Номер паспорта],
		pas.passport_series [Серия паспорта]
	from tickets ti
	join flights fl on fl.id = ti.flight_id
	join passengers pas on pas.id = ti.id_passenger
	where fl.flight_name = @FlightName AND
		  CONVERT(DATE, fl.departure_date) = @Date;
END;
GO

/*
select 
	pas.[name] [Имя],
	pas.surname [Фамилия],
	pas.sex [Пол],
	pas.date_of_birth [Дата рождения],
	pas.passport_id [Номер паспорта],
	pas.passport_series [Серия паспорта]
from tickets ti
join flights fl on fl.id = ti.flight_id
join passengers pas on pas.id = ti.id_passenger
where fl.flight_name = 'SU 1602' and
	  CONVERT(DATE, fl.departure_date) = CONVERT(DATE, '2023-03-05 15:04:30');
*/

/*
select * from country
select * from cities
select * from airplane
select * from flights

select ci.name [Город], co.name from cities ci
join country co on co.id = ci.country_id

select * from tickets ti
select * from flights ti

select 
	ti.flight_id [Номер рейса],
	sum(fl.price) [Сумма по всем билетам]
from tickets ti
join flights fl on fl.id = ti.flight_id
group by ti.flight_id, fl.price

select 
	year(fl.departure_date) [Год],
	DATENAME(month, fl.departure_date) [Месяц],
	sum(fl.price) [Сумма по всем билетам]
from tickets ti
join flights fl on fl.id = ti.flight_id
group by fl.departure_date
order by fl.departure_date
*/

--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'GetStatisticsOnSoldTickets' AND type = 'P')
BEGIN
   DROP PROCEDURE GetStatisticsOnSoldTickets;
END
GO

GO
CREATE PROCEDURE GetStatisticsOnSoldTickets
AS
BEGIN
	select 
		year(fl.departure_date) [Год],
		DATENAME(month, fl.departure_date) [Месяц],
		sum(fl.price) [Сумма по всем билетам]
	from tickets ti
	join flights fl on fl.id = ti.flight_id
	group by fl.departure_date
	order by fl.departure_date
END;
GO

--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'GetCountries' AND type = 'P')
BEGIN
   DROP PROCEDURE GetCountries;
END
GO

GO
CREATE PROCEDURE GetCountries
AS
BEGIN
	select 
		name
	from country
END;
GO

--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'AddCities' AND type = 'P')
BEGIN
   DROP PROCEDURE AddCities;
END;
GO

GO
CREATE PROCEDURE AddCities(
	@CityName nvarchar(255),
	@CountryName nvarchar(255)
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;

		declare @CountryId bigint;
		set @CountryId = (select id from country where name = @CountryName);

		insert into cities(name, country_id) values (@CityName, @CountryId);

        COMMIT;
		SELECT 1;
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO
--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'ChangeCities' AND type = 'P')
BEGIN
   DROP PROCEDURE ChangeCities;
END;
GO

GO
CREATE PROCEDURE ChangeCities(
	@CityName nvarchar(255),
	@NewCityName nvarchar(255),
	@CountryName nvarchar(255),
	@NewCountryName nvarchar(255)
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;

		update cities
		set name = @NewCityName
		where name = @CityName

		declare @CountryId bigint;
		set @CountryId = (select id from country 
						  where name = @NewCountryName);

		update cities 
		set country_id = @CountryId
		where name = @NewCityName;

        COMMIT;
		SELECT 1;
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO

/*
select 
	ci.id,
	ci.name,
	co.name 
from cities ci
join country co on	
	ci.country_id = co.id

select * from cities
select * from airplane
*/


--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'GetFlights' AND type = 'P')
BEGIN
   DROP PROCEDURE GetFlights;
END
GO

GO
CREATE PROCEDURE GetFlights
AS
BEGIN
	select 
		fl.flight_name [Название рейса],
		(select name from cities where id = fl.departure_city) [Город отправления], 
		(select name from cities where id = fl.arrival_city) [Город прибытия],
		fl.departure_date [Дата отправления],
		fl.travel_time [Время в пути],
		fl.arrival_date [Дата прибытия],
		fl.price [Цена]
	from flights fl
	join airplane air on air.id = fl.airplane_id
END;
GO
exec GetFlights;
--************************************************************************
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'AddFlight' AND type = 'P')
BEGIN
   DROP PROCEDURE AddFlight;
END;
GO

GO
CREATE PROCEDURE AddFlight(
	@DepartureCityName nvarchar(255),
	@ArrivalCityName nvarchar(255),
	@AirplaneName nvarchar(255),
	@FlightName nvarchar(255),
	@TraveTime time,
	@Price float,
	@DepartureDate datetime
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;

		declare @DepartureCity bigint, @ArrivalCity bigint, @Airplane bigint;

		set @DepartureCity = (select id from cities where name = @DepartureCityName);
		set @ArrivalCity = (select id from cities where name = @ArrivalCityName);
		set @Airplane = (select id from airplane where model = @AirplaneName);

		insert into flights([departure_city], [arrival_city], airplane_id, flight_name, travel_time,price,departure_date) values
		(@DepartureCity, 
		@ArrivalCity, 
		@Airplane,
		@FlightName,
		@TraveTime,
		@Price,
		@DepartureDate);

        COMMIT;
		SELECT 1;
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO

--************************************************************************
/*
GO
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'FlightsToArchive' AND type = 'P')
BEGIN
   DROP PROCEDURE FlightsToArchive;
END;
GO

GO
CREATE PROCEDURE FlightsToArchive(
	@FlightId bigint
) AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;
		


        COMMIT;
		SELECT 1;
	END TRY
    BEGIN CATCH
        ROLLBACK;
		SELECT 0,
			   ERROR_MESSAGE() ErrorMessage; 
    END CATCH;
END;
GO
*/
