using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Proof.Elastic
{
    public class MatchingRequestRepository : IMatchingRequestRepository
    {
        public string ConnectionString { get; } =
            // @"Data Source=bench2\SQL2014;Initial Catalog=LatviaMatching;User ID=lsd;Password=heroin";
            "Server=localhost;Database=TestDB;Trusted_Connection=True;";

        public MatchingRequestRepository()
        {
            
        }

        public MatchingRequestRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public IEnumerable<List<MatchingRequest>> GetAll()
        {
            var page = 0;
            var data = GetAll(page);

            while (data.Any())
            {
                yield return data;
                data = GetAll(++page);
            }
        }

        public bool Add(MatchingRequest matchingRequest)
        {
            try
            {
                string sql = @"
                INSERT INTO [dbo].[MatchingRequest]
                           ([CustomerCode]
                           ,[SubjectType]
                           ,[FullName]
                           ,[Gender]
                           ,[DateOfBirth]
                           ,[CountryOfBirth]
                           ,[CreditinfoId]
                           ,[TaxNumber]
                           ,[NationalId]
                           ,[PassportNumber]
                           ,[PassportIssuerCountry]
                           ,[IdCardNumber]
                           ,[DrivingLicenseNumber]
                           ,[VotersId]
                           ,[ForeignUniqueId]
                           ,[CustomIdNumber1]
                           ,[CustomIdNumber2]
                           ,[BusinessLicense]
                           ,[CompanyName]
                           ,[TradeName]
                           ,[LegalForm]
                           ,[EstablishmentDate]
                           ,[RegistrationNumber]
                           ,[Inserted]
                           ,[SocialSecurityNumber]
                           ,[PreviousPassport]
                           ,[BankingID]
                           ,[ArtificialID]
                           ,[CustomIdNumber3]
                           ,[NationalIDIssuerCountry]
                           ,[IDCardIssuerCountry]
                           ,[DrivingLicenseIssuerCountry]
                           ,[CustomIdNumber1IssuerCountry]
                           ,[TaxNumberIssuerCountry]
                           ,[BusinessLicenseIssuerCountry]
                           ,[RegistrationNumberIssuerCountry]
                           ,[AssociationIdNumber]
                           ,[OtherIdNumber])
                     VALUES
                           (@CustomerCode 
                           ,@SubjectType 
                           ,@FullName
                           ,@Gender
                           ,@DateOfBirth 
                           ,@CountryOfBirth 
                           ,@CreditinfoId 
                           ,@TaxNumber 
                           ,@NationalId 
                           ,@PassportNumber 
                           ,@PassportIssuerCountry 
                           ,@IdCardNumber
                           ,@DrivingLicenseNumber 
                           ,@VotersId 
                           ,@ForeignUniqueId 
                           ,@CustomIdNumber1 
                           ,@CustomIdNumber2 
                           ,@BusinessLicense 
                           ,@CompanyName
                           ,@TradeName
                           ,@LegalForm 
                           ,@EstablishmentDate
                           ,@RegistrationNumber
                           ,@Inserted 
                           ,@SocialSecurityNumber 
                           ,@PreviousPassport 
                           ,@BankingID 
                           ,@ArtificialID 
                           ,@CustomIdNumber3 
                           ,@NationalIDIssuerCountry 
                           ,@IDCardIssuerCountry 
                           ,@DrivingLicenseIssuerCountry 
                           ,@CustomIdNumber1IssuerCountry 
                           ,@TaxNumberIssuerCountry 
                           ,@BusinessLicenseIssuerCountry 
                           ,@RegistrationNumberIssuerCountry
                           ,@AssociationIdNumber 
                           ,@OtherIdNumber);

                            SELECT CAST(SCOPE_IDENTITY() as INT)";

                using (IDbConnection db = new SqlConnection(ConnectionString))

                {
                    var id = db.Query<int>(sql, matchingRequest);
                }

                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }

        private List<MatchingRequest> GetAll(int page)

        {
            int pageSize = 10000;
            Console.WriteLine($"Reading page {page}");
            using (IDbConnection db = new SqlConnection(ConnectionString))

            {
                return db.Query<MatchingRequest>($"Select * From MatchingRequest order by MatchingRequestId OFFSET {page * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY").ToList();
            }
        }
    }
}