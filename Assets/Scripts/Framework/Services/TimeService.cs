using System;

// http://tool.chacuo.net/timeunixtime 浏览器时间戳转换工具
// 时间戳定义是：格林威治时间1970年01月01日00时00分00秒(北京时间1970年01月01日08时00分00秒)起至现在的总秒数，表示为：1970-01-01 00:00:00 UTC。它称为Unix时间(Unix time)、POSIX时间(POSIX time)。从定义可以看到，全球相同时刻，不管你是什么时区，时间戳是一致的，时间戳是不会跟着时区的改变而改变。
// 时区最本质：都是计算当前到1970的一个时间差diff, 只不过在同一时间，比如北京是8点的时候，伦敦是0点，也就是秒数一样，但是时间展示不一样。
// 但是c#在处理datetime.now和datetime.utcnow的时候，居然通过+-时区秒数的方式去实现。
public static class TimeService {
    public static readonly DateTime Utc_Start_Time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static TimeZoneInfo _beiJingTimeZone;
    public static TimeZoneInfo BeiJingTimeZone {
        get {
            if (_beiJingTimeZone == null) {
                _beiJingTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            }

            return _beiJingTimeZone;
        }
        set {
            // 强制清空
            _beiJingTimeZone = null;
        }
    }

    // 返回的是不带时区的秒数,也就是浏览器的秒数
    public static long NowTotalSeconds {
        get {
            return (long)DateTime.UtcNow.Subtract(Utc_Start_Time).TotalSeconds;
        }
    }

    public static bool IsSameDay(ref DateTime x, ref DateTime y) {
        return x.Year == y.Year && x.DayOfYear == y.DayOfYear;
    }

    // xUtcSeconds和yUtcSeconds是浏览器北京时间秒数， timeZoneSeconds是北京时间时区相对于utc时区的偏移秒数即8*3600
    public static bool IsSameDay(long xUnixTimeStamp, long yUnixTimeStamp, long offsetSeconds = 0, long timeZoneSeconds = 0) {
        if (xUnixTimeStamp == yUnixTimeStamp) {
            return true;
        }

        DateTime l = FromUtcToDateTime(xUnixTimeStamp - offsetSeconds, timeZoneSeconds);
        DateTime r = FromUtcToDateTime(yUnixTimeStamp - offsetSeconds, timeZoneSeconds);
        return IsSameDay(ref l, ref r);
    }

    public static bool IsSameWeek(ref DateTime x, ref DateTime y) {
        var l = x.AddDays(-(int)x.DayOfWeek).Date;
        var r = y.AddDays(-(int)y.DayOfWeek).Date;
        return IsSameDay(ref l, ref r);
    }

    public static bool IsSameMonth(long xUnixTimeStamp, long yUnixTimeStamp, long offsetSeconds = 0, long timeZoneSeconds = 0) {
        if (xUnixTimeStamp == yUnixTimeStamp) {
            return true;
        }

        DateTime l = FromUtcToDateTime(xUnixTimeStamp - offsetSeconds, timeZoneSeconds);
        DateTime r = FromUtcToDateTime(yUnixTimeStamp - offsetSeconds, timeZoneSeconds);
        return l.Year == r.Year && l.Month == r.Month;
    }

    public static bool IsSameWeek(long xUnixTimeStamp, long yUnixTimeStamp, long offsetSeconds = 0, long timeZoneSeconds = 0) {
        if (xUnixTimeStamp == yUnixTimeStamp) {
            return true;
        }

        DateTime l = FromUtcToDateTime(xUnixTimeStamp - offsetSeconds, timeZoneSeconds);
        DateTime r = FromUtcToDateTime(yUnixTimeStamp - offsetSeconds, timeZoneSeconds);
        return IsSameWeek(ref l, ref r);
    }

    public static long ToUtcSeconds(DateTime utcDt, long offsetSeconds = 0) {
        if (utcDt.Kind == DateTimeKind.Utc) {
            return (long)utcDt.Subtract(Utc_Start_Time).TotalSeconds - offsetSeconds;
        }
        else if(utcDt.Kind == DateTimeKind.Local) {
            utcDt = TimeZoneInfo.ConvertTimeToUtc(utcDt);
            return (long)utcDt.Subtract(Utc_Start_Time).TotalSeconds - offsetSeconds;
        }
        throw new ArgumentException("utcDt parameter is an invalid kind time");
    }

    // 时间戳 浏览器中的时间秒数是不带TimeZone的
    // 比如https://tool.lu/timestamp/
    // 61s在北京时间就是：01/01/1970 08:01:01
    // 返回值依然是utc时间，如果需要转到本时区，需要ToLocalTime
    // ToLocalTime其实就是添加一些时区偏移的秒数，不是调用一次，就添加一次秒数，因为设置成Local了。所以后面调用不直接返回this

    // server下发的秒数是北京时区 2022-7-31 0:17:7，1659197827s, 这个1659197827其实是不带时区秒数的，另外有个8*3600=28800的时区偏移
    // 转换为 dateTime=FromUtcToDateTime(1659197827, 0).ToLocalTime();
    // 或者 dateTime=FromUtcToDateTime(1659197827, 28800);
    // https://tool.lu/timestamp/
    public static DateTime FromUtcToDateTime(long unixTimeStamp, long offsetSeconds = 0) {
        DateTime targetTime = Utc_Start_Time.AddSeconds(unixTimeStamp + offsetSeconds);
        return targetTime;
        
        /*
            // 北京时间：2022/7/31 4:59:40
            secBefore = 1659214780;
            // 北京时间：2022-07-31 05:00:40
            secAfter = 1659214840;
            // utc时区
            var l = TimeService.FromUtcToDateTime(secBefore, -5*3600);
            var r = TimeService.FromUtcToDateTime(secAfter, -5*3600);
            Debug.LogError(l + "  " + r);
            
            // 当前时区
            l = l.ToLocalTime();
            r = r.ToLocalTime();
            Debug.LogError(l + "  " + r);
            
            // 北京时区
            l = TimeService.FromUtcToDateTime(secBefore, -5*3600 + 8*3600);
            r = TimeService.FromUtcToDateTime(secAfter, -5*3600 + 8*3600);
            Debug.LogError(l + "  " + r);
        */
    }
    
    // TimeSpan其实就是:时间差
    
    // DateTime.SpecifyKind(dt, DateTimeKind.Unspecified); 指定一个时区，然后配合TimeZoneInfo.ConvertTime
    // TimeZoneInfo.ConvertTime是将一个Datetime从原时区转换为目标时区
    // public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)

    // 转换为DateTime的Ticks 参考：TimeSpan.TimeToTicks
    public static long SecondsToTicks(long seconds) {
        return seconds * 10000000L; //1E+07;
    }
}

/*
public class TimeServiceTest : MonoBehaviour {
    void Update() {
        // Debug.LogError(DateTime.Now + " sec:  " + TimeService.ToUtcSeconds(DateTime.Now) + " " + DateTime.UtcNow + "  " + 
        //                TimeService.ToUtcSeconds(DateTime.UtcNow) + "  " +
        //                TimeService.NowTotalSeconds + "  " +
        //                TimeService.ToUtcSeconds(DateTime.Now.ToUniversalTime()) + "  " +
        //                TimeService.ToUtcSeconds(TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        
        // var dtt = DateTime.Now;
        // dtt = DateTime.SpecifyKind(dtt, DateTimeKind.Utc);
        // Debug.LogError(TimeService.ToUtcSeconds(DateTime.Now) + "  " + DateTime.Now + " " + TimeService.ToUtcSeconds(dtt) + "  " + dtt);
        // dtt = DateTime.UtcNow;
        // dtt = DateTime.SpecifyKind(dtt, DateTimeKind.Local);
        // Debug.LogError(TimeService.ToUtcSeconds(DateTime.UtcNow) + "  " + DateTime.UtcNow + " " + TimeService.ToUtcSeconds(dtt) + "  " + dtt);
        
        if (Input.GetKeyDown(KeyCode.A)) {
            /*
            Debug.LogError(TimeService.Utc_Start_Time + "  " + TimeService.Utc_Start_Time.ToLocalTime() + "  " + TimeService.Utc_Start_Time.ToUniversalTime());
            Debug.LogError(TimeService.LocalStartTime + "  " + TimeService.LocalStartTime.ToLocalTime() + "  " + TimeService.LocalStartTime.ToUniversalTime());
            Debug.LogError(DateTime.Now + "  " + DateTime.Now.ToLocalTime() + "  " + DateTime.Now.ToUniversalTime());
            Debug.LogError(DateTime.UtcNow + "  " + DateTime.UtcNow.ToLocalTime() + "  " + DateTime.UtcNow.ToUniversalTime());

            var dt = new DateTime(1970, 1, 1, 0, 1, 1);
            Debug.LogError(dt + "  " + dt.ToLocalTime() + "  " + dt.ToUniversalTime());
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            Debug.LogError("DateTimeKind.Local :" + dt + "  " + dt.ToLocalTime() + "  " + dt.ToUniversalTime());
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            Debug.LogError("DateTimeKind.Utc :" + dt + "  " + dt.ToLocalTime() + "  " + dt.ToUniversalTime());
            
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            
            // 北京时区：China Standard Time
            // 0时区：UTC
            dt = TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            Debug.LogError("TimeZoneInfo.FindSystemTimeZoneById :" + dt + "  " + dt.ToLocalTime() + "  " + dt.ToUniversalTime());
            dt = TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Local);
            Debug.LogError("TimeZoneInfo.Local :" + dt + "  " + dt.ToLocalTime() + "  " + dt.ToUniversalTime());
            dt = TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc);
            Debug.LogError("TimeZoneInfo.Utc :" + dt + "  " + dt.ToLocalTime() + "  " + dt.ToUniversalTime());
            
            Debug.LogError("=============");
            foreach (var one in TimeZoneInfo.GetSystemTimeZones()) {
                Debug.LogError(one + "  |||  " + one.Id + "   |||   " + one.BaseUtcOffset);
            }
            #1#

            // 时间戳 浏览器中的时间秒数是不带TimeZoneOffset的

            /*
            // 北京时间 2022-7-31 0:17:7
            long seconds = 1659197827;
            Debug.LogError(TimeService.FromUtcToDateTime(seconds - 8 * 3600) + "  " + TimeService.FromUtcToDateTime(seconds - 8 * 3600).ToLocalTime());
            Debug.LogError(TimeService.FromUtcToDateTime(seconds) + "  " + TimeService.FromUtcToDateTime(seconds).ToLocalTime());
            
            Debug.LogError(TimeService.FromUtcToDateTime(seconds + 8 * 3600) + "  " + TimeService.FromUtcToDateTime(seconds + 8 * 3600).ToLocalTime());
            #1#

            long seconds = 61;
            var dt = TimeService.FromUtcToDateTime(seconds);
            Debug.LogError(dt + "  " + dt.ToLocalTime() + "  " + TimeService.ToUtcSeconds(dt) + "  " + dt.ToLocalTime().ToLocalTime() + "  " + TimeService.ToUtcSeconds(dt.ToLocalTime()));

            dt = TimeService.FromUtcToDateTime(seconds + 8 * 3600);
            Debug.LogError(dt + "  " + dt.ToLocalTime() + "  " + TimeService.ToUtcSeconds(dt) + "  " + dt.ToLocalTime().ToLocalTime() + "  " + TimeService.ToUtcSeconds(dt.ToLocalTime()));

            dt = TimeService.FromUtcToDateTime(seconds);
            var beijingTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            Debug.LogError(TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc) + "  " + 
                           TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Local) + "  " + 
                           TimeZoneInfo.ConvertTime(dt, beijingTimeZone) + " " + 
                           TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc, beijingTimeZone));
            dt = TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc);
        }
        else if (Input.GetKeyDown(KeyCode.B)) {
            // 北京时间：2022/7/31 4:59:40
            var secBefore = 1659214780;
            // 北京时间：2022-07-31 05:00:40
            var secAfter = 1659214840;

            bool isDay = TimeService.IsSameDay(secBefore, secAfter, 5 * 3600, 8 * 3600);
            Debug.LogError("isDay: " + isDay);
            
            secBefore = 1659214780;
            secAfter = 1659214786;

            isDay = TimeService.IsSameDay(secBefore, secAfter, 5 * 3600, 8 * 3600);
            Debug.LogError("isDay: " + isDay);
            
            secBefore = 1659214840;
            secAfter = 1659214846;

            isDay = TimeService.IsSameDay(secBefore, secAfter, 5 * 3600, 8 * 3600);
            Debug.LogError("isDay: " + isDay);

            // 北京时间：2022/7/31 4:59:40
            secBefore = 1659214780;
            // 北京时间：2022-07-31 05:00:40
            secAfter = 1659214840;
            var l = TimeService.FromUtcToDateTime(secBefore, -5*3600);
            var r = TimeService.FromUtcToDateTime(secAfter, -5*3600);
            Debug.LogError(l + "  " + r);
            
            l = l.ToLocalTime();
            r = r.ToLocalTime();
            Debug.LogError(l + "  " + r);
        }
    }
}
*/
