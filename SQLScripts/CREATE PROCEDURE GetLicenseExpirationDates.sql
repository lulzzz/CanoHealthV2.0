CREATE PROCEDURE GetLicenseExpirationDates

AS
DECLARE @NotificationDate DateTime;  
SET @NotificationDate = DATEADD(month, 2, GETDATE()); 
SELECT 
	'OutOfNetworkContracts' as [Source]
	,ooc.ExpirationDate
	,ooc.DoctorId
	,doc.FirstName + ' ' + doc.LastName as [DoctorFullName]
	,ooc.InsurnaceId
	,ins.Name as [InsuranceName]
	,NULL as [LicenseType]
	,NULL as [Location]
FROM [dbo].[OutOfNetworkContracts] ooc
INNER JOIN [dbo].[Doctors] doc
	on ooc.DoctorId = doc.DoctorId
INNER JOIN [dbo].[Insurances] ins
	on ooc.InsurnaceId = ins.InsuranceId
WHERE ExpirationDate < @NotificationDate

UNION ALL

SELECT 
	'DoctorMedicalLicense' as [Source]
	,ml.ExpireDate
	,ml.DoctorId
	,doc.FirstName + ' ' + doc.LastName as [DoctorFullName]
	,NULL as [InsurnaceId]
	,NULL as [InsuranceName]
	,mlt.Classification as [LicenseType]
	,NULL as [Location]
FROM MedicalLicenses ml
INNER JOIN [dbo].[Doctors] doc
	on ml.DoctorId = doc.DoctorId
INNER JOIN [dbo].[MedicalLicenseTypes] mlt
	on ml.MedicalLicenseTypeId = mlt.MedicalLicenseTypeId
WHERE ml.ExpireDate < @NotificationDate

UNION ALL

SELECT 
	'LocationLicense' as [Source]
	,pl.ExpireDate
	,NULL as [DoctorId]
	,NULL as [DoctorFullName]
	,NULL as [InsurnaceId]
	,NULL as [InsuranceName]
	,lt.LicenseName as [LicenseType]
	,pos.Name as [Location]
FROM PosLicenses pl
INNER JOIN LicenseTypes lt
	ON pl.LicenseTypeId = lt.LicenseTypeId
INNER JOIN PlaceOfServices pos
	ON pl.PlaceOfServiceId = pos.PlaceOfServiceId
WHERE pl.ExpireDate < @NotificationDate