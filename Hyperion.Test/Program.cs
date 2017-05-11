using System;
using CI.CBS.Contracts.Internal.Structures.Reports;
using CI.CBS.Framework.Base.Configuration.Sections;
using CI.CBS.Framework.Base.DataAccess;
using CI.CBS.Framework.Common.Configuration;
using CI.CBS.Framework.Common.Logging;
using CI.CBS.Framework.Common.Time;
using CI.CBS.InternalModules.Reports.DataAccess;
using CI.CBS.InternalModules.Reports.Serialization;
using Moq;

namespace Hyperion.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost;Database=LatviaTestUpload;Trusted_Connection=True;";

            var configurationProvider = Mock.Of<IConfigurationProvider>(
                cf => cf.GetConfigurationSection<DataAccessConfiguration>() == new DataAccessConfiguration
                {
                    ConnectionStringUpload = connectionString
                });

            var logger = Mock.Of<ILogger>();
            var timeProvider = Mock.Of<ITimeProvider>(t => t.Now == DateTime.Now);

            ReportStorage rs = new ReportStorage(
                new SqlGenericDataAccessFactory<IReportUnitOfWork>(
                    configurationProvider, logger, () => new SqlReportUnitOfWork(configurationProvider, timeProvider)), new XmlReportSerializer(new DummyPerformanceLoggerProvider()));


            var response = rs.GetReport<CreditinfoReportPlus>(new Guid("A0209CE2-F7BE-42F0-9284-5CF525347991"));
        }
    }
}
