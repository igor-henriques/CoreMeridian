namespace CoreMeridian.Models;

internal sealed record PendingMeridianOrders : IPendingMeridianOrders
{
    private readonly ILogger<PendingMeridianOrders> _logger;
    public List<MeridianOrder> Orders { get; }

    public PendingMeridianOrders(
        ILogger<PendingMeridianOrders> logger)
    {
        this.Orders = new();
        this._logger = logger;
    }

    public IEnumerable<MeridianOrder> GetPendingOrders => this.Orders.Where(order => order.IsLoggedOff);

    public void UpdateLogoffStatus(LogoffRecord logoffRecord)
    {
        this.Orders
            .Where(order => order.Role.GRoleBase.Id == logoffRecord.IdRoleLogoff)
            .ToList()
            .ForEach(order => order.IsLoggedOff = true);

        _logger.Write($"Logoff notification received for player ID {logoffRecord.IdRoleLogoff}", LogLevel.Information);
    }

    public void UpdateLogoffStatus(IEnumerable<LogoffRecord> logoffRecords) => logoffRecords.ToList().ForEach(this.UpdateLogoffStatus);
    public void RemoveOrderByReference(MeridianOrder order)
    {
        Orders.Where(o => o.Role.GRoleBase.Id.Equals(order.Role.GRoleBase.Id))
              .ToList()
              .ForEach(o => Orders.Remove(o));
    }
}