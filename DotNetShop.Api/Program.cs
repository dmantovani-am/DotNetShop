
using DotNetShop.Api.Infrastructure;
using DotNetShop.Data;
using DotNetShop.Infrastructure;
using DotNetShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
InjectRepositories(builder.Services, builder.Configuration);

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSecretKey"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyName.IsAdmin, policy =>
    {
        policy.AddRequirements(new RoleRequirement(Role.Admin));
    });

    options.AddPolicy("@uniupo.it", policy =>
    {
        policy.AddRequirements(new UserNameRequirement());
    });
});

// Attenzione! Gli handler dei requirement vanno registrati nel contenitore delle dipendenze altrimenti non vengono innescati.
builder.Services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, UserNameRequirementHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DotNetShop API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

MapRoutes<Product>("Products");
MapRoutes<Category>("Categories");
MapOrderRoutes();

app.Run();


void InjectRepositories(IServiceCollection services, ConfigurationManager configuration)
{
    var connectionString = configuration.GetConnectionString("(default)");

    var options = new DbContextOptionsBuilder<DataContext>()
        .UseSqlite(connectionString)
        .Options;

    var dataContext = new DataContext(options);
    dataContext.Database.EnsureCreated();

    services.AddScoped<DataContext>(_ => dataContext);
    services.AddScoped<IRepository<Product>>(_ => new DataContextRepository<Product>(dataContext));
    services.AddScoped<IRepository<Category>>(_ => new DataContextRepository<Category>(dataContext));

    services.AddScoped<IOrderRepository>(_ => new OrderRepository(dataContext, null));
}

void MapRoutes<T>(string tag)
    where T : class, IHasId
{
    var prefix = $"/{tag}";
    var group = app.MapGroup(prefix).WithTags(tag);

    group.MapGet("/", (IRepository<T> repository) => repository.GetAll());

    group.MapGet("/{id}", (int id, IRepository<T> repository) => repository.Get(id))
        .Produces<T>(200)
        .Produces(404);

    group.MapPost("/", (T product, IRepository<T> repository) => repository.Add(product));

    group.MapDelete("/{id}", (int id, IRepository<T> repository) => repository.Delete(id));
}

void MapOrderRoutes()
{
    var group = app.MapGroup("/orders").WithTags("Orders");

    group
        .MapGet("/", (IOrderRepository repository) => repository.GetAll())
        .RequireAuthorization(PolicyName.IsAdmin);
}

class UserNameRequirement : IAuthorizationRequirement
{ }

class UserNameRequirementHandler : AuthorizationHandler<UserNameRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserNameRequirement requirement)
    {
        /// TODO: fix this
        if (context.User.HasClaim(c => c.Type == "email" && c.Value.EndsWith("@uniupo.it")))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}