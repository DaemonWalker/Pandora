using System.Text;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Pandora.Api.Contract;
using Pandora.Api.Data;
using Pandora.Api.Middlewares;
using Pandora.Api.Service;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<ISniffer, BtsowSniffer>();
builder.Services.AddScoped<ISniffer, CDN1998Sniffer>();
builder.Services.AddScoped<ISniffer, CDN1999Sniffer>();
builder.Services.AddTransient<PandoraHttpClient>();
builder.Services.AddDbContext<PandoraDbContext>(options =>
    options.UseMySql(
        "Server=nas.com;Database=pandora;Uid=root;Pwd=123456;",
        new MySqlServerVersion("8.0.0")
    )
);
builder.Services.AddScoped(typeof(ISnifferConfigurationService<>), typeof(SnifferConfigurationService<>));
builder.Services.AddHttpClient<HttpClient>(client =>
{
    client.DefaultRequestHeaders.Add(
        "User-Agent",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0"
    );
});
builder.Services.AddTransient<HtmlWeb>();

var app = builder.Build();
app.UseMiddleware<ExceptionCatcherMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
//app.UseHttpsRedirection();

app.Run();
