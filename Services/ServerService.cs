namespace CoreMeridian.Services;

internal sealed class ServerService : IServerService
{
    private readonly ServerConnection _serverConnection;

    public ServerService(ServerConnection serverConnection)
    {
        this._serverConnection = serverConnection;
    }

    public void SendPrivateMessage(int roleId, string message)
          => PrivateChat.Send(_serverConnection.gdeliveryd, roleId, message);

    public GRoleData GetRoleData(int roleId)
          => PWToolKit.API.Gamedbd.GetRoleData.Get(_serverConnection.gamedbd, roleId);

    public bool SetRoleData(GRoleData roleData) => PutRoleData.Put(_serverConnection.gamedbd, roleData);
}
