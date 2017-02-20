using System.Collections.Generic;

namespace Proof.Elastic
{
    public interface IMatchingRequestRepository
    {
        IEnumerable<List<MatchingRequest>> GetAll();
        bool Add(MatchingRequest matchingRequest);
    }
}
