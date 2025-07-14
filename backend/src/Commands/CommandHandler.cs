namespace backend.Commands;

public class CommandHandler
{
    public static async Task HandleCommand(string command, Dictionary<string, object> args)
    {
        // if (CommandsList.Commands.TryGetValue(command, out var command))
        // {
        //     await command.Handler(args);
        // }
        await Task.CompletedTask;
    }
}