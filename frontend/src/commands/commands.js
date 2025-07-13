import {
  handleHelp,
  handleLogin,
  handleRegister,
  handleJoinChat,
  handleLogout,
} from "@/helpers/commandsHelper";

import { useI18n } from "vue-i18n";

export default function commands() {
  const { t } = useI18n();

  const help = {
    description: t("commands.help.description"),
    args: {},
    handler: handleHelp,
  };

  const logout = {
    description: t("commands.logout.description"),
    args: {},
    handler: handleLogout,
  };

  const login = {
    description: t("commands.login.description"),
    args: {
      username: {
        type: "string",
        description: t("commands.login.args.username.description"),
        required: true,
      },
      password: {
        type: "string",
        description: t("commands.login.args.password.description"),
        required: true,
      },
    },
    handler: handleLogin,
  };

  const register = {
    description: t("commands.register.description"),
    args: {
      username: {
        type: "string",
        description: t("commands.register.args.username.description"),
        required: true,
      },
      password: {
        type: "string",
        description: t("commands.register.args.password.description"),
        required: true,
      },
    },
    handler: handleRegister,
  };

  const join = {
    description: t("commands.join.description"),
    args: {
      chatId: {
        type: "string",
        description: t("commands.join.args.chatId.description"),
        required: true,
      },
    },
    handler: handleJoinChat,
  };

  return {
    help,
    login,
    logout,
    register,
    join,
  };
}