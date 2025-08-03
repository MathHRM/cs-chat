
<template>
  <div v-if="show && filteredCommands.length > 0" class="command-modal">
    <div v-if="selectedCommand" class="command-syntax-hint">
      <span class="syntax-label">Sintaxe:</span>
      <span class="syntax-text">{{ getCommandSyntax(selectedCommand) }}</span>
    </div>

    <div class="command-list">
      <div
        v-for="(command, index) in filteredCommands"
        :key="command.name"
        class="command-item"
        :class="{ 'selected': index === selectedIndex }"
        @click="selectCommand(command)"
      >
        <div class="command-header">
          <span class="command-name">/{{ command.name }}</span>
          <span class="command-description">{{ command.description }}</span>
        </div>
        <div v-if="command.arguments && command.arguments.length > 0" class="command-arguments">
          <div v-for="arg in command.arguments" :key="arg.name" class="argument">
            <span class="argument-name">{{ arg.name }}</span>
            <span class="argument-description">{{ arg.description }}</span>
          </div>
        </div>
        <div v-if="command.options && command.options.length > 0" class="command-options">
          <div v-for="option in command.options" :key="option.name" class="option">
            <span class="option-name">{{ getOptionName(option) }}</span>
            <span class="option-description">{{ option.description }}</span>
          </div>
        </div>
      </div>
      <div v-if="isLoading" class="loading-commands">
        Carregando comandos...
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, defineProps, defineEmits } from "vue";

const props = defineProps({
  show: {
    type: Boolean,
    default: false,
  },
  commands: {
    type: Array,
    default: () => [],
  },
  selectedIndex: {
    type: Number,
    default: 0,
  },
  currentInput: {
    type: String,
    default: "",
  },
  isLoading: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["select-command"]);

const selectedCommand = computed(() => {
  if (props.show && filteredCommands.value.length > 0 && props.selectedIndex < filteredCommands.value.length) {
    return filteredCommands.value[props.selectedIndex];
  }
  return null;
});

const filteredCommands = computed(() => {
  if (!props.currentInput.startsWith("/")) return [];

  const searchTerm = props.currentInput.substring(1).toLowerCase();
  if (!searchTerm) return props.commands;

  return props.commands.filter(command =>
    command.name.toLowerCase().includes(searchTerm)
  );
});

const selectCommand = (command) => {
  emit("select-command", command);
};

const getOptionName = (option) => {
  if (option.isRequired) {
    return `${option.name}*`;
  }
  return option.name;
};

const getCommandSyntax = (command) => {
  let syntax = `/${command.name}`;

  if (command.arguments && command.arguments.length > 0) {
    syntax += ` ${command.arguments.map(arg => `<${arg.name}>`).join(' ')}`;
  }

  if (command.options && command.options.length > 0) {
    const requiredOptions = command.options.filter(opt => opt.isRequired);
    const optionalOptions = command.options.filter(opt => !opt.isRequired);

    if (requiredOptions.length > 0) {
      syntax += ` ${requiredOptions.map(opt => `${opt.name}`).join(' ')}`;
    }

    if (optionalOptions.length > 0) {
      syntax += ` [${optionalOptions.map(opt => `${opt.name}`).join(' | ')}]`;
    }
  }

  return syntax;
};
</script>

<style scoped>
.command-modal {
  position: absolute;
  bottom: 100%;
  left: 0;
  right: 0;
  background: #1a1a1a;
  border: 1px solid #333;
  border-radius: 4px;
  max-height: 300px;
  overflow-y: auto;
  z-index: 1000;
  margin-bottom: 5px;
}

.command-list {
  padding: 8px 0;
}

.command-item {
  padding: 8px 12px;
  cursor: pointer;
  border-bottom: 1px solid #333;
  transition: background-color 0.2s;
}

.command-item:hover,
.command-item.selected {
  background-color: #2a2a2a;
}

.command-item:last-child {
  border-bottom: none;
}

.command-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 4px;
}

.command-name {
  color: #4CAF50;
  font-weight: bold;
  font-family: 'Courier New', monospace;
}

.command-description {
  color: #ccc;
  font-size: 0.9em;
}

.command-arguments,
.command-options {
  margin-left: 20px;
  margin-top: 4px;
}

.argument,
.option {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 2px;
  font-size: 0.8em;
}

.argument-name,
.option-name {
  color: #FF9800;
  font-family: 'Courier New', monospace;
  min-width: 80px;
}

.argument-description,
.option-description {
  color: #999;
}

.no-commands,
.loading-commands {
  padding: 12px;
  color: #999;
  text-align: center;
  font-style: italic;
}

.loading-commands {
  color: #4CAF50;
}

.command-syntax-hint {
  background: #2a2a2a;
  border-bottom: 1px solid #333;
  padding: 8px 12px;
  font-family: 'Courier New', monospace;
  font-size: 0.9em;
}

.syntax-label {
  color: #4CAF50;
  font-weight: bold;
  margin-right: 8px;
}

.syntax-text {
  color: #ccc;
}
</style>