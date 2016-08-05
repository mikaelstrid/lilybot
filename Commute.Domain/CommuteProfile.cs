using System.ComponentModel.DataAnnotations.Schema;
using Lilybot.Core.Domain.Model;

namespace Lilybot.Commute.Domain
{
    [Table("Profiles")]
    public class CommuteProfile : AggregateRoot
    {
        internal CommuteProfile() : base() { }

        public CommuteProfile(string username) : base(username) { }

        public double HomeLocationLatitude { get; set; }

        public double HomeLocationLongitude { get; set; }

        public double WorkLocationLatitude { get; set; }

        public double WorkLocationLongitude { get; set; }

        public string HomePublicTransportStationId { get; set; }

        public string WorkPublicTransportStationId { get; set; }

        public string PrimaryWayOfCommuting { get; set; }
    }
}
