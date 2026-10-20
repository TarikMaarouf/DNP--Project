using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;
public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = [];

    public PostInMemoryRepository()
    {
        posts.Add(new Post { Id = 1, Title = "Welcome to the forum", Body = "This is the first post on this forum. Enjoy your stay!", UserId = 1 });
        posts.Add(new Post { Id = 2, Title = "Best pizza toppings", Body = "What are your favorite pizza toppings? Mine is pepperoni.", UserId = 2 });
        posts.Add(new Post { Id = 3, Title = "Learning C#", Body = "I just started learning C# and I'm loving it so far.", UserId = 3 });
    }

    public Task<Post>AddAsync (Post post)
    {
        post.Id = posts.Any()
        ? posts.Max(p=>p.Id) + 1
        :1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault (p=> p.Id == post.Id);
        if (existingPost is null)
        {
            throw new NotFoundException($"Post with ID '{post.Id}' not found");
        }
        posts.Remove(existingPost);
        posts.Add(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);

        if (postToRemove is null)
        {
            throw new NotFoundException(
            $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);

        return Task.CompletedTask;

    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);

        if (post is null)
        {
            throw new NotFoundException(
                $"Post with ID '{id}' not found");
        }
        
        return Task.FromResult(post);
    }

    public IQueryable<Post> GetManyAsync()
    {
        return posts.AsQueryable();
    }
}