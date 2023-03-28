using UnityEngine;

public static class ScreenService {
    // 横屏/竖屏
    public static bool PortraitOrLandscape {
        get {
            var cur = Screen.orientation;
            if (cur == ScreenOrientation.Portrait || cur == ScreenOrientation.PortraitUpsideDown) {
                return true;
            }

            return false;
        }
    }
}
