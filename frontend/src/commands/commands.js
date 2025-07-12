import {
    handleHelp,
    handleLogin,
    handleRegister,
    handleJoinChat,
} from "@/helpers/commandsHelper";

export const help = {
  description: "Show available commands and their usage",
  args: {},
  handler: handleHelp,
};

export const login = {
  description: "Login to the system",
  args: {
    username: {
      type: "string",
      description: "The username to login with",
      required: true,
    },
    password: {
      type: "string",
      description: "The password to login with",
      required: true,
    },
  },
  handler: handleLogin,
};

export const register = {
  description: "Register a new user",
  args: {
    username: {
      type: "string",
      description: "The username to register with",
      required: true,
    },
    password: {
      type: "string",
      description: "The password to register with",
      required: true,
    },
  },
  handler: handleRegister,
};

export const joinChat = {
  description: "Join a chat",
  args: {
    chatId: {
      type: "string",
      description: "The ID of the chat to join",
      required: true,
    },
  },
  handler: handleJoinChat,
};

export default {
  help,
  login,
  register,
  joinChat,
};
