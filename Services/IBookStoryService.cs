using BookStories.Models;

namespace BookStories.Services;

public interface IBookStoryService
{
    Task<IEnumerable<BookStory>> GetAllSortedByRatingDesc();
    Task<BookStory?> GetById(int id);
    Task<BookStory> Create(BookStory story);
    Task<bool> Delete(int id);
}