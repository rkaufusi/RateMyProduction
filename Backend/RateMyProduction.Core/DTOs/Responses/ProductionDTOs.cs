namespace RateMyProduction.Core.DTOs.Responses
{
    public record ProductionDTOs(
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
}
