using System.IO;

namespace Ludiq.PeekCore
{
	public static class VersionControlUtility
	{
		// Perforce and other VCS have a lock mechanism that usually
		// only makes the file writable once checked out. However, this
		// is inconvenient for game development projects and Unity 
		// disregards this lock for all its generated files, so we do the same.
		// https://support.ludiq.io/communities/5/topics/1645-x
		public static void Unlock(string path)
		{
			Ensure.That(nameof(path)).IsNotNull(path);

			if (File.Exists(path))
			{
				new FileInfo(path).IsReadOnly = false;
			}
		}
	}
}
