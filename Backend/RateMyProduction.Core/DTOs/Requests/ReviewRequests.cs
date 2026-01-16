using System.ComponentModel.DataAnnotations;

namespace RateMyProduction.Core.DTOs.Requests
{
    public record CreateReviewRequest(
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        byte RatingOverall,

        [Required(ErrorMessage = "Role worked is required")]
        [MaxLength(100, ErrorMessage = "Role worked cannot exceed 100 characters")]
        string? RoleWorked,

        [Required(ErrorMessage = "Review text is required")]
        [MaxLength(2000, ErrorMessage = "Review text cannot exceed 2000 characters")]
        [MinLength(10, ErrorMessage = "Review text must be at least 10 characters")]
        string ReviewText,

        bool IsAnonymous = false);

    public record UpdateReviewRequest(
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        byte RatingOverall,

        [Required(ErrorMessage = "Role worked is required")]
        [MaxLength(100, ErrorMessage = "Role worked cannot exceed 100 characters")]
        string? RoleWorked,

        [Required(ErrorMessage = "Review text is required")]
        [MaxLength(2000, ErrorMessage = "Review text cannot exceed 2000 characters")]
        [MinLength(10, ErrorMessage = "Review text must be at least 10 characters")]
        string ReviewText,

        bool IsAnonymous);

    public record ReviewQueryParameters(
        [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
        int Page = 1,

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        int PageSize = 20,

        int? CurrentUserId = null);
}
