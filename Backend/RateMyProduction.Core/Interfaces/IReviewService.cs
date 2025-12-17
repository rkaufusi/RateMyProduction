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
    }

    //// TODO: abstract to seperate file
    //public record CreateReviewRequest(
    //    byte RatingOverall,
    //    string? RoleWorked,
    //    string ReviewText,
    //    bool IsAnonymous = false);

    //public record UpdateReviewRequest(
    //    byte RatingOverall,
    //    string? RoleWorked,
    //    string ReviewText,
    //    bool IsAnonymous);

    //public record ReviewQueryParameters(
    //    int Page = 1,
    //    int PageSize = 20,
    //    int? CurrentUserId = null);

    //public record ReviewDto(
    //    int ReviewID,
    //    int ProductionID,
    //    string ProductionTitle,
    //    byte RatingOverall,
    //    string? RoleWorked,
    //    string ReviewText,
    //    string DisplayName,
    //    DateTime DatePosted,
    //    bool IsAnonymous,
    //    bool IsOwnReview);
}
