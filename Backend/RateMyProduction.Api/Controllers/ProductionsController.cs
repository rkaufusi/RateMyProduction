// Api/Controllers/ProductionsController.cs
using Microsoft.AspNetCore.Mvc;
using RateMyProduction.Core.Interfaces;
using RateMyProduction.Core.DTOs.Responses;

namespace RateMyProduction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionsController : ControllerBase
{
    private readonly IProductionService _productionService;

    public ProductionsController(IProductionService productionService)
    {
        _productionService = productionService;
    }

    // GET: api/productions
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductionDTOs>>> GetAll()
        => Ok(await _productionService.GetAllAsync());

    // GET: api/productions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductionDTOs>> GetById(int id)
    {
        var production = await _productionService.GetByIdAsync(id);
        return production is null ? NotFound() : Ok(production);
    }

    // GET: api/productions/top
    [HttpGet("top")]
    public async Task<ActionResult<IReadOnlyList<ProductionDTOs>>> GetTopRated([FromQuery] int count = 10)
        => Ok(await _productionService.GetTopRatedAsync(count));

    // GET: api/productions/paged
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<ProductionDTOs>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => Ok(await _productionService.GetPagedAsync(page, pageSize));

}