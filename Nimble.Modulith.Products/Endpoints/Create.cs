using FastEndpoints;
using Nimble.Modulith.Products.Data;
using Nimble.Modulith.Products.Models;
using Nimble.Modulith.Users.Contracts;

namespace Nimble.Modulith.Products.Endpoints;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
}

public class Create(ProductsDbContext dbContext, IUserService userService) : Endpoint<CreateProductRequest, CreateProductResponse>
{
    private readonly ProductsDbContext _dbContext = dbContext;
    private readonly IUserService _userService = userService;

    public override void Configure()
    {
        Post("/products");
        Tags("products");
        Roles("Admin");
        Summary(s =>
        {
            s.Summary = "Create a new product";
            s.Description = "Creates a new product with a name and description";
        });
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var email = User.FindFirst("Email")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            ThrowError("User email not found in token.");
        }

        var userDetails = await _userService.GetUserAsync(email, ct);

        if (userDetails == null)
        {
            ThrowError("User does not exist in the system.");
        }

        var product = new Product
        {
            Name = req.Name,
            Description = req.Description,
            DateCreated = DateTime.UtcNow,
            CreatedByUser = userDetails.Id
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(ct);

        Response = new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            DateCreated = product.DateCreated,
            CreatedByUser = product.CreatedByUser
        };
    }
}