using System;
using System.Collections.Generic;
using size_t = System.UIntPtr;

namespace Algorithms.ZstdWizard
{
	public class CConfig : IDisposable
	{
		public readonly int CompressionLevel;
		public bool HasDictionary => Cdict != IntPtr.Zero;

		public IntPtr Cdict { get; private set; }

		public CConfig(byte[] dict, int compressionLevel = 3)
		{
			CompressionLevel = compressionLevel;

			if (dict == null)
			{
				GC.SuppressFinalize(this);
				return;
			}
			
			Cdict = Zstd.ZSTD_createCDict(dict, (size_t)dict.Length, compressionLevel);
		}

		~CConfig()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool _)
		{
			if (Cdict == IntPtr.Zero)
				return;

			Zstd.ZSTD_freeCDict(Cdict);
			Cdict = IntPtr.Zero;
		}
	}
}
