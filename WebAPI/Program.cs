using Application;
using CrossCuttingConcerns.Exceptions.Extensions;
using Microsoft.OpenApi;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(); // Add services from the Application assembly
builder.Services.AddPersistenceServices(builder.Configuration); // Add services from the Persistence assembly

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Pharmacy Depot API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global hata yönetimi yalnızca Production ortamında aktif edilir.
// Development ortamında varsayılan ASP.NET Core hata sayfası kullanılır.
if (app.Environment.IsProduction())
{
    // ─── GLOBAL HATA YÖNETİMİ MİDDLEWARE ────────────────────────────────────────
    // ÖNEMLI: Bu middleware mümkün olduğunca ERKEN pipeline'a eklenmelidir.
    // Böylece sonraki tüm middleware/controller'lardan fırlayan exception'lar yakalanır.
    // CrossCuttingConcerns katmanında tanımlanan ExceptionMiddleware devreye girer:
    //   - BusinessException → HTTP 400 + BusinessProblemDetails JSON
    //   - NotFoundException → HTTP 404 + NotFoundProblemDetails JSON
    //   - Exception (diğer) → HTTP 500 + InternalServerErrorProblemDetails JSON
    app.UseCustomExceptionMiddleware();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
