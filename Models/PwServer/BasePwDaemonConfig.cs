namespace CoreMeridian.Models.PwServer;

public abstract record BasePwDaemonConfig
{
    [JsonProperty("HOST")]
    public string Host { get; init; }

    [JsonProperty("PORT")]
    public int Port { get; init; }
}
