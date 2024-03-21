public static class TimeUtility {
    // 获取hour
    public static int GetH(int second) {
        return second / 3600;
    }

    // 获取minutes
    public static int GetM(int second) {
        return second / 60 % 60;
    }

    // 获取seconds
    public static int GetS(int second) {
        return second % 3600;
    }

    // 获取day
    public static int GetD(int second) {
        return second / 3600 / 24;
    }
}
