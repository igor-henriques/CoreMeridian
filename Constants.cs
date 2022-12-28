namespace CoreMeridian;

internal sealed record LogIdentifier
{
    public const string Logoff = "playerlogout";
}

internal sealed record LogFilePath
{
    public const string World2Chat = "world2.chat";
    public const string World2Log = "world2.log";
    public const string World2FormatLog = "world2.formatlog";
    public const string OutputLogFilePath = "./log.txt";
}

internal sealed record ChatTrigger
{
    public const string MeridianoTrigger = "!meridiano";
}

internal sealed record GameConstants
{
    public const string MeridianData = "0000005000000000000000000000000500000064000048760000000100000000000000000000000000000000000000000000000000000000";
}

internal sealed record MeridianItem
{
    public static readonly string Id;

    static MeridianItem()
    {
        Id = File.ReadAllText("./Configurations/MeridianItem.txt");
    }
}