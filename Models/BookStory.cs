namespace BookStories.Models;

public class BookStory
{
    public int Id { get; set; }
    public string BookInfo { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5
    public string Text { get; set; } = string.Empty;
}