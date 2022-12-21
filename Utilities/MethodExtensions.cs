namespace CoreMeridian.Utilities;

internal static class MethodExtensions
{
    public static bool PlayerHasRequiredItem(this GRoleData player, string itemId)
        => player.GRolePocket.Items.Where(inventory => inventory.Id.Equals(itemId)).Any();

    public static GRoleInventory[] CountItemOnInventory(this GRoleData player, string itemId)
        => player.GRolePocket.Items.Where(inventory => inventory.Id.Equals(itemId)).ToArray();
}
