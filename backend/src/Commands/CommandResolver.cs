namespace backend.Commands;

public class CommandResolver : ICommandResolver
{
    private readonly IServiceProvider _serviceProvider;

    public CommandResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Command? GetCommand(string commandName)
    {
        if (CommandsList.CommandTypes.TryGetValue(commandName, out Type? commandType))
        {
            return (Command?)_serviceProvider.GetService(commandType);
        }

        return null;
    }

    public IEnumerable<Command> GetAllCommands()
    {
        foreach (var commandType in CommandsList.CommandTypes.Values)
        {
            var command = (Command?)_serviceProvider.GetService(commandType);
            if (command != null)
            {
                yield return command;
            }
        }
    }
}