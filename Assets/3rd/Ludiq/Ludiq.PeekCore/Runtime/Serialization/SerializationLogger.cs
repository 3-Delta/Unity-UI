using System;
using System.Threading;
using UnityEngine;
using ILogger = Ludiq.OdinSerializer.ILogger;

namespace Ludiq.PeekCore
{
	public class SerializationLogger : ILogger
	{
		public static SerializationLogger instance { get; } = new SerializationLogger();

		public void LogWarning(string warning)
		{
			Debug.LogWarning(warning);
		}

		public void LogError(string error)
		{
			Debug.LogError(error);
		}

		public void LogException(Exception exception)
		{
			if (exception is ThreadAbortException)
			{
				return;
			}

			Debug.LogException(exception);
		}
	}
}