using System.CommandLine;

namespace backend.Commands;

public static class CommandsList
{
    public static Dictionary<string, Type> CommandTypes = new Dictionary<string, Type>
    {
        { "help", typeof(Help) },
        { "login", typeof(Login) },
        { "register", typeof(Register) },
        { "join", typeof(Join) },
        { "logout", typeof(Logout) },
        { "chat", typeof(Chat) },
        { "create", typeof(Create) },
        { "profile", typeof(Profile) },
        { "list", typeof(List) },
    };
}