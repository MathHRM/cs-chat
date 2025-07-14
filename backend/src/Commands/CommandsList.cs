using backend.Commands.Commands;

namespace backend.Commands;

public static class CommandsList
{
    public static Dictionary<string, Command> Commands = new Dictionary<string, Command>
    {
        { "help", new Help() },
    };
}