namespace CoreMeridian.Models;

public sealed record MeridianOrder
{
	public GRoleData Role { get; init; }
	public bool IsLoggedOff { get; set; }
}