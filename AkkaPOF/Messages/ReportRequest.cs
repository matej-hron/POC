using System;

namespace AkkaPOF.Messages
{
    public class ReportRequest
    {
        public Guid RequestUid { get; private set; }
        public int UserId { get; private set; }
        public DateTime SnapshotDate { get; private set; } 
        public int SubscriberId { get; private set; }
        public long CreditinfoId { get; private set; }

        public SubjectType SubjectType { get; private set; }

        public string ReportType { get; private set; }

        public ReportRequest()
        {
            RequestUid = Guid.NewGuid();
        }

        public ReportRequest(int subscriberId)
        {
            this.SubscriberId = subscriberId;
        }
    }
}