using System.ComponentModel.DataAnnotations.Schema;
using Lilybot.Core.Domain.Model;

namespace Lilybot.Commute.Domain
{
    [Table("Profiles")]
    public class CommuteProfile : AggregateRoot
    {
        internal CommuteProfile() : base() { }

        public CommuteProfile(string username) : base(username) { }
    }
}
