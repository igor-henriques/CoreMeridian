namespace CoreMeridian.Interfaces;

internal interface IServerService
{
    void SendPrivateMessage(int roleId, string message);
    GRoleData GetRoleData(int roleId);
    bool SetRoleData(GRoleData roleData);
}