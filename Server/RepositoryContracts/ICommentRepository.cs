using Entities;
using System;

namespace RepositoryContracts;

public interface  ICommentRepository
{
    Task<Post> AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(int id);
    Task<Post> GetSingleAsync(int id);
    IQueryable<Post>GetManyAsync();
}