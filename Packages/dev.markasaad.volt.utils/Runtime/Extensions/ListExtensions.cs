public static class ListExtensions {
    public static bool IsAnyNull(this object[] objects) {
        int l = objects.Length;

        for (int i = 0; i < l; i++)
            if (objects[i] == null)
                return true;

        return false;
    }
}