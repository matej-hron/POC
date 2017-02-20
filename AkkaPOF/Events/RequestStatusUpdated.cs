using System;
using AkkaPOF.Messages;

namespace AkkaPOF.Events
{
    public class RequestStatusUpdated
    {
        public Guid RequestUid { get; private set; }
        public RequestStatus RequestStatus { get; private set; }
        public long? ReportId { get; private set; }

        public RequestStatusUpdated(Guid requestUid, RequestStatus requestStatus, long? reportId)
        {
            RequestUid = requestUid;
            RequestStatus = requestStatus;
            ReportId = reportId;
        }

    }
}