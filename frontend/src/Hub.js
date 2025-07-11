import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

export default class Hub {
    constructor() {
        this.loginToken = localStorage.getItem('@auth');

        this.connection = new HubConnectionBuilder()
            .withUrl('http://localhost:5136/Hub', { accessTokenFactory: () => this.loginToken })
            .configureLogging(LogLevel.Information)
            .build();
    }
}