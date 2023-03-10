namespace CoreMeridian.Workers;

internal sealed class MeridianWorker : BackgroundService
{
    private readonly IPendingMeridianOrders _pendingMeridianOrders;
    private readonly IServerService _serverService;
    private readonly ILogger<MeridianWorker> _logger;

    public MeridianWorker(
        IPendingMeridianOrders pendingMeridianOrders,
        IServerService serverService,
        ILogger<MeridianWorker> logger)
    {
        this._pendingMeridianOrders = pendingMeridianOrders;
        this._serverService = serverService;
        this._logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting {nameof(MeridianWorker)}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var ordersToRemove = new List<MeridianOrder>();
                var readyPendingOrders = _pendingMeridianOrders.Orders.Where(order => order.IsLoggedOff);

                if (!readyPendingOrders.Any())
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                foreach (var readyOrder in readyPendingOrders)
                {
                    var updatedIssuerPlayer = _serverService.GetRoleData(readyOrder.Role.GRoleBase.Id);
                    var itemsFound = updatedIssuerPlayer.CountItemOnInventory(MeridianItem.Id);

                    if (itemsFound.Length <= 0)
                    {
                        _logger.Write($"Weren't found any item id {MeridianItem.Id} on player id {readyOrder.IssuerRoleId} inventory when trying to deliver meridian after log off. Probably the player tried to remove the item from inventory.", LogLevel.Critical);
                        ordersToRemove.Add(readyOrder);
                        continue;
                    }

                    readyOrder.Role.GRolePocket.Items = itemsFound.Length > 1
                        ? BuildInventory(updatedIssuerPlayer.GRolePocket.Items.ToList(), int.Parse(MeridianItem.Id), decreaseCount: true)
                        : BuildInventory(updatedIssuerPlayer.GRolePocket.Items.Except(itemsFound).ToList(), itemId: int.Parse(MeridianItem.Id));

                    readyOrder.Role.GRoleStatus.MeridianData = GameConstants.MeridianData;
                    var response = _serverService.SetRoleData(readyOrder.Role);

                    if (!response)
                    {
                        _logger.Write($"Failed to deliver meridian to player {readyOrder.Role.GRoleBase.Id}", LogLevel.Critical);
                        _ = _serverService.SendPrivateMessage(readyOrder.Role.GRoleBase.Id, "Falha ao entregar meridiano. Entre em contato com a administração.");
                        ordersToRemove.Add(readyOrder);
                        continue;
                    }

                    _pendingMeridianOrders.UpdateDeliveredStatus(readyOrder.IssuerRoleId, deliveredStatus: true);
                    _logger.Write($"Successfully delivered for {readyOrder.Role.GRoleBase.Name}");
                }

                ordersToRemove.AddRange(_pendingMeridianOrders.GetDeliveredOrders());
                ordersToRemove.ForEach(_pendingMeridianOrders.RemoveOrderByReference);

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.Write(e.ToString(), LogLevel.Error);
                await Task.Delay(10_000, stoppingToken);
            }
        }
    }

    private static GRoleInventory[] BuildInventory(List<GRoleInventory> inventory, int itemId, bool decreaseCount = false)
    {
        GRoleInventory[] newInventory = new GRoleInventory[inventory.Count];

        for (int i = 0; i < inventory.Count; i++)
        {
            if (decreaseCount & inventory[i].Id.Equals(itemId))
            {
                inventory[i].Count--;
            }

            newInventory[i] = inventory[i];
        }

        return newInventory;
    }
}
