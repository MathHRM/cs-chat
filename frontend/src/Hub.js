import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

export default class Hub {
    constructor() {
        this.connection = new HubConnectionBuilder()
            .withUrl('https://localhost:5136/Hub')
            .configureLogging(LogLevel.Information)
            .build();
    }
}