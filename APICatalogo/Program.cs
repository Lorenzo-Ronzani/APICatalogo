using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Models;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

//var AllowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(
    policy =>
    {
        policy.AllowAnyOrigin()
              .WithMethods("GET", "POST")
              .AllowAnyHeader();
    })
);


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    { 
        Title = "APICatalogo",
        Version = "v1", 
        Description = "Catálogo de Produtos e Categorias",
        TermsOfService = new Uri("https://my-portfolio-pearl-two-63.vercel.app/"),
        Contact = new OpenApiContact
        {
            Name = "lorenzoRonzani",
            Email = "lorenzo.lronzani@gmail.com",
            Url = new Uri ("https://my-portfolio-pearl-two-63.vercel.app/"),

        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri ("https://my-portfolio-pearl-two-63.vercel.app/")
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
    

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
{
    {
        new OpenApiSecuritySchemeReference("Bearer", document),
        new List<string>()
    }
});
});

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Config ASP NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Auth
builder.Services.AddAuthorization();

var secretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new InvalidOperationException("JWT:SecretKey não foi configurada.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin").
                                                          RequireClaim("id", "lorenzo"));

    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("ExclusiveOnly", policy =>
                                                policy.RequireAssertion(context =>
                                                context.User.HasClaim(Claim =>
                                                Claim.Type == "id" && Claim.Value == "Lorenzo")
                                                || context.User.IsInRole("SuperAdmin")));
});

builder.Services.AddRateLimiter(RateLimiterOptions =>
{
    RateLimiterOptions.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(5);
        options.QueueLimit = 2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
      

    });

    RateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

});

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
                        new QueryStringApiVersionReader(),
                        new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;


});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(typeof(ProductDTOMappingProfile));

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
//CORS
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();