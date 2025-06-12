public static class LoadBuffer {
    public static int? pendingSlotIndex = null;

    public static void Clear() {
        pendingSlotIndex = null;
    }

    public static bool HasPendingLoad => pendingSlotIndex != null;
}