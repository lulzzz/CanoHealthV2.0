USE [CanoHealth]
GO
/****** Object:  StoredProcedure [dbo].[SearchBycorporationInsuranceDoctorSql]    Script Date: 7/29/2018 11:46:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SearchBycorporationInsuranceDoctorSql] 
	-- Add the parameters for the stored procedure here
	@corporationId UNIQUEIDENTIFIER,
	@insuranceId UNIQUEIDENTIFIER,	
	@doctorId UNIQUEIDENTIFIER	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	    -- Insert statements for procedure here

	SELECT				corp.CorporationId
						,corp.CorporationName
						,ins.InsuranceId
						,ins.[Name] as InsuranceName
						,cont.GroupNumber as [Contract]
						,lb.PlanTypeId
						,lb.Code + ' ' + '-' + ' ' + lb.[Name] as [LineOfBusiness]
						,clb.EffectiveDate
						,clb.Note
						,loc.PlaceOfServiceId
						,loc.[Name] as [Location]
						,loc.PhoneNumber
						,loc.FaxNumber
						,loc.[Address]
						,pnbi.ProviderNumber as [ProviderNumberByInsurance]
						,pnbl.LocacionProviderNumber as [ProviderNumberByLocation]

	FROM				Corporations corp

	INNER JOIN			Contracts cont
		ON corp.CorporationId = cont.CorporationId

	INNER JOIN			Insurances ins
		ON cont.InsuranceId = ins.InsuranceId

	--to get the individual provider by insurance too
	LEFT JOIN DoctorIndividualProviders pnbi
		ON pnbi.InsuranceId = ins.InsuranceId AND pnbi.DoctorId  = @doctorId

	INNER JOIN (	SELECT		 a.*
								,b.DoctorCorporationContractLinkId
								,b.EffectiveDate
								,b.Note 
					FROM ContractLineofBusinesses a  --get the line of business where doctor is linked inicialmente solo estaba a.*

					INNER JOIN DoctorCorporationContractLinks b
						on a.ContractLineofBusinessId = b.ContractLineofBusinessId
					WHERE b.DoctorId = @doctorId
				) clb 
	
		ON cont.ContractId = clb.ContractId

	INNER JOIN			PlanTypes lb
		ON clb.PlanTypeId = lb.PlanTypeId	

	INNER JOIN			ClinicLineofBusinessContracts clbc
		ON clb.ContractLineofBusinessId = clbc.ContractLineofBusinessId

	INNER JOIN			PlaceOfServices loc
		ON clbc.PlaceOfServiceId = loc.PlaceOfServiceId

	INNER JOIN			DoctorClinics dc
		ON loc.PlaceOfServiceId = dc.PlaceOfServiceId	

	LEFT JOIN ProviderByLocations pnbl
		ON dc.PlaceOfServiceId = pnbl.PlaceOfServiceId AND pnbl.DoctorCorporationContractLinkId = clb.DoctorCorporationContractLinkId

	WHERE corp.CorporationId = @corporationId
		AND ins.InsuranceId = @insuranceId
		AND dc.DoctorId = @doctorId
		AND dc.Active = 1
		AND corp.Active = 1
		AND ins.Active = 1
		AND cont.Active = 1
		AND loc.Active = 1
	

ORDER BY loc.PlaceOfServiceId
END
