using System;
using System.Collections.Generic;
using System.Text;

namespace RateMyProduction.Core.Interfaces
{
    public interface IProductionService
    {
        Task<ProductionDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<ProductionDto>> GetAllAsync();
        Task<IReadOnlyList<ProductionDto>> GetTopRatedAsync(int count = 10);
        Task<PagedResult<ProductionDto>> GetPagedAsync(int page = 1, int pageSize = 20);
    }

    // TODO: abstract to seperate file
    public record ProductionDto(
        int ProductionID,
        string Title,
        string ProductionType,
        string? Studio,
        string? Director,
        int? YearReleased,
        decimal? AverageRating,
        int ReviewCount,
        string? Synopsis,
        DateTime CreatedDate
    );

    public record PagedResult<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize
    );
}
