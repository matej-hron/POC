using System;
using System.Collections.Generic;

namespace Proof.Elastic
{
    public class MatchingRequestIndividual
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public DateTime? DateOfBirth { get; set; }

        public List<Identification> Identifications { get; set; }

        public MatchingRequestIndividual()
        {
            Identifications = new List<Identification>();
        }
    }
}