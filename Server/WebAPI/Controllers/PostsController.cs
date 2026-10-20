using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }

    // POST /posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        User? author = userRepo.GetManyAsync().SingleOrDefault(u => u.Id == request.UserId);
        if (author is null)
        {
            return BadRequest($"User with ID '{request.UserId}' does not exist");
        }

        Post post = new Post
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.UserId
        };
        Post created = await postRepo.AddAsync(post);

        PostDto dto = ToDto(created, author.Username);
        return Created($"/posts/{dto.Id}", dto);
    }

    // PUT /posts/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        Post existingPost = await postRepo.GetSingleAsync(id);
        existingPost.Title = request.Title;
        existingPost.Body = request.Body;
        await postRepo.UpdateAsync(existingPost);
        return NoContent();
    }

    // GET /posts/{id}?includeComments=true
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost([FromRoute] int id, [FromQuery] bool includeComments = false)
    {
        Post post = await postRepo.GetSingleAsync(id);
        Dictionary<int, string> usernames = GetUsernames();

        PostDto dto = ToDto(post, usernames.GetValueOrDefault(post.UserId));
        if (includeComments)
        {
            dto.Comments = GetCommentsForPost(id, usernames);
        }

        return Ok(dto);
    }

    // GET /posts?titleContains=pizza&userId=2&username=bob
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetPosts(
        [FromQuery] string? titleContains,
        [FromQuery] int? userId,
        [FromQuery] string? username)
    {
        IQueryable<Post> posts = postRepo.GetManyAsync();
        Dictionary<int, string> usernames = GetUsernames();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            posts = posts.Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
        }

        if (userId is not null)
        {
            posts = posts.Where(p => p.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            posts = posts.Where(p => usernames.ContainsKey(p.UserId)
                                     && usernames[p.UserId].Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        List<PostDto> dtos = posts
            .Select(p => ToDto(p, usernames.GetValueOrDefault(p.UserId)))
            .ToList();
        return Ok(dtos);
    }

    // GET /posts/{id}/comments
    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsOfPost([FromRoute] int id)
    {
        await postRepo.GetSingleAsync(id); // throws NotFoundException -> 404 if the post does not exist
        return Ok(GetCommentsForPost(id, GetUsernames()));
    }

    // DELETE /posts/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePost([FromRoute] int id)
    {
        await postRepo.DeleteAsync(id);
        return NoContent();
    }

    private Dictionary<int, string> GetUsernames()
    {
        return userRepo.GetManyAsync().ToDictionary(u => u.Id, u => u.Username);
    }

    private List<CommentDto> GetCommentsForPost(int postId, Dictionary<int, string> usernames)
    {
        return commentRepo.GetManyAsync()
            .Where(c => c.PostId == postId)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                PostId = c.PostId,
                AuthorName = usernames.GetValueOrDefault(c.UserId)
            })
            .ToList();
    }

    private static PostDto ToDto(Post post, string? authorName)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            AuthorName = authorName
        };
    }
}
