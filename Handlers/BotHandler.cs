using System.Text;
using System.Text.RegularExpressions;
using AssisToad.database;
using AssisToad.Entities;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Issue = AssisToad.Entities.Task;
using Task = System.Threading.Tasks.Task;
using User = AssisToad.Entities.User;

namespace AssisToad.Handlers;

public class BotHandler
{

    private static readonly string FROG = "┈┈┈┈╱▔╲╮╭╱▔╲┈┈┈┈\n" +
        "┈┈┈┈▏▉▕╰╯▏▉▕┈┈┈┈\n" +
        "┈┈┈╱╲▂╱┈┈╲▂╱╲┈┈┈\n" +
        "┈╱▔┈┈┈┈┈┈┈┈┈┈▔╲┈ \n" +
        "┈╲╲▂▂▂▂▂▂▂▂▂▂╱╱┈ \n" +
        "┈╱╲┈┈┈┈┈┈┈┈┈┈╱╲┈ \n" +
        "╱┈╱▏▕▔╲▃╱▔▏┈▕╲┈╲ \n" +
        "▏▕┈▏▕▂╱▔╲▂▏┈▕┈▏▕\n"; 
    private static ITelegramBotClient bot;
    private static ApplicationContext Context;

    public static void SetContext(ApplicationContext context)
    {
        Context = context;
        Context.Users.Add(new User() { ChatId = 0 });
        Context.SaveChanges();
    }

    static BotHandler()
    {
        bot = new TelegramBotClient(Environment.GetEnvironmentVariable("botToken") ?? string.Empty);
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message == null)
            return;
 
        var answer = string.Empty;
        var id = update.Message.Chat.Id;
        var users = Context.Users;
        User user;
        try
        {
            user = Context.Users.First(u => u.ChatId == id);
        }
        catch
        {
            user = new User() { ChatId = id };
            Context.Add(user);
        }

        await Context.SaveChangesAsync(cancellationToken);

        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;
            var textMessage = Regex.Replace(message!.Text?.Trim().ToLower() ?? string.Empty,"\\s+", " ");
            var options = Regex.Matches(textMessage, " --?[a-zA-Z0-9]+ ");
            var values = Regex.Split(textMessage, "\\s--?[a-zA-Z0-9]+\\s");
            var command = values[0];
            
            switch (command)
            {
                case "at":
                case "add task":
                    answer += AddTask(options, values, user);
                    break;
                case "ac":
                case "add category":
                    answer += AddCategory(options, values, user);
                    break;
                case "gt":
                case "get tasks":
                    answer += GetTasks(options, values, user);
                    break;
                case "ss":
                case "set status":
                    answer += SetStatus(options, values, user);
                    break;
                case "gc":
                case "get categories":
                    answer += GetCategories(user);
                    break;
                case "non":
                case "notify on":
                    user.Settings.Notify = true;
                    break;
                case "no":
                case "notify off":
                    user.Settings.Notify = false;
                    break;
                case "df":
                case "date format":
                    answer = $"Your date foaaormat: {user.Settings.DateTimeFormat}";
                    break;
                default:
                    answer = "Croooak?";
                    break;
            }

            await Context.SaveChangesAsync(cancellationToken);
            await botClient.SendTextMessageAsync(message.Chat, answer, cancellationToken: cancellationToken);
        }
    }

    private static string GetCategories(User user)
    {
        var result = new StringBuilder("Your croaaategories:\n");

        foreach (var category in Context.Categories.Where(e => e.Owner.Equals(user)))
            result.Append('-').Append(category).Append('\n');
        
        return result.ToString();
    }

    private static string SetStatus(MatchCollection options, IReadOnlyList<string> values, User user)
    {
        string answer;
        try
        {
            var title = string.Empty;
            var status = Status.NONE;
            for (var i = 0; i < options.Count; i++)
            {
                switch (options[i].ToString().Trim())
                {
                    case "-t":
                    case "--title":
                        title = values[i + 1];
                        break;
                    case "-s":
                    case "--status":
                        status = (Status)Enum.Parse(typeof(Status), values[i + 1].ToUpper());
                        break;
                }
            }

            var task = Context.Tasks.First(e => e.Title.Equals(title) && e.Owner.Equals(user));
            task.Status = status;
            Context.Update(task);
            answer = $"Yeaaaah, successful update status task {task.Title}";
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception);
            answer = "Invalid command or options, frogy bro(";
        }

        return answer;
    }

    private static string GetTasks(MatchCollection options, IReadOnlyList<string> values, User user)
    {
        try
        {
            var tasks = Context.Tasks.Where(e => e.Owner.Equals(user));
            for (var i = 0; i < options.Count; i++)
            {
                switch (options[i].ToString())
                {
                    case "-c":
                    case "--category":
                        tasks = tasks.Where(e => e.Category.Title.Equals(values[i + 1]));
                        i++;
                        break;
                    case "-d":
                    case "--deadline":
                        tasks = tasks.Where(e => e.Deadline <= DateTime.Parse(values[i + 1]));
                        i++;
                        break;
                }
            }

            var result = new StringBuilder("Your toooasks:\n");
            foreach (var task in tasks)
                result.Append('*').Append(task).Append('\n');

            return result.ToString();
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception);
            return "Invalid command or options, bro :(";
        }
    }

    private static string AddCategory(MatchCollection options, IReadOnlyList<string> values, User user)
    {
        string answer;
        try
        {
            var newCategory = new Category() {Owner = Context.Users.First(u => u.Equals(user))};
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i].ToString().Trim();
                if (option.Equals("-t") || option.Equals("--title"))
                    newCategory.Title = values[i + 1];
            }

            Context.Categories.Add(newCategory);
            Context.SaveChanges();
            answer = "Success creating new category!";
        }
        catch(Exception exception)
        {
            answer = "Invooalid category title";
            Console.WriteLine(exception);
        }

        return answer;
    }

    private static string AddTask(MatchCollection options, IReadOnlyList<string> values, User user)
    {
        string answer;
        try
        {
            var newTask = new Issue{ Owner = Context.Users.First(u => u.Equals(user)) };
            for (var i = 0; i < options.Count; i++)
            {
                switch (options[i].ToString().Trim())
                {
                    case "-t":
                    case "--title":
                        newTask.Title = values[i + 1];
                        break;
                    case "-c":
                    case "--content":
                        try
                        {
                            newTask.Content = values[i + 1];
                        }
                        catch
                        {
                            throw new Exception("Invalid task content :(");
                        }

                        break;
                    case "-ct":
                    case "--category":
                        try
                        {
                            var category = Context.Categories.First(e => e.Title.Equals(values[i + 1]));
                            newTask.Category = category;
                        }
                        catch
                        {
                            throw new Exception($"Brooak, not found a category with title: {values[i + 1]}");
                        }
                        break;
                    case "-d":
                    case "--deadline":
                        try
                        {
                            newTask.Deadline = DateTime.Parse(values[i + 1]);
                        }
                        catch
                        {
                            throw new Exception(
                                "Cro... Bro, invalid date format");
                        }
                        break;
                    case "-i":
                    case "--importance":
                        var value = values[i + 1].ToString();
                        if (value.Length == 1)
                            value = value.Replace("l", "low").Replace("m", "medium").Replace("h", "high").ToUpper();
                        Level level;
                        try
                        {
                            level = (Level)Enum.Parse(typeof(Level), value);
                        }
                        catch
                        {
                            throw new Exception(
                                "Broooa, choaaaaase one of the next importance levels: low/medium/high");
                        }
                        newTask.Importance = level;
                        break;
                }
            }

            Context.Tasks.Add(newTask);
            Context.SaveChanges();
            answer = "Success croating new task!";
        }
        catch(Exception exception)
        {
            answer = $"*Sad croooaking* \nInvalid command or options: \n{exception.Message}";
            Console.WriteLine(exception);
        }

        return answer;
    }

    private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }

    public static void StartBot()
    {
        Console.WriteLine("Was started bot:  " + bot.GetMeAsync().Result.FirstName);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = {},
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
    }

}