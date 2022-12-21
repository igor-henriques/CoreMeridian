namespace CoreMeridian.Models.PwServer;

public sealed record Gamedbd : IPwDaemonConfig
{
    [JsonProperty("HOST")]
    public string Host { get; set; }

    [JsonProperty("PORT")]
    public int Port { get; set; }
}
