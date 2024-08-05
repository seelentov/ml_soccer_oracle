using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Services;
using WebApplication2.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite($"Data Source=db.db"));


builder.Services.AddSingleton<MLGameAdapter>();
builder.Services.AddSingleton<HTMLDriverFactory>();
builder.Services.AddSingleton<SeleniumFactory>();
builder.Services.AddSingleton<FormatService>();
builder.Services.AddSingleton<Soccer365Parser>();

builder.Services.AddTransient<GamesService>();
builder.Services.AddTransient<OptionsService>();
builder.Services.AddTransient<LeaguesService>();

builder.Services.AddHostedService<LeaguesWorker>();
builder.Services.AddHostedService<GamesWorker>();
builder.Services.AddHostedService<MLWorker>();

var app = builder.Build();

app.Run();
