using System;
using Lilybot.Commute.CommuteHomeNotifierJob.Models;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public interface IStateRepository
    {
        FamilyState GetState(Guid id);
        void SaveState(FamilyState state);
    }

    public class StateRepository : IStateRepository
    {
        public FamilyState GetState(Guid id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveState(FamilyState state)
        {
            throw new System.NotImplementedException();
        }
    }
}