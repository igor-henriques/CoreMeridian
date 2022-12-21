namespace CoreMeridian.Models;

public sealed record LogoffRecord
{
    public DateTime RoleLogoffTime { get; init; }
    public int IdRoleLogoff { get; init; }

    public static LogoffRecord ParseFromLogString(string log)
    {
        var logData = new LogoffRecord
        {
            RoleLogoffTime = DateTime.Now,
            IdRoleLogoff = int.Parse(CompiledLogoffRegex.Regex.Match(log).Value.Replace(JapaneseLogIdentifiers.LogoffLogId, null).Trim())
        };

        return logData;
    }
}
