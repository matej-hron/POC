using System;
using AkkaPOF.Messages;

namespace AkkaPOF.Commands
{
    public class RequestStatusInfo
    {
        public Guid RequestUid { get; private set; }
        public RequestStatus RequestStatus { get; private set; }
        public long? ReportId { get; private set; }

        public RequestStatusInfo()
        {
            
        }

        public RequestStatusInfo(Guid requestUid, RequestStatus requestStatus, long? reportId = null)
        {
            RequestUid = requestUid;
            RequestStatus = requestStatus;
            ReportId = reportId;
        }

        public override string ToString()
        {
            return $"Request [Uid: {RequestUid}, Status: {RequestStatus}, ReportId: {ReportId}";
        }

        public RequestStatusInfo WithNewStatus(RequestStatus requestStatus)
        {
            return new RequestStatusInfo(this.RequestUid, requestStatus);
        }

        public RequestStatusInfo WithNewStatusAndReportId(RequestStatus requestStatus, long reportId)
        {
            return new RequestStatusInfo(this.RequestUid, requestStatus, reportId);
        }
    }
}
