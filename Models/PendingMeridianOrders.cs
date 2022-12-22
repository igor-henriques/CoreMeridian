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
        foreach (var order in this.Orders)
            if (logoffRecord.IdRoleLogoff.Equals(order.IssuerRoleId))
                order.IsLoggedOff = true;

        _logger.Write($"Logoff notification received for player ID {logoffRecord.IdRoleLogoff}", LogLevel.Information);
    }

    public void UpdateLogoffStatus(IEnumerable<LogoffRecord> logoffRecords)
    {
        foreach (var logoffRecord in logoffRecords)
            this.UpdateLogoffStatus(logoffRecord);
    }
    public void RemoveOrderByReference(MeridianOrder order)
    {
        List<MeridianOrder> recordsToRemove = new();

        for (int i = 0; i < Orders.Count; i++)
            if (Orders[i].Role.GRoleBase.Id.Equals(order.IssuerRoleId))
                recordsToRemove.Add(Orders[i]);

        foreach (var recordToRemove in recordsToRemove)
            this.Orders.Remove(recordToRemove);
    }
    public void UpdateDeliveredStatus(int issuerRoleId, bool deliveredStatus)
    {
        var orders = this.Orders.Where(order => order.IssuerRoleId.Equals(issuerRoleId));

        foreach (var order in orders)
            order.IsDelivered = deliveredStatus;
    }

    public List<MeridianOrder> GetDeliveredOrders()
    {
        return this.Orders.Where(order => order.IsDelivered).ToList();
    }
}