using ReviewAnythingAPI.DTOs.LogDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface ILogService
{
    Task<RequestLog> InsertNewVisitLogAsync(LogInsertRequestDto logInsert);
    Task<RequestLog> GetVisitLogByIdAsync(int logId);
    Task<IEnumerable<RequestLog>> GetAllLogsAsync();
}