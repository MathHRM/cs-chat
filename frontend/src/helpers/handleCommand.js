function handleHelp(messages, command)
{
    messages.value.push(command.response);
}

export default function proxy(messages, command)
{
    if (!command.success)
    {
        
    }

    switch (command.command)
    {
        case "help":
            handleHelp(messages, command);
            break;
        default:
            break;
    }
}