using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public SinglePostView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.Write("Enter post id: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        Post post;
        try
        {
            post = await postRepository.GetSingleAsync(postId);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"=== {post.Title} (id {post.Id}) ===");
        Console.WriteLine(post.Body);
        Console.WriteLine();
        Console.WriteLine("Comments:");

        List<Comment> comments = commentRepository.GetManyAsync()
            .Where(c => c.PostId == postId)
            .ToList();

        if (comments.Count == 0)
        {
            Console.WriteLine("(no comments yet)");
        }
        else
        {
            foreach (Comment comment in comments)
            {
                Console.WriteLine($"- [{comment.Id}] (user {comment.UserId}): {comment.Body}");
            }
        }

        Console.WriteLine();
        Console.Write("Add a comment? (y/n): ");
        string? answer = Console.ReadLine();
        if (answer?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true)
        {
            await AddCommentAsync(postId);
        }
    }

    private async Task AddCommentAsync(int postId)
    {
        Console.Write("Your user id: ");
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

        Console.Write("Comment: ");
        string body = Console.ReadLine() ?? "";

        Comment comment = new Comment
        {
            Body = body,
            UserId = userId,
            PostId = postId
        };

        Comment created = await commentRepository.AddAsync(comment);
        Console.WriteLine($"Comment added with ID: {created.Id}");
    }
}
