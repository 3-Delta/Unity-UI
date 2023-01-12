using UnityEngine;

namespace Ludiq.PeekCore
{
	[Singleton(Name = "Coroutine Runner", Automatic = true, Persistent = true)]
	[AddComponentMenu("")]
	[DisableAnnotation]
	public sealed class CoroutineRunner : MonoBehaviour, ISingleton
	{
		private void Awake()
		{
			Singleton<CoroutineRunner>.Awake(this);
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			Singleton<CoroutineRunner>.OnDestroy(this);
		}

		public static CoroutineRunner instance => Singleton<CoroutineRunner>.instance;
	}
}