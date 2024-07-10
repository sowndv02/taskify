using taskify_api.Models;

namespace taskify_api.Repository.IRepository
{
    public interface IMeetingUserRepository : IRepository<MeetingUser>
    {
        Task<MeetingUser> UpdateAsync(MeetingUser entity);
    }
}
