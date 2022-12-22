namespace CoreMeridian.Models.PwServer;

public sealed record ServerConnection
{
    [JsonProperty("GAMEDBD")]
    public Gamedbd gamedbd { get; init; }

    [JsonProperty("GPROVIDER")]
    public GProvider gprovider { get; init; }
    
    [JsonProperty("GDELIVERYD")]
    public GDeliveryd gdeliveryd { get; init; }

    [JsonProperty("LOGS_PATH")]
    public string logsPath { get; init; }

    [JsonProperty("PW_VERSION")]
    public PwVersion PwVersion { get; init; }
}
