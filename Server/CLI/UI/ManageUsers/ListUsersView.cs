using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public Task ShowAsync()
    {
        Console.WriteLine();
        Console.Write("Filter by username contains (blank for none): ");
        string? filter = Console.ReadLine();

        IQueryable<User> users = userRepository.GetManyAsync();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            users = users.Where(u => u.Username.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        Console.WriteLine();
        Console.WriteLine("--- Users ---");
        foreach (User user in users)
        {
            Console.WriteLine($"[{user.Id}] {user.Username}");
        }

        return Task.CompletedTask;
    }
}
