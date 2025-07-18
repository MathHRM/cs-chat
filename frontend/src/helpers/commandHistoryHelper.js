import { useCommandHistoryStore } from "@/stores/commandHistory";

export function saveCommandHistory(command) {
  const commandHistoryStore = useCommandHistoryStore();
  commandHistoryStore.addCommand(command);

  console.log(`Saved command: ${command}`);
}

export function nextCommand() {
  const commandHistoryStore = useCommandHistoryStore();
  const command = commandHistoryStore.getNextCommand();

  console.log(`Next command: ${command}`);

  return command;
}

export function previousCommand() {
  const commandHistoryStore = useCommandHistoryStore();
  const command = commandHistoryStore.getPreviousCommand();

  console.log(`Previous command: ${command}`);

  return command;
}
