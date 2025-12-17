using System;
using System.Collections.Generic;
using System.Text;
using RateMyProduction.Core.DTOs.Responses;

namespace RateMyProduction.Core.Interfaces
{
    public interface IProductionService
    {
        Task<ProductionDTOs?> GetByIdAsync(int id);
        Task<IReadOnlyList<ProductionDTOs>> GetAllAsync();
        Task<IReadOnlyList<ProductionDTOs>> GetTopRatedAsync(int count = 10);
        Task<PagedResult<ProductionDTOs>> GetPagedAsync(int page = 1, int pageSize = 20);
    }

    // TODO: abstract to seperate file
    //public record ProductionDto(
    //    int ProductionID,
    //    string Title,
    //    string ProductionType,
    //    string? Studio,
    //    string? Director,
    //    int? YearReleased,
    //    decimal? AverageRating,
    //    int ReviewCount,
    //    string? Synopsis,
    //    DateTime CreatedDate
    //);

    public record PagedResult<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize)
    {
        public int TotalCount { get; init; }
        public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
