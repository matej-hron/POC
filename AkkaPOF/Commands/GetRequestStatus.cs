using System;

namespace AkkaPOF.Commands
{
    public class GetRequestStatus
    {
        public Guid RequestUid { get; private set; }

        public static GetRequestStatus Create(Guid uid) => new GetRequestStatus {RequestUid = uid};
    }
}