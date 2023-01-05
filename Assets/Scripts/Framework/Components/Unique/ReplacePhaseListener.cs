using UnityEngine;

#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
[DisallowMultipleComponent]
public class ReplacePhaseListener //: ReplaceComponent<Apple.PHASE.PHASEListener>
{ }
#endif