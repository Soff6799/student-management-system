using Microsoft.EntityFrameworkCore;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.Notifications;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationDto>> GetAsync(string sortBy = "nextDate", bool descending = false)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var trainings = await _context.StudentTrainings
            .Include(t => t.Employee).ThenInclude(e => e.Organization)
            .Include(t => t.TrainingGroup).ThenInclude(g => g.TrainingProgram)
            .Where(t => t.Status == TrainingStatus.Completed
                        && t.NextTrainingDate != null
                        && t.Employee.Status == EmployeeStatus.Active)
            .ToListAsync();

        var notifications = trainings
            .Select(t => new
            {
                Training = t,
                Days = t.NextTrainingDate!.Value.DayNumber - today.DayNumber,
                Lead = t.TrainingGroup.TrainingProgram?.NotificationLeadTimeDays ?? 60
            })
            .Where(x => x.Days <= x.Lead)
            .Select(x => new NotificationDto
            {
                StudentTrainingId = x.Training.Id,
                EmployeeId = x.Training.EmployeeId,
                EmployeeFullName = $"{x.Training.Employee.LastName} {x.Training.Employee.FirstName} {x.Training.Employee.Patronymic}".Trim(),
                OrganizationName = x.Training.Employee.Organization?.ShortName,
                ProgramName = x.Training.TrainingGroup.TrainingProgram?.Name ?? string.Empty,
                LastCompletionDate = x.Training.CompletionDate,
                NextTrainingDate = x.Training.NextTrainingDate!.Value,
                DaysRemaining = x.Days,
                Color = GetColor(x.Days)
            })
            .ToList();

        return Sort(notifications, sortBy, descending);
    }

    /// <summary>
    /// Цветовая индикация по количеству дней до срока
    /// </summary>
    private static NotificationColor GetColor(int daysRemaining)
    {
        return daysRemaining switch
        {
            < 60 => NotificationColor.Red,
            <= 120 => NotificationColor.Yellow,
            _ => NotificationColor.Green
        };
    }

    /// <summary>
    /// Сортировка реестра
    /// </summary>
    private static IEnumerable<NotificationDto> Sort(List<NotificationDto> list, string sortBy, bool descending)
    {
        Func<NotificationDto, object> key = sortBy.ToLower() switch
        {
            "employee" => n => n.EmployeeFullName,
            "organization" => n => n.OrganizationName ?? string.Empty,
            "program" => n => n.ProgramName,
            _ => n => n.NextTrainingDate
        };
        return descending ? list.OrderByDescending(key) : list.OrderBy(key);
    }
}