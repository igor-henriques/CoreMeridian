namespace CoreMeridian.Services;

internal sealed class ServerService : IServerService
{
    private readonly ServerConnection _serverConnection;
    private readonly ILogger<ServerService> _logger;

    public ServerService(
        ServerConnection serverConnection,
        ILogger<ServerService> logger)
    {
        this._serverConnection = serverConnection;
        this._logger = logger;
        
        PWGlobal.UsedPwVersion = serverConnection.PwVersion;
    }

    public bool SendPrivateMessage(int roleId, string message)
    {
        try
        {
            return PrivateChat.Send(_serverConnection.gdeliveryd, roleId, message);
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString(), LogLevel.Error);
            return false;
        }
    }

    public GRoleData GetRoleData(int roleId)
    {
        try
        {
            return PWToolKit.API.Gamedbd.GetRoleData.Get(_serverConnection.gamedbd, roleId);
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString(), LogLevel.Error);
            return default;
        }
    }

    public bool SetRoleData(GRoleData roleData)
    {
        try
        {
            return PutRoleData.Put(_serverConnection.gamedbd, roleData);
        }
        catch (Exception e)
        {
            _logger.Write(e.ToString(), LogLevel.Error);
            return false;
        }
    }
}