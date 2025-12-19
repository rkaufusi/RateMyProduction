using RateMyProduction.Core.Entities;
using RateMyProduction.Core.Interfaces;
using RateMyProduction.Core.DTOs.Responses;
using RateMyProduction.Core.DTOs.Requests;

namespace RateMyProduction.Core.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepo;
        private readonly IRepository<Production> _prodRepo;
        private readonly IRepository<User> _userRepo;

        public ReviewService(
            IRepository<Review> reviewRepo,
            IRepository<Production> prodRepo,
            IRepository<User> userRepo)
        {
            _reviewRepo = reviewRepo;
            _prodRepo = prodRepo;
            _userRepo = userRepo;
        }

        public async Task<ReviewDto> CreateReviewAsync(int userId, int productionId, CreateReviewRequest request)
        {
            if (await UserHasReviewedAsync(userId, productionId))
                throw new InvalidOperationException("You have already reviewed this production.");

            var production = await _prodRepo.GetByIdAsync(productionId)
                ?? throw new KeyNotFoundException($"Production {productionId} not found");

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found");

            var review = new Review
            {
                ProductionID = productionId,
                UserID = userId,
                RatingOverall = request.RatingOverall,
                RoleWorked = request.RoleWorked,
                ReviewText = request.ReviewText.Trim(),
                IsAnonymous = request.IsAnonymous,
                DatePosted = DateTime.UtcNow
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();

            return ToDto(review, production, user, userId);
        }

        public async Task<ReviewDto?> UpdateReviewAsync(int userId, int reviewId, UpdateReviewRequest request)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId)
                ?? throw new KeyNotFoundException($"Review {reviewId} not found");

            if (review.UserID != userId)
                throw new UnauthorizedAccessException("You can only edit your own reviews.");

            var production = await _prodRepo.GetByIdAsync(review.ProductionID)
                ?? throw new KeyNotFoundException($"Production {review.ProductionID} not found");

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found");

            review.RatingOverall = request.RatingOverall;
            review.RoleWorked = request.RoleWorked;
            review.ReviewText = request.ReviewText.Trim();
            review.IsAnonymous = request.IsAnonymous;

            await _reviewRepo.SaveChangesAsync();

            return ToDto(review, production, user, userId);
        }

        public async Task DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId)
                ?? throw new KeyNotFoundException($"Review {reviewId} not found");

            if (review.UserID != userId)
                throw new UnauthorizedAccessException("You can only delete your own reviews.");

            _reviewRepo.Delete(review);
            await _reviewRepo.SaveChangesAsync();
        }

        public async Task<PagedResult<ReviewDto>> GetReviewsForProductionAsync(
            int productionId,
            ReviewQueryParameters parameters)
        {
            // 1. Make sure the production actually exists
            var production = await _prodRepo.GetByIdAsync(productionId)
                ?? throw new KeyNotFoundException($"Production {productionId} not found");

            // 2. Load ALL reviews for this production (your repo only has ListAllAsync + AnyAsync)
            var allReviewsForThisProduction = (await _reviewRepo.ListAllAsync())
                .Where(r => r.ProductionID == productionId)
                .OrderByDescending(r => r.DatePosted)
                .ToList();

            var totalCount = allReviewsForThisProduction.Count;

            // 3. Apply paging
            var pageReviews = allReviewsForThisProduction
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToList();

            // 4. Load only the users we actually need (bulk lookup)
            var userIdsNeeded = pageReviews.Select(r => r.UserID).Distinct().Distinct().ToList();
            var allUsers = await _userRepo.ListAllAsync();
            var userMap = allUsers
                .Where(u => userIdsNeeded.Contains(u.UserID))
                .ToDictionary(u => u.UserID);

            // 5. Map to DTOs — now matches the updated ToDto signature
            var dtos = pageReviews.Select(r => ToDto(
                review: r,
                production: production,
                user: userMap.GetValueOrDefault(r.UserID),
                currentUserId: parameters.CurrentUserId ?? 0
            )).ToList();

            // 6. Return paged result
            return new PagedResult<ReviewDto>(dtos, parameters.Page, parameters.PageSize)
            {
                TotalCount = totalCount
            };
        }

        public async Task<bool> UserHasReviewedAsync(int userId, int productionId)
            => await _reviewRepo.AnyAsync(r => r.UserID == userId && r.ProductionID == productionId);

        private static ReviewDto ToDto(Review review, Production production, User? user, int currentUserId) => new(
            ReviewID: review.ReviewID,
            ProductionID: review.ProductionID,
            ProductionTitle: production.Title,
            RatingOverall: review.RatingOverall,
            RoleWorked: review.RoleWorked,
            ReviewText: review.ReviewText,
            DisplayName: review.IsAnonymous ? "Anonymous" : (user?.DisplayName ?? "User"),
            DatePosted: review.DatePosted,
            IsAnonymous: review.IsAnonymous,
            IsOwnReview: review.UserID == currentUserId
        );

        public async Task<ReviewDto?> GetByIdAsync(int reviewId, int? currentUserId = null)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId);
            if (review == null)
                return null;

            var production = await _prodRepo.GetByIdAsync(review.ProductionID);
            if (production == null)
                return null;

            var user = await _userRepo.GetByIdAsync(review.UserID);

            return ToDto(review, production, user, currentUserId ?? 0);
        }
    }
}