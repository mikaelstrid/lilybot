using System.Collections.Generic;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public interface IFamilyRepository
    {
        Family GetByFacebookUserId(string facebookUserId);
    }

    public class FamilyRepository : IFamilyRepository
    {
        public Family GetByFacebookUserId(string facebookUserId)
        {
            return new Family
            {
                GrownUps = new List<User>()
            };
        }
    }

    public class Family
    {
        public List<User> GrownUps { get; set; }
    }

    public class User
    {
        public string FacebookUserId { get; set; }
    }
}