using System.ComponentModel.DataAnnotations.Schema;
using Lilybot.Core.Domain.Model;

namespace Lilybot.Shopping.Domain
{
    [Table("Profiles")]
    public class ShoppingProfile : AggregateRoot
    {
        internal ShoppingProfile() : base() { }

        public ShoppingProfile(string username) : base(username) { }

        public string Friends { get; set; }
    }
}
