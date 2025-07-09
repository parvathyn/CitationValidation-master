 --use pems_us_pro;      
 --go      
 --sp_helptext Space_Plate_status_Zone_v5_1_1_IPS      
       
--       [Space_Plate_status_Zone_v5_1_1_test]      7029 ,'24ELYB'  
--		 [Space_Plate_status_Zone_v5_1_1_IPS]      7062 ,'Q57GJ' 
--       [Space_Plate_status_Zone_v5_1_1_IPS]  7010 , 'CWF5746'      
--       [Space_Plate_status_Zone_v5_1_1_IPS]  7010 , 'CWF5746'      
    
--[Space_Plate_status_Zone_v5_1_1]  7062 , 'CWG2519'        
;                      
--CREATE   or alter   PROCEDURE [dbo].[Space_Plate_status_Zone_v5_1_1_IPS]      
--(@customerID INT, @PlateNumber VARCHAR(25))       
--AS       
SET FMTONLY OFF;       
SET NOCOUNT ON;       
      
  Declare @customerID     INT = 7010 ,                                          
@PlateNumber    VARCHAR(25) = 'CWF5746'                                              
                                                    
DECLARE @PresentMeterTime DATETIME, @GracePeriodMinute INT,@PresentMeterTime1 DATETIME,@ENFSource varchar(250),                                                                                                                     
                                                                            
@DateTimeFilter DATETIME, @StartDateTime DATETIME,@Count int,@Count1 int;       
      
SET @DateTimeFilter = DATEADD(dd, -1, GETDATE());
drop table if exists #CustomerData;
CREATE TABLE #CustomerData (CustomerId INT, PlateNumber   VARCHAR(25));

drop table if exists #SpaceTxnStaging;
CREATE TABLE #SpaceTxnStaging      
(ID INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,       
EnfCustomerId INT,      
EnfVEndorID   INT,       
PurchasedDate DATETIME,       
ExpiryDate    DATETIME,       
PlateNumber   VARCHAR(25),       
RateNumber Varchar(50), Amount decimal, ValidAnyZone bit,       
LPLocation varchar(15),       
Zoneid int,       
ZoneName varchar(30),       
Vendorname varchar(30), meterid varchar(30), LE varchar(50),       
DeltaTime datetime,       
ENFCreatedDateTime datetime       
);       
      
      ------TO GET THE PRESENTMETER TIME-----------------------      
      
SELECT @PresentMeterTime = DATEADD(mi, LocalTimeUTCDifference, GETDATE()),       
@PresentMeterTime1= DATEADD(mi, LocalTimeUTCDifference, GETDATE()+1)       
FROM Timezones T WITH (Nolock)       
INNER JOIN customers C WITH (NOLOCK) ON T.TimezoneID = c.Timezoneid       
WHERE C.customerid = @customerID;       
      
--------------GETTING THE DATA FROM TABLES AND INSERTING INTO TEMP TABLE------------       
--DEFAULT VALUES SHOULD APPEAR , EVEN WHEN ALL OTHER TABLES ARE BLANK  :- ENFPERMITS , PAYBYCELL,ENFVENDORTXN, LPREVENTS                                                          
INSERT INTO #CustomerData (CustomerId,PlateNumber) select @CustomerId , @PlateNumber;       
--TXN TABLES                                                                        
                                                                   
INSERT INTO #SpaceTxnStaging      
(EnfCustomerId, EnfVEndorID, PurchasedDate, ExpiryDate, PlateNumber,      
RateNumber,Amount, ValidAnyZone, LPLocation,Zoneid , ZoneName, Vendorname, meterid)       
SELECT p.CustomerId,VendorId,       
TransDateTime = isnull(p.TransDateTime, '1900/01/01'),       
ExpiryDateTime = isnull(p.ExpiryDateTime, '1900/01/01'),       
PlateNumber=V.LPNumber,TxnSeqNum as RateNumber,P.Amount,       
(case when p.CustomerId =7056 and P.Amount=500   and Z.[Name] like '99%' then 1  else NULL end) as ValidAnyZone,       
--and P.ZoneId like '99%' then 1       
v.LPLocation, Z.ID as ZoneID, Z.[Name] as ZoneName,       
case when @customerID=4140 then  'Passport'  else Null end as Vendorname,       
str(p.MeterId)      
FROM      
PayByCellPlateTxn  P WITH (NOLOCK)      
inner join Parkvehicle V  with(nolock)  on p.VehicleID=V.VehicleID      
--left join EnfVendorStall as e with(nolock) on p.CustomerId=e.EnfCustomerId and str(p.ZoneId) = e.MeterID       
--NEW LINE:       
Left join EnfZone as z with(nolock) on  p.CustomerId=z.EnfCustomerId and p.zoneid =z.name         
--and CustomerId = @customerID        
where CustomerId = @customerID and v.LPNumber=@PlateNumber        
and TransDateTime >= dateadd(hour,-48,@PresentMeterTime)      
      
INSERT INTO #SpaceTxnStaging                                                                   
                                                      
(EnfCustomerId,  EnfVEndorID, PurchasedDate,  ExpiryDate, PlateNumber, RateNumber, Amount,ValidAnyZone, Zoneid  , ZoneName, Vendorname, MeterID )                                                                          
                                                                    
SELECT p.EnfCustomerId, p.EnfVendorId, TransDateTime = isnull(p.PurchasedDate, '1900/01/01')        
, ExpiryDateTime = isnull(p.ExpiryDate, '1900/01/01'), p.PlateNumber, '' as RateNumber, p.ChargedAmount,         
(case when p.EnfCustomerId =7056 and p.ChargedAmount=5 and  P.ArticleName like '99%' then 1    else NULL end) as ValidAnyZone ,                                                                             
--and e.Zoneid like '99%' then 1                                                                            
e.zoneid,                                                                                
                                                                             
(case when  p.EnfCustomerId IN(7056,4337) then  P.ArticleName else Z.Name end) as ZoneName,                                     
case when @customerID=4140 then  'Parkeon'  else Null end as Vendorname,e.MeterID        
FROM                                         
                                        
EnfVendorTransaction  P WITH (NOLOCK)        
left join EnfVendorStall as e with(nolock) on p.EnfCustomerId=e.EnfCustomerId and e.EnfVendorId=p.EnfVendorId and p.PayStationId=e.PayStationId        
Left join EnfZone as z with(nolock) on e.EnfCustomerId=z.EnfCustomerId and e.EnfVendorId=z.EnfVendorId and e.Zoneid=z.ID        
where p.EnfCustomerId = @customerID and p.PlateNumber=@PlateNumber        
and p.PurchasedDate >= dateadd(hour,-48,@PresentMeterTime)        
--TESTING                                           
                     
	--				 select @PlateNumber;
 --select * from #SpaceTxnStaging                    
--------------JOINING THE SPACE DETAILS WITH THE TEMP TABLE AND OTHER PARAMETER TABLES TO GET THE DETAILED INFO------                                       
;With cte0 as (                                  
                                  
select  EnfCustomerID,  PlateNumber , HitID, EnfVendorID,  DeltaTime as DeltaTimeGlendale                                  
FROM  dbo.EnfVendorLPREvent LP WITH(NOLOCK)                                   
Where LP.HitType=0 and LP.EnfCustomerID = 4142 and  LP.PlateNumber = @PlateNumber                                  
                                  
), cte   AS                                
(                                                                        
SELECT Customerid =cd.CustomerId, PlateNumber = cd.PlateNumber, PresentMeterTime = @PresentMeterTime,       
SensorEventTime = isnull(tx.PurchasedDate, @DateTimeFilter), tx.PurchasedDate,       
--ExpiryTime = isnull(isnull( tx.ExpiryDate , EP.ENFCreatedDateTime ) , @DateTimeFilter) ,       
-- ExpiryTime = isnull( tx.ExpiryDate , EP.ENFCreatedDateTime ) ,       
 tx.ExpiryDate as ExpiryTime , ExpiredMinutes = isnull(DATEDIFF(mi, @PresentMeterTime, tx.ExpiryDate), -100),       
 tx.ExpiryDate, tx.RateNumber, EP.EnfType, EP.EnfSource, tx.LPLocation as ENFState,  ep.ENFPlateNo,  tx.amount,       
 tx.ValidAnyZone,                                                                            
--isnull(tx.Zoneid, EP.Zoneid) as Zoneid,       
 tx.Zoneid,       
 tx.ZoneName as ZoneName,  tx.Vendorname ,tx.meterid ,       
                                                                        
--LP.AlertStatus,       
EP.ENFPermitNo, isnull (EP.ENFCreatedDateTime, getutcdate()) as ENFCreatedDateTime  ,       
-- LP.HotlistCategory as LE,                                        
--(case when isnull(LP.AlertStatus , LP.HotlistCategory) is null then GenetecUserAction end) as LE,       
COALESCE ( LP.AlertStatus , (case when ltrim(rtrim(LP.HotlistCategory)) ='' then NULL else LP.HotlistCategory end) , (case when cd.customerid IN(7056,7064,4176) then h.HitDescription else null end)) as LE,       
(case when LP.DeltaTime is not null then                     
convert(varchar,cast(@PresentMeterTime  as date)) +  ' '  + format(dateadd(ss,cast((case when isnumeric(LP.DeltaTime)=1 then LP.DeltaTime else NULL end) as int) ,0),'HH:mm:ss')                                    
else                                  
convert(varchar,cast(@PresentMeterTime  as date)) +  ' '  + format(dateadd(ss,cast((case when isnumeric(GLP.DeltaTimeGlendale)=1 then GLP.DeltaTimeGlendale else NULL end) as int) ,0),'HH:mm:ss')                                   
end) DeltaTime                                  
--(case when cd.CustomerID=4142 then cte0.DeltaTimeGlendale else LP.DeltaTime end) as DeltaTime                               
, LP.EnfVendorID as VendorID , LP.PlateImage as ImageRawData , LP.ID as EnfVendorLPRId, LP.EnforceDatetime , LP.LPReadDatetime , LP.HitType , LP.ReadType       
FROM                                                                                                                     
#CustomerData cd WITH(NOLOCK)                                                                         
left join  #SpaceTxnStaging tx WITH(NOLOCK) ON cd.PlateNumber = tx.PlateNumber and cd.customerid=tx.enfcustomerid                                                                                                       
left join dbo.Enf_permits EP WITH(NOLOCK) ON cd.PlateNumber = EP.enfplateno and cd.customerid=ep.enfcustomerid  --and ep.ENFCreatedDateTime>=@DateTimeFilter       
left join dbo.EnfVendorLPREvent LP WITH(NOLOCK) on cd.customerid=LP.EnfCustomerID  and   cd.PlateNumber = LP.PlateNumber                            
--and isnull(LP.LPReadDatetime,@PresentMeterTime) >= dateAdd(Hour,-16,@PresentMeterTime)                           
and isnull(LP.EnforceDatetime,@PresentMeterTime)  >= dateAdd(Minute,-29060,@PresentMeterTime)     --->= dateAdd(Second,-57600,@PresentMeterTime)                        
left join cte0 GLP WITH(NOLOCK) on cd.customerid=GLP.EnfCustomerID  and   cd.PlateNumber = GLP.PlateNumber and LP.EnfVendorID=GLP.EnfVendorID and LP.HitID =GLP.HitID                                   
--left join #tempcte0 as cte0 WITH(NOLOCK) on cte0.EnfCustomerID=cd.customerid  and cte0.PlateNumber = cd.PlateNumber                                        
left join hitmaster h with(nolock) on h.hittype = LP.HitType                                        
                                                            
WHERE cd.customerid = @customerid and  cd.PlateNumber = @PlateNumber)       
                                                                            
SELECT top 1 CustomerId, PlateNumber, PresentMeterTime,       
  case when @customerid in(4120,7034,8016,7056,7030,4135,4147,7014,7036,7040,7046,7055,7061,7029,7064)      
then       
(CASE  when EnfType in(select ScofflawDescription from SCOFFLAWMASTER with(nolock)) then 'Violation'       
when EnfType in('Permit','Exempt','EXEMPT' , 'PREVEXPR' ,'platelist') then 'Permit'       
WHEN ExpiredMinutes <= -1  THEN 'Expired'      
WHEN ExpiredMinutes >= 0   then 'Paid'      
else  EnfType                                      
END)      
WHEN @customerid IN(4337)               
  THEN (              
    CASE WHEN enfsource IN('Scofflaw') THEN 'Violation'               
 WHEN enfsource IN('PlatePermit') THEN 'Permit'               
 WHEN expiredminutes <= -1 THEN 'Expired'               
 WHEN expiredminutes >= 0 THEN 'Paid' ELSE enftype END              
  )                                                                    
else (CASE when EnfType in(select ScofflawDescription from SCOFFLAWMASTER with(nolock)) then 'Violation'                      
when EnfType in('Permit','Exempt','EXEMPT' , 'PREVEXPR','platelist') then 'Permit'        
WHEN ExpiredMinutes <= -1 THEN 'Expired'       
WHEN ExpiredMinutes >= 0   then 'Paid'       
END) END SpaceStatus,      
(CASE      
 when EnfSource in('Exempt','MiamiDadeAPI','Permit','PlatePermit','platelist') or EnfSource like'%Permit%' or EnfSource like'%Exempt%'  then   isnull(EnfType  , EnfSource)      
 when (select top 1 EnfSource from Enf_Permits where EnfCustomerID=@CustomerID and enfplateno=@PlateNumber and  (EnfSource like '%Permit%'  or EnfSource like '%platelist%' )       
 order by ENFCreatedDateTime desc) like '%Permit%' then 'Permit'       
 else NULL END)   as AC1,       
 (case      
  WHEN @PresentMeterTime >=ExpiryTime  and ExpiryTime >=dateadd(hour,-4,@PresentMeterTime) THEN 'Expired'       
  WHEN ExpiryTime >= @PresentMeterTime and ExpiryTime >=dateadd(hour,-4,@PresentMeterTime) THEN 'Paid'      
  else NULL       
  end) as AC2,       
        
  (case when EnfSource in('Scofflaw') or enfType in('Scofflaw') then enfType       
  when (select top 1 EnfSource from Enf_Permits                                     
  where EnfCustomerID=@CustomerID and enfplateno=@PlateNumber and  EnfSource='Scofflaw')       
  ='Scofflaw' then (select top 1 EnfType from Enf_Permits where EnfCustomerID=@CustomerID and enfplateno=@PlateNumber and  EnfSource='Scofflaw')      
 else   NULL       
end ) as AC3,       
(case when EnfSource in('Stolen') or enfType in('Stolen')  then  EnfType  end ) as AC4,                          
--AlertStatus ,                                                        
(case when ExpiryTime >= dateadd(hour,-4,@PresentMeterTime) then ExpiryTime else NULL end) as ExpiryTime,       
EnfType, ENFState,RateNumber, Amount, ValidAnyZone,      
(case when @customerID=7006 then meterid else Zoneid end) as Zoneid,      
(case when @customerID=7006  then cast(meterid as varchar) else ZoneName end) as Zonename,      
Vendorname,      
(select top 1 ENFPermitNo from Enf_Permits where EnfCustomerID=@CustomerID and enfplateno=@PlateNumber and      
 (EnfSource like '%Permit%' or EnfSource like '%platelist%')                                            
                                                    
order by ENFCreatedDateTime desc) as ENFPermitNo,                                                
                                                
LE,                                               
              
cast (DeltaTime as datetime) as  DeltaTime   -- This line had error  "Conversion failed for Date/Time" 29/10/2020            
      
--(case when DeltaTime is null or DeltaTime='NULL' then NULL else  cast (DeltaTime as datetime)  end) as  DeltaTime                                            
,VendorID                                     
,ImageRawData                                    
,EnfVendorLPRId                                    
,(case  when VendorID=5 and HitType=2 then LPReadDatetime  else EnforceDatetime end) as ObservedTime       
FROM cte       
order by cte.ExpiryTime desc , ENFCreatedDateTime desc ,       
EnforceDatetime desc , ReadType asc       
      
----------Additional Permit details------------------                                                      
                                                      
exec spPlatePermit @customerID, @PlateNumber      
      
DROP TABLE #CustomerData      
DROP TABLE #SpaceTxnStaging 