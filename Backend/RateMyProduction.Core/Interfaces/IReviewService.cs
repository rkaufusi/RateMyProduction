using System;
using System.Collections.Generic;
using System.Text;
using RateMyProduction.Core.Entities;
using RateMyProduction.Core.DTOs.Responses;
using RateMyProduction.Core.DTOs.Requests;

namespace RateMyProduction.Core.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(int userId, int productionId, CreateReviewRequest request);
        Task<ReviewDto?> UpdateReviewAsync(int userId, int reviewId, UpdateReviewRequest request);
        Task DeleteReviewAsync(int userId, int reviewId);
        Task<PagedResult<ReviewDto>> GetReviewsForProductionAsync(int productionId, ReviewQueryParameters parameters);
        Task<bool> UserHasReviewedAsync(int userId, int productionId);
        Task<ReviewDto?> GetByIdAsync(int reviewId, int? currentUserId = null);
    }

}
