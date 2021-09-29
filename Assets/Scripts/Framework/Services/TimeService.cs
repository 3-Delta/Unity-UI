using System;

// 时区最本质：都是计算当前到1970的一个时间差diff, 只不过在同一时间，比如北京是8点的时候，伦敦是0点，也就是秒数一样，但是时间展示不一样。
// 但是c#在处理datetime.now和datetime.utcnow的时候，居然通过+-时区秒数的方式去实现。
public static class TimeService {
    public static readonly DateTime UTC_START_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    
    public static bool IsSameDay(DateTime x, DateTime y) {
        return x.Year == y.Year && x.DayOfYear == y.DayOfYear;
    }

    public static bool IsSameDay(long xSeconds, long ySeconds, long secondsOffset = 0, long timeZoneSeconds = 0) {
        if (xSeconds == ySeconds) {
            return true;
        }

        DateTime l = ToDateTime(xSeconds - secondsOffset, timeZoneSeconds);
        DateTime r = ToDateTime(xSeconds - secondsOffset, timeZoneSeconds);
        return IsSameDay(l, r);
    }
    
    public static bool IsSameWeek(DateTime x, DateTime y) {
        var l = x.AddDays(-(int)x.DayOfWeek).Date;
        var r = y.AddDays(-(int)y.DayOfWeek).Date;
        return IsSameDay(l, r);
    }

    public static bool IsSameWeek(long xSeconds, long ySeconds, long secondsOffset = 0, long timeZoneSeconds = 0) {
        if (xSeconds == ySeconds) {
            return true;
        }

        DateTime l = ToDateTime(xSeconds - secondsOffset, timeZoneSeconds);
        DateTime r = ToDateTime(xSeconds - secondsOffset, timeZoneSeconds);
        return IsSameWeek(l, r);
    }

    public static long ToSeconds(DateTime dt, long timeZoneSeconds = 0) {
        return (long)dt.Subtract(UTC_START_TIME).TotalSeconds - timeZoneSeconds;
    }

    public static DateTime ToDateTime(long utcSeconds, long timeZoneSeconds) {
        DateTime startTime = UTC_START_TIME;
        DateTime targetTime = startTime.AddSeconds(utcSeconds + timeZoneSeconds);
        return targetTime; 
    }
    
    // DateTime utcBegin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    // DateTime localBegin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
    //     
    // Debug.LogError(TimeService.ToSeconds(localBegin, 8 * 3600) + "  " + TimeService.ToSeconds(utcBegin));
    // -28800  0
    // Debug.LogError(DateTime.Now + "  " + DateTime.UtcNow);
    // 2021/9/29 18:17:27  2021/9/29 10:17:27
    // Debug.LogError(TimeService.ToSeconds(DateTime.Now) + "  " + TimeService.ToSeconds(DateTime.UtcNow));
    // 1632939447  1632910647
    // Debug.LogError(TimeService.ToSeconds(DateTime.Now, 8 * 3600) + "  " + TimeService.ToSeconds(DateTime.UtcNow, 0));
    // 1632910647  1632910647
    // Debug.LogError(TimeService.ToDateTime(1632909781, 8 * 3600) + "  " + TimeService.ToDateTime(1632909781, 0));
    // 2021/9/29 18:03:01  2021/9/29 10:03:01
}
