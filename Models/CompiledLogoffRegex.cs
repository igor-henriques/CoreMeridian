namespace CoreMeridian.Models;

internal sealed class CompiledLogoffRegex
{
    public readonly static Regex Regex = new Regex(@"playerlogout:roleid=([0-9]*)", RegexOptions.Compiled);
}
