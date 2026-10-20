using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentsController(ICommentRepository commentRepo, IUserRepository userRepo, IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    // POST /comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto request)
    {
        User? author = userRepo.GetManyAsync().SingleOrDefault(u => u.Id == request.UserId);
        if (author is null)
        {
            return BadRequest($"User with ID '{request.UserId}' does not exist");
        }

        bool postExists = postRepo.GetManyAsync().Any(p => p.Id == request.PostId);
        if (!postExists)
        {
            return BadRequest($"Post with ID '{request.PostId}' does not exist");
        }

        Comment comment = new Comment
        {
            Body = request.Body,
            UserId = request.UserId,
            PostId = request.PostId
        };
        Comment created = await commentRepo.AddAsync(comment);

        CommentDto dto = ToDto(created, author.Username);
        return Created($"/comments/{dto.Id}", dto);
    }

    // PUT /comments/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto request)
    {
        Comment existingComment = await commentRepo.GetSingleAsync(id);
        existingComment.Body = request.Body;
        await commentRepo.UpdateAsync(existingComment);
        return NoContent();
    }

    // GET /comments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment([FromRoute] int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);
        string? authorName = GetUsernames().GetValueOrDefault(comment.UserId);
        return Ok(ToDto(comment, authorName));
    }

    // GET /comments?userId=1&username=alice&postId=2
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] string? username,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = commentRepo.GetManyAsync();
        Dictionary<int, string> usernames = GetUsernames();

        if (userId is not null)
        {
            comments = comments.Where(c => c.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            comments = comments.Where(c => usernames.ContainsKey(c.UserId)
                                           && usernames[c.UserId].Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        if (postId is not null)
        {
            comments = comments.Where(c => c.PostId == postId);
        }

        List<CommentDto> dtos = comments
            .Select(c => ToDto(c, usernames.GetValueOrDefault(c.UserId)))
            .ToList();
        return Ok(dtos);
    }

    // DELETE /comments/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteComment([FromRoute] int id)
    {
        await commentRepo.DeleteAsync(id);
        return NoContent();
    }

    private Dictionary<int, string> GetUsernames()
    {
        return userRepo.GetManyAsync().ToDictionary(u => u.Id, u => u.Username);
    }

    private static CommentDto ToDto(Comment comment, string? authorName)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            PostId = comment.PostId,
            AuthorName = authorName
        };
    }
}
