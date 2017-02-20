using Akka.Actor;
using AkkaPOF.Messages;
using System;
using System.Collections.Generic;
using Akka.Persistence;
using AkkaPOF.Commands;
using AkkaPOF.Events;

namespace AkkaPOF.Actors
{    
    public class ReportStatusActor : ReceivePersistentActor
    {
        private readonly Dictionary<Guid, RequestStatusUpdated> requestCache = new Dictionary<Guid, RequestStatusUpdated>();

        public override string PersistenceId => "ReportStatusActor";

        public ReportStatusActor()
        {
            Command<GetRequestStatus>(m => GetRequestStatus(m));
            Command<RequestStatusInfo>(m => ReceiveRequestStatus(m));

            Recover<RequestStatusUpdated>(m => ReplayStatusInfoEvent(m));
        }

        private void GetRequestStatus(GetRequestStatus message)
        {
            RequestStatusUpdated requestStatusInfo;

            if (requestCache.TryGetValue(message.RequestUid, out requestStatusInfo))
            {
                this.Sender.Tell(requestStatusInfo);
            }

            this.Sender.Tell(new RequestStatusInfo(message.RequestUid, RequestStatus.NotFound));
        }

        private void ReceiveRequestStatus(RequestStatusInfo message)
        {
            Console.WriteLine($"Status kua: ReportStatusActor.ReceiveRequestStatus: {message}");

            var @event = new RequestStatusUpdated(message.RequestUid, message.RequestStatus, message.ReportId);

            Persist(@event, m =>
            {
                Console.WriteLine("Status actor persisting");

                ReplayStatusInfoEvent(m);
            });
        }

        private void ReplayStatusInfoEvent(RequestStatusUpdated @event)
        {
            if (requestCache.ContainsKey(@event.RequestUid))
            {
                requestCache[@event.RequestUid] = @event;
            }
            else
            {
                requestCache.Add(@event.RequestUid, @event);
            }
        }
    }
}