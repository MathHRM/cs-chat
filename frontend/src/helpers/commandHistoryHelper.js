import { useCommandHistoryStore } from "@/stores/commandHistory";

export function saveCommandHistory(command) {
  const commandHistoryStore = useCommandHistoryStore();
  commandHistoryStore.addCommand(command);
}

export function nextCommand() {
  const commandHistoryStore = useCommandHistoryStore();
  const command = commandHistoryStore.getNextCommand();

  return command;
}

export function previousCommand() {
  const commandHistoryStore = useCommandHistoryStore();
  const command = commandHistoryStore.getPreviousCommand();

  return command;
}
