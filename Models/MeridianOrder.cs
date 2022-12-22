namespace CoreMeridian.Models;

public sealed record MeridianOrder
{
	public int IssuerRoleId { get; init; }
	public GRoleData Role { get; init; }
	public bool IsLoggedOff { get; set; }
	public bool IsDelivered { get; set; }
}