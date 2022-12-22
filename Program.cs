await Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddHostedService<EventsLogWorker>();
        services.AddHostedService<MeridianWorker>();
        services.AddHostedService<ChatLogWorker>();
        services.AddSingleton<IPendingMeridianOrders, PendingMeridianOrders>();
        services.AddSingleton<IServerService, ServerService>();
        services.AddSingleton<IChatMessageFactory, ChatMessageFactory>();
        services.AddSingleton<ServerConnection>(
            _ => JsonConvert.DeserializeObject<ServerConnection>(File.ReadAllText("./Configurations/ServerConnection.json")));
    })
    .Build()
    .RunAsync();