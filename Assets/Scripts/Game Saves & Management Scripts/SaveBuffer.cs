/// <summary>
/// Temporary buffer for holding save requests across scene transitions.
/// </summary>
public static class SaveBuffer {
    public static int? pendingSlotIndex = null;
    public static string pendingPlayerName = null;

    public static void Clear() {
        pendingSlotIndex = null;
        pendingPlayerName = null;
    }

    public static bool HasPendingSave => pendingSlotIndex != null && !string.IsNullOrWhiteSpace(pendingPlayerName);
}
