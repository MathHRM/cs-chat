import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

export default class Hub {
    constructor() {
        this.loginToken = localStorage.getItem('@auth');

        this.connection = new HubConnectionBuilder()
            .withUrl(`${process.env.VUE_APP_API_URL}/Hub`, { accessTokenFactory: () => this.loginToken })
            .configureLogging(LogLevel.Information)
            .build();
    }
}