export default function handleCommand(command, messages) {
    switch (command.command) {
        case "help":
            handleHelp(messages, command);
            break;
        default:
            break;
    }
}

function handleHelp(messages, command) {
    messages.value.push(command.response);
}