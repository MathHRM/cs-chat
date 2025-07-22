import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useI18n } from "vue-i18n";
import { alert } from "@/helpers/messageHandler";

export default class Hub {
    constructor() {
        const { t } = useI18n();

        this.loginToken = localStorage.getItem('@auth');

        this.connection = new HubConnectionBuilder()
            .withUrl(`${process.env.VUE_APP_API_URL}/Hub`, { accessTokenFactory: () => this.loginToken })
            .withAutomaticReconnect([0, 1000, 3000, 5000, 10000, 20000])
            .configureLogging(LogLevel.Information)
            .build();

        this.connection.onreconnecting(() => {
            alert(t("connection.reconnecting"), 2);
        });

        this.connection.onreconnected(() => {
            alert(t("connection.reconnected"), 3);
        });

        this.connection.onclose(() => {
            alert(t("connection.closed"), 1);
        });
    }
}