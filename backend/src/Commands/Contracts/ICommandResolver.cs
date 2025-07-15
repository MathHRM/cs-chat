namespace backend.Commands;

public interface ICommandResolver
{
    Command? GetCommand(string commandName);
    IEnumerable<Command> GetAllCommands();
}