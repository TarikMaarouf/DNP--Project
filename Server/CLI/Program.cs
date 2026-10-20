using CLI.UI;
using FileRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI app...");

IUserRepository userRepository = new UserFileRepository();      // old: UserInMemoryRepository()
IPostRepository postRepository = new PostFileRepository();      // old: PostInMemoryRepository()
ICommentRepository commentRepository = new CommentFileRepository(); // old: CommentInMemoryRepository()

CliApp cliApp = new CliApp(userRepository, postRepository, commentRepository);
await cliApp.StartAsync();
