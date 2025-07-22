namespace backend.Commands;

public interface ICommandResolver
{
    CommandBase? GetCommand(string commandName);
    IEnumerable<CommandBase> GetAllCommands();
}