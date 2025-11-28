using RateMyProduction.Core.Entities;
using RateMyProduction.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateMyProduction.Core.Services
{
    public class ProductionService : IProductionService
    {
        private readonly IRepository<Production> _repository;

        public ProductionService(IRepository<Production> repository)
        {
            _repository = repository;
        }

        public async Task<ProductionDto?> GetByIdAsync(int id)
        {
            var production = await _repository.GetByIdAsync(id);
            return production is null ? null : ToDto(production);
        }

        public async Task<IReadOnlyList<ProductionDto>> GetAllAsync()
        {
            var productions = await _repository.ListAllAsync();
            return productions.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<ProductionDto>> GetTopRatedAsync(int count = 10)
        {
            var all = await _repository.ListAllAsync();

            return all
                .Where(p => p.AverageRating.HasValue && p.ReviewCount > 0)
                .OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.ReviewCount)
                .Take(count)
                .Select(ToDto)
                .ToList();
        }

        public async Task<PagedResult<ProductionDto>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var items = await _repository.ListAllAsync();

            var result = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return new PagedResult<ProductionDto>(result, page, pageSize);
        }

        private static ProductionDto ToDto(Production p) => new(
            p.ProductionID,
            p.Title,
            p.ProductionType,
            p.Studio,
            p.Director,
            p.YearReleased,
            p.AverageRating,
            p.ReviewCount,
            p.Synopsis,
            p.CreatedDate
        );
    }
}
