using System;
using System.Collections.Generic;
using System.Text;

namespace RateMyProduction.Core.DTOs.Requests
{
    public record CreateReviewRequest(
        byte RatingOverall,
        string? RoleWorked,
        string ReviewText,
        bool IsAnonymous = false);

    public record UpdateReviewRequest(
        byte RatingOverall,
        string? RoleWorked,
        string ReviewText,
        bool IsAnonymous);

    public record ReviewQueryParameters(
        int Page = 1,
        int PageSize = 20,
        int? CurrentUserId = null);
}
