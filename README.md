# Unity-UGUI-UI

## 进度: 0%
UGUI框架

考虑：
1. 支持引导
2. 支持红点
3. 刘海屏适配， ipad适配
4. 热更新框架， 打包ab, 下载ab, 加载资源，ab的高中低适配，ab的国际化适配
5. https://github.com/Inspiaaa/UnityHFSM 启动,游戏阶段使用状态机控制,方便后期方便调控      
    状态机细化游戏的各个生命阶段  
    比如启动阶段：启动，sdkinit, sdklogin, 热更url获取，热更资源版本比对，热更资源下载，serverlist获取，登录         
    有时候为了简单，比如editor下需要跳过sdk，热更阶段，直达serverlist获取阶段               
    有时候为了调试，比如editor下需要直接登录某个特定的ip地址从而登录游戏，需要跳过sdk, 热更，选服，直达登录阶段，然后直接在某个mono上输入ip地址点击登录即可           
    比如游戏阶段：loading，主城，战斗，对话，cutscene    
