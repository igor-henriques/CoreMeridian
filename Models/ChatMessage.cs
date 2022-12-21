namespace CoreMeridian.Models;

internal sealed record ChatMessage
{
    public BroadcastChannel Channel { get; init; }
    public int RoleID { get; init; }
    public string Text { get; init; }
}
