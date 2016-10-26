using Lilybot.Commute.Domain;

namespace Lilybot.Commute.Application
{
    public interface IFamilyProfileRepository
    {
        FamilyProfile GetByFacebookUserId(string facebookUserId);
    }
}