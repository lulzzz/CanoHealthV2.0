USE [CanoHealth]
GO
/****** Object:  StoredProcedure [dbo].[SearchByCorporationInsuranceLocationSql]    Script Date: 7/29/2018 9:08:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SearchByCorporationInsuranceLocationSql]
	-- Add the parameters for the stored procedure here
	@corporationId UNIQUEIDENTIFIER,
	@insuranceId UNIQUEIDENTIFIER,	
	@placeOfServiceId UNIQUEIDENTIFIER	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT			 corp.CorporationId
					,corp.CorporationName
					,ins.InsuranceId
					,ins.Name as InsuranceName
					,cont.GroupNumber as [Contract]
					,docinfo.Degree
					,docinfo.FirstName + ' ' + docinfo.LastName as [FullName]
					,docinfo.DateOfBirth
					,docinfo.NpiNumber
					,docinfo.CaqhNumber
					,pnbi.ProviderNumber as [ProviderNumberByInsurance]
					,lb.Code + ' ' + '-' + ' ' + lb.Name as [LineOfBusiness]
					,linked.EffectiveDate
					,linked.Note
					,pnbl.LocacionProviderNumber as [ProviderNumberByLocation]
	
	FROM			Corporations corp

	INNER JOIN		Contracts cont
		ON corp.CorporationId = cont.CorporationId

	INNER JOIN		Insurances ins 
		ON cont.InsuranceId = ins.InsuranceId

	INNER JOIN		ContractLineofBusinesses clb			--contract line of business
		ON cont.ContractId = clb.ContractId

	INNER JOIN		PlanTypes lb							-- line of business
		ON clb.PlanTypeId = lb.PlanTypeId

	INNER JOIN		ClinicLineofBusinessContracts clbc	--location contract line of business
		ON clb.ContractLineofBusinessId = clbc.ContractLineofBusinessId

	INNER JOIN		PlaceOfServices loc
		ON clbc.PlaceOfServiceId = loc.PlaceOfServiceId

	LEFT JOIN		DoctorCorporationContractLinks linked	-- doctor linked contract
		ON clb.ContractLineofBusinessId = linked.ContractLineofBusinessId

	--to get provider number by location and line of business
	INNER JOIN ProviderByLocations pnbl
		ON linked.DoctorCorporationContractLinkId = pnbl.DoctorCorporationContractLinkId 
		AND pnbl.PlaceOfServiceId = @placeOfServiceId


	INNER JOIN (SELECT doc.* 
				FROM Doctors doc 
				INNER JOIN DoctorClinics docc 
					ON doc.DoctorId = docc.DoctorId 
				INNER JOIN PlaceOfServices loc 
					ON docc.PlaceOfServiceId = loc.PlaceOfServiceId 
				WHERE loc.PlaceOfServiceId = @placeOfServiceId 
					AND docc.Active = 1) docinfo
		ON linked.DoctorId = docinfo.DoctorId
	
	--to get the individual provider by insurance too
	LEFT JOIN DoctorIndividualProviders pnbi
		ON docinfo.DoctorId = pnbi.DoctorId AND pnbi.InsuranceId = @insuranceId

	WHERE corp.CorporationId = @corporationId --'F7BB11F3-E35C-4117-B413-FD0306E373B8'	
		AND ins.InsuranceId = @insuranceId --'543BEA59-68D1-4BDF-864B-1E324E3DC2C3'	
		AND clbc.PlaceOfServiceId = @placeOfServiceId --'A04F07FF-916C-45DA-8D5A-64ACD742A7BC'
		AND corp.Active = 1
		AND ins.Active = 1
		AND cont.Active = 1
		AND docinfo.Active = 1
		AND loc.Active = 1
END