using System;
using System.Linq.Expressions;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class PlatformUtility
	{
		public static readonly bool supportsJit;

		static PlatformUtility()
		{
			supportsJit = CheckJitSupport();
		}

		private static bool CheckJitSupport()
		{
			// Temporary hotfix
			// https://forum.unity.com/threads/is-jit-no-longer-supported-on-standalone-mono.671572/
			// https://support.ludiq.io/communities/5/topics/3129-bolt-143-runtime-broken
			return Application.platform.IsEditor();
		}
		
		public static bool IsEditor(this RuntimePlatform platform)
		{
			return
				platform == RuntimePlatform.WindowsEditor ||
				platform == RuntimePlatform.OSXEditor ||
				platform == RuntimePlatform.LinuxEditor;
		}
		
		public static bool IsStandalone(this RuntimePlatform platform)
		{
			return
				platform == RuntimePlatform.WindowsPlayer ||
				platform == RuntimePlatform.OSXPlayer ||
				platform == RuntimePlatform.LinuxPlayer;
		}
	}
}