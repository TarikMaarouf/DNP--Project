using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public ManagePostsView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage posts ---");
            Console.WriteLine("1) Create new post");
            Console.WriteLine("2) View posts overview");
            Console.WriteLine("3) View single post (and comments)");
            Console.WriteLine("4) Update post");
            Console.WriteLine("5) Delete post");
            Console.WriteLine("0) Back");
            Console.Write("Choose an option: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    CreatePostView createPostView = new CreatePostView(postRepository, userRepository);
                    await createPostView.ShowAsync();
                    break;
                case "2":
                    ListPostsView listPostsView = new ListPostsView(postRepository);
                    await listPostsView.ShowAsync();
                    break;
                case "3":
                    SinglePostView singlePostView = new SinglePostView(postRepository, commentRepository, userRepository);
                    await singlePostView.ShowAsync();
                    break;
                case "4":
                    await UpdatePostAsync();
                    break;
                case "5":
                    await DeletePostAsync();
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option, try again.");
                    break;
            }
        }
    }

    private async Task UpdatePostAsync()
    {
        Console.Write("Enter id of post to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        Post existingPost;
        try
        {
            existingPost = await postRepository.GetSingleAsync(id);
        }
        catch (NotFoundException e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        Console.Write($"New title (blank to keep '{existingPost.Title}'): ");
        string? title = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(title))
        {
            existingPost.Title = title;
        }

        Console.Write("New body (blank to keep current): ");
        string? body = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(body))
        {
            existingPost.Body = body;
        }

        await postRepository.UpdateAsync(existingPost);
        Console.WriteLine("Post updated.");
    }

    private async Task DeletePostAsync()
    {
        Console.Write("Enter id of post to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        try
        {
            await postRepository.DeleteAsync(id);
            Console.WriteLine("Post deleted.");
        }
        catch (NotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
