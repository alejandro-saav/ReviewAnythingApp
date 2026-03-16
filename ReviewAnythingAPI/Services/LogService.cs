
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.LogDTOs;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class LogService : ILogService
{
    private readonly IRepository<RequestLog> _repository;
    private readonly ReviewAnythingDbContext _dbContext;

    public LogService(IRepository<RequestLog> repository, ReviewAnythingDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<RequestLog> InsertNewVisitLogAsync(LogInsertRequestDto logRequest)
    {
        RequestLog newLog = new RequestLog
        {
            IpAddress = logRequest.IpAddress,
            AcceptLanguage = logRequest.AcceptLanguage,
            CreatedAt = logRequest.CreatedAt,
            UserAgent = logRequest.UserAgent
        };
        await _repository.AddAsync(newLog);
        await _dbContext.SaveChangesAsync();
        return newLog;
    }

    public async Task<RequestLog> GetVisitLogByIdAsync(int logId)
    {
        var log = await _repository.GetByIdAsync(logId);
        if (log is null)
        {
            throw new EntityNotFoundException($"Log not found by the given logId: {logId}");
        }
        return log;
    }

    public async Task<IEnumerable<RequestLog>> GetAllLogsAsync()
    {
        var logs = await _repository.GetAllAsync();
        return logs;
    }
}