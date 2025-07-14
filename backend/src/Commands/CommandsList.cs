using backend.Commands.Commands;

namespace backend.Commands;

public static class CommandsList
{
    public static Dictionary<string, Type> CommandTypes = new Dictionary<string, Type>
    {
        { "help", typeof(Help) },
        { "login", typeof(Login) },
    };
}