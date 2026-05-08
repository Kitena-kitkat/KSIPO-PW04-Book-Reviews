using BookStories.Data;
using BookStories.Services;
using BookStories.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookStoryService, BookStoryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); 
}


app.MapGet("/api/config", (IConfiguration config) =>
{
    return Results.Json(new
    {
        AppName = config["AppSettings:AppName"],
        Version = config["AppSettings:Version"],
        ConnectionStringName = "DefaultConnection"
    });
});

app.MapGet("/api/stories", async (IBookStoryService service) =>
{
    var stories = await service.GetAllSortedByRatingDesc();
    
    var result = stories.Select(s => new 
    { 
        s.Id, 
        s.BookInfo, 
        s.Author, 
        s.Rating 
    });

    return Results.Json(result);
});

app.MapGet("/api/stories/{id}", async (int id, IBookStoryService service) =>
{
    var story = await service.GetById(id);
    if (story == null) return Results.NotFound();
    
    return Results.Json(story);
});

app.MapPost("/api/stories", async (BookStory story, IBookStoryService service) =>
{
    try
    {
        var created = await service.Create(story);
        return Results.Created($"/api/stories/{created.Id}", created);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/api/stories/{id}", async (int id, IBookStoryService service) =>
{
    var deleted = await service.Delete(id);
    if (!deleted) return Results.NotFound();
    
    return Results.NoContent(); 
});

app.Run();