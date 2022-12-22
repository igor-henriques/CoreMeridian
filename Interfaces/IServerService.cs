namespace CoreMeridian.Interfaces;

internal interface IServerService
{
    bool SendPrivateMessage(int roleId, string message);
    GRoleData GetRoleData(int roleId);
    bool SetRoleData(GRoleData roleData);
}