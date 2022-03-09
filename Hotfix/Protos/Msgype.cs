// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: Msgype.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Logic.Pbf {

  #region Enums
  /// <summary>
  /// 框架层协议号，更换游戏项目依然可以使用
  /// 最大支持65535
  /// </summary>
  public enum MsgType {
    /// <summary>
    /// ----------------------- 框架层协议 ----------------------------
    /// </summary>
    Cslogin = 0,
    Sclogin = 1,
    Cslogout = 2,
    Sclogout = 3,
    CsheartBeat = 4,
    ScheartBeat = 5,
    Csreconnect = 6,
    Screconnect = 7,
    CstimeCorrect = 8,
    SctimeCorrect = 9,
    SctimeNtf = 10,
    SckickOff = 11,
    Csgm = 12,
    Csgms = 13,
    Scgmntf = 14,
    /// <summary>
    /// ----------------------- 逻辑层协议 ----------------------------
    /// sync
    /// </summary>
    CsAddEntity = 1100,
    ScAddEntity = 1101,
    CsRemoveEntity = 1102,
    ScRemoveEntity = 1103,
    CsUpdateEntity = 1104,
    ScUpdateEntity = 1105,
    CsOpEntity = 1106,
    ScOpEntity = 1107,
  }

  #endregion

}

#endregion Designer generated code
