using UnityEngine;

/// <summary>
/// Attach this to any GameObject you want the player to "pick up".
/// The player stores the ItemId when the object is collected.
/// </summary>
public class PickupItem : MonoBehaviour
{
    [Tooltip("A unique identifier for this item. Example: car_keys, health_potion, golden_coin")]
    public string ItemId = "";

    [Tooltip("Optional display name for UI or debugging.")]
    public string DisplayName = "";
}
