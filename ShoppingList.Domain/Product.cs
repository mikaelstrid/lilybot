using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class Product : AggregateRoot
    {
        internal Product() { }

        public Product(string username) : base(username) { }
        
        public string Name { get; set; }
    }
}
