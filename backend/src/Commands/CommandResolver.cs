namespace backend.Commands;

public class CommandResolver : ICommandResolver
{
    private readonly IServiceProvider _serviceProvider;

    public CommandResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CommandBase? GetCommand(string commandName)
    {
        if (CommandsList.CommandTypes.TryGetValue(commandName, out Type? commandType))
        {
            return (CommandBase?)_serviceProvider.GetService(commandType);
        }

        return null;
    }

    public IEnumerable<CommandBase> GetAllCommands()
    {
        foreach (var commandType in CommandsList.CommandTypes.Values)
        {
            var command = (CommandBase?)_serviceProvider.GetService(commandType);
            if (command != null)
            {
                yield return command;
            }
        }
    }
}