using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("--- Create new user ---");

        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";

        bool usernameTaken = userRepository.GetManyAsync()
            .Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (usernameTaken)
        {
            Console.WriteLine($"Username '{username}' is already taken.");
            return;
        }

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        User user = new User
        {
            Username = username,
            Password = password
        };

        User created = await userRepository.AddAsync(user);
        Console.WriteLine($"User created with ID: {created.Id}");
    }
}
