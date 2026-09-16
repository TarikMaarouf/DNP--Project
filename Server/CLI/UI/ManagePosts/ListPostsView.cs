using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public Task ShowAsync()
    {
        Console.WriteLine();
        Console.Write("Filter by user id (blank for none): ");
        string? filterInput = Console.ReadLine();

        IQueryable<Post> posts = postRepository.GetManyAsync();
        if (int.TryParse(filterInput, out int userId))
        {
            posts = posts.Where(p => p.UserId == userId);
        }

        Console.WriteLine();
        Console.WriteLine("--- Posts overview ---");
        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }

        return Task.CompletedTask;
    }
}
