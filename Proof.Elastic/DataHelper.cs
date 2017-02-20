using System;
using Common.DataAccess;

namespace Proof.Elastic
{
    public class DataHelper
    {
        public static void CopyData()
        {
            var remote = new MatchingRequestRepository();

            var pages = remote.GetAll();

            int i = 0;

            foreach (var page in pages)
            {
                BulkInsertHelper.BulkSave(page, "Server=localhost;Database=TestDB;Trusted_Connection=True;", "[dbo].[MatchingRequest]");
                Console.WriteLine(i++);
            }
        } 
    }
}