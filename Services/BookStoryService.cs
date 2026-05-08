using BookStories.Data;
using BookStories.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStories.Services;

public class BookStoryService : IBookStoryService
{
    private readonly AppDbContext _context;

    public BookStoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookStory>> GetAllSortedByRatingDesc()
    {
        return await _context.Stories
            .OrderByDescending(s => s.Rating)
            .ToListAsync();
    }

    public async Task<BookStory?> GetById(int id)
    {
        return await _context.Stories.FindAsync(id);
    }

    public async Task<BookStory> Create(BookStory story)
    {
        if (story.Rating < 1 || story.Rating > 5)
        {
            throw new ArgumentException("Рейтинг должен быть от 1 до 5");
        }

        _context.Stories.Add(story);
        await _context.SaveChangesAsync();
        return story;
    }

    public async Task<bool> Delete(int id)
    {
        var story = await _context.Stories.FindAsync(id);
        if (story == null) return false;

        _context.Stories.Remove(story);
        await _context.SaveChangesAsync();
        return true;
    }
}