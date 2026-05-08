using BookStories.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStories.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BookStory> Stories => Set<BookStory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Инициализация тестовыми данными
        modelBuilder.Entity<BookStory>().HasData(
            new BookStory { Id = 1, BookInfo = "Война и мир. Толстой", Author = "User1", Rating = 5, Text = "Классический шедевр." },
            new BookStory { Id = 2, BookInfo = "Хоббит. Толкин", Author = "User2", Rating = 4, Text = "Великое приключение." },
            new BookStory { Id = 3, BookInfo = "1984. Оруэлл", Author = "User3", Rating = 3, Text = "Антиутопический роман." }
        );
    }
}