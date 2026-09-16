using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CreatePostView(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("--- Create new post ---");

        Console.Write("Title: ");
        string title = Console.ReadLine() ?? "";

        Console.Write("Body: ");
        string body = Console.ReadLine() ?? "";

        Console.Write("User id (author): ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid user id.");
            return;
        }

        try
        {
            await userRepository.GetSingleAsync(userId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No user with ID '{userId}' exists.");
            return;
        }

        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        Post created = await postRepository.AddAsync(post);
        Console.WriteLine($"Post created with ID: {created.Id}");
    }
}
