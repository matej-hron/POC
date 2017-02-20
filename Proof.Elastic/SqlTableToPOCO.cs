using System;
using Nest;
using Newtonsoft.Json;

namespace Proof.Elastic
{
    [ElasticsearchType()]
    public class MatchingRequest
    {
                
        public long MatchingRequestId { get; set; }

        [Nest.String(Store = false, Index = FieldIndexOption.No)]
        public string Hash { get; set; }

        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string CustomerCode { get; set; }
        public int SubjectType { get; set; }
        public string FullName { get; set; }
        public int Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int CountryOfBirth { get; set; }
        public long? CreditinfoId { get; set; }

        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string TaxNumber { get; set; }

        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string NationalId { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string PassportNumber { get; set; }
        public int PassportIssuerCountry { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string IdCardNumber { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string DrivingLicenseNumber { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string VotersId { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string ForeignUniqueId { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string CustomIdNumber1 { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string CustomIdNumber2 { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string BusinessLicense { get; set; }
        public string CompanyName { get; set; }
        public string TradeName { get; set; }
        public int LegalForm { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string RegistrationNumber { get; set; }
        public DateTime Inserted { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string SocialSecurityNumber { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string PreviousPassport { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string BankingID { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string ArtificialID { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string CustomIdNumber3 { get; set; }
        public int NationalIDIssuerCountry { get; set; }
        public int IDCardIssuerCountry { get; set; }
        public int DrivingLicenseIssuerCountry { get; set; }
        public int CustomIdNumber1IssuerCountry { get; set; }
        public int TaxNumberIssuerCountry { get; set; }
        public int BusinessLicenseIssuerCountry { get; set; }
        public int RegistrationNumberIssuerCountry { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string AssociationIdNumber { get; set; }
        [Nest.String(Store = true, Index = FieldIndexOption.NotAnalyzed)]
        public string OtherIdNumber { get; set; }
    }
}