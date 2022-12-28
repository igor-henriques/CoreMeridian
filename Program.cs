Console.WriteLine("ELIMINANDO INSTÂNCIAS SECUNDÁRIAS DO COREMERIDIAN");
var currentProcess = Process.GetCurrentProcess();
var secondaryProcesses = Process.GetProcessesByName(currentProcess.ProcessName).Where(p => p.Id != currentProcess.Id);
foreach (var process in secondaryProcesses)
    process.Kill();

await Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddHostedService<EventsLogWorker>();
        services.AddHostedService<MeridianWorker>();
        services.AddHostedService<ChatLogWorker>();
        services.AddSingleton<IPendingMeridianOrders, PendingMeridianOrders>();
        services.AddSingleton<IServerService, ServerService>();
        services.AddSingleton<IChatMessageFactory, ChatMessageFactory>();        
        services.AddSingleton(
            _ => JsonConvert.DeserializeObject<ServerConnection>(File.ReadAllText("./Configurations/ServerConnection.json")));
    })
    .Build()
    .RunAsync();