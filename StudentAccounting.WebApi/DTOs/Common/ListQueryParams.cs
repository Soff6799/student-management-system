using System.ComponentModel.DataAnnotations;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.Common;

/// <summary>
/// Базовые параметры запроса списков: поиск, сортировка, пагинация
/// </summary>
public class ListQueryParams
{
    /// <summary>
    /// Полнотекстовый поиск по ключевым полям раздела
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Колонка сортировки (набор значений свой у каждого раздела)
    /// </summary>
    public string SortBy { get; set; } = "createdAt";

    public bool Descending { get; set; }

    /// <summary>
    /// Номер страницы, начиная с 1
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Размер страницы
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Страница списка с метаданными пагинации
/// </summary>
public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}

//параметры разделов

/// <summary>
/// Фильтры списка организаций
/// </summary>
public class OrganizationListParams : ListQueryParams
{
}

/// <summary>
/// Фильтры списка сотрудников 
/// </summary>
public class EmployeeListParams : ListQueryParams
{
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// Показать уволенных (по умолчанию скрыты)
    /// </summary>
    public bool IncludeDismissed { get; set; }

    public EducationLevel? EducationLevel { get; set; }
}

/// <summary>
/// Фильтры списка программ обучения
/// </summary>
public class TrainingProgramListParams : ListQueryParams
{
    public ProgramStatus? Status { get; set; }
}

/// <summary>
/// Фильтры списка групп обучения
/// </summary>
public class TrainingGroupListParams : ListQueryParams
{
    public Guid? TrainingProgramId { get; set; }
    public GroupStatus? Status { get; set; }
}

/// <summary>
/// Фильтры реестра уведомлений
/// </summary>
public class NotificationListParams : ListQueryParams
{
    public Guid? OrganizationId { get; set; }
    public Guid? TrainingProgramId { get; set; }
}

/// <summary>
/// Фильтры списка обучающихся в группе
/// </summary>
public class GroupStudentListParams : ListQueryParams
{
    public TrainingStatus? Status { get; set; }
}