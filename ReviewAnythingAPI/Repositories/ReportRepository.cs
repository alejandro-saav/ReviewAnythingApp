using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class ReportRepository : Repository<Report>, IReportRepository
{
    public ReportRepository(ReviewAnythingDbContext context) : base(context){}

    public async Task<IEnumerable<Report>> GetAllReportsByUserIdAsync(int userId)
    {
        return await _context.Reports.AsNoTracking().Where(r => r.ReporterUserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetAllReportsByTargetIdAsync(int targetId)
    {
        return await _context.Reports.AsNoTracking().Where(r => r.TargetId == targetId).ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetAllReportsByStatusIdAsync(int statusId)
    {
        return await _context.Reports.AsNoTracking().Where(r => r.StatusReportId == statusId).ToListAsync();
    }
}