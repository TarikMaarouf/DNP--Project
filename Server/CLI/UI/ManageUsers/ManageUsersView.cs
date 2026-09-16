using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
    private readonly IUserRepository userRepository;

    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage users ---");
            Console.WriteLine("1) Create new user");
            Console.WriteLine("2) List all users");
            Console.WriteLine("3) Update user");
            Console.WriteLine("4) Delete user");
            Console.WriteLine("0) Back");
            Console.Write("Choose an option: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    CreateUserView createUserView = new CreateUserView(userRepository);
                    await createUserView.ShowAsync();
                    break;
                case "2":
                    ListUsersView listUsersView = new ListUsersView(userRepository);
                    await listUsersView.ShowAsync();
                    break;
                case "3":
                    await UpdateUserAsync();
                    break;
                case "4":
                    await DeleteUserAsync();
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

    private async Task UpdateUserAsync()
    {
        Console.Write("Enter id of user to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        User existingUser;
        try
        {
            existingUser = await userRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        Console.Write($"New username (blank to keep '{existingUser.Username}'): ");
        string? username = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(username))
        {
            existingUser.Username = username;
        }

        Console.Write("New password (blank to keep current): ");
        string? password = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(password))
        {
            existingUser.Password = password;
        }

        await userRepository.UpdateAsync(existingUser);
        Console.WriteLine("User updated.");
    }

    private async Task DeleteUserAsync()
    {
        Console.Write("Enter id of user to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        try
        {
            await userRepository.DeleteAsync(id);
            Console.WriteLine("User deleted.");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
