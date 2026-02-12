// Api/Controllers/ProductionsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RateMyProduction.Core.DTOs.Responses;
using RateMyProduction.Core.Interfaces;

namespace RateMyProduction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ProductionsController : ControllerBase
{
    private readonly IProductionService _productionService;

    public ProductionsController(IProductionService productionService)
    {
        _productionService = productionService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductionDTOs>>> GetAll()
        => Ok(await _productionService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductionDTOs>> GetById(int id)
    {
        var production = await _productionService.GetByIdAsync(id);
        return production is null ? NotFound() : Ok(production);
    }

    [HttpGet("top")]
    public async Task<ActionResult<IReadOnlyList<ProductionDTOs>>> GetTopRated([FromQuery] int count = 10)
        => Ok(await _productionService.GetTopRatedAsync(count));

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<ProductionDTOs>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => Ok(await _productionService.GetPagedAsync(page, pageSize));

}