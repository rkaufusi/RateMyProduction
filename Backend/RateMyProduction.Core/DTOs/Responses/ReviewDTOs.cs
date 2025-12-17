using System;
using System.Collections.Generic;
using System.Text;

namespace RateMyProduction.Core.DTOs.Responses
{
    public record ReviewDto(
        int ReviewID,
        int ProductionID,
        string ProductionTitle,
        byte RatingOverall,
        string? RoleWorked,
        string ReviewText,
        string DisplayName,
        DateTime DatePosted,
        bool IsAnonymous,
        bool IsOwnReview);
}
