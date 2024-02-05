
using WebApplication2.Services;
using static WebApplication2.TestServices;

namespace WebApplication2
{
    [TestAttribute("ProgramT")]
    public class Program
    {
        [TestAttribute("MainT")]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:7250/api/Prices") // Update with the actual URL of WebApplication2
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddTransient<ITransientService, TransientService>();
            builder.Services.AddScoped<IScopedService, ScopedService>();
            builder.Services.AddSingleton<ISingletonService, SingletonService>();

            builder.Services.AddHttpClient();
            builder.Services.AddHostedService<ElectricityPriceFetchingService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowSpecificOrigin");

            app.MapControllers();

            app.Run();
        }
    }
}