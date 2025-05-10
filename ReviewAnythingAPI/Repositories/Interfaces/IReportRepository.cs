using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IReportRepository : IRepository<Report>
{
    public Task<IEnumerable<Report>> GetAllReportsByUserIdAsync(int userId);
    
    public Task<IEnumerable<Report>> GetAllReportsByTargetIdAsync(int targetId);

    public Task<IEnumerable<Report>> GetAllReportsByStatusIdAsync(int statusId);
}