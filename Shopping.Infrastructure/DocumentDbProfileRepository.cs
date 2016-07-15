//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Lily.Core.Infrastructure.Persistence;
//using Lily.Core.Infrastructure.Persistence.DocumetDb;
//using Lily.ShoppingList.Application;
//using Lily.ShoppingList.Domain;

//namespace Lily.ShoppingList.Infrastructure
//{
//    public class DocumentDbProfileRepository : DocumentDbAggregateRepository<Profile>, IProfileRepository
//    {
//        public DocumentDbProfileRepository(string collectionName) : base(collectionName) { }

//        public async Task<Profile> GetFriend(string username)
//        {
//            return (await Get(p => p.Friends.Contains(username))).FirstOrDefault();
//        }
//    }
//}
