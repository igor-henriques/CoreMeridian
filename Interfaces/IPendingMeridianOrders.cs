namespace CoreMeridian.Interfaces;

internal interface IPendingMeridianOrders
{
    List<MeridianOrder> Orders { get; }
    void UpdateLogoffStatus(LogoffRecord logoffRecord);
    void UpdateLogoffStatus(IEnumerable<LogoffRecord> logoffRecords);
    void RemoveOrderByReference(MeridianOrder order);
    void UpdateDeliveredStatus(int issuerRoleId, bool deliveredStatus);
    List<MeridianOrder> GetDeliveredOrders();
}