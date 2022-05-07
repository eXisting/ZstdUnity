using System;
using System.Collections.Generic;
using size_t = System.UIntPtr;

namespace Algorithms.ZstdWizard
{
	public class DConfig : IDisposable
	{
		public bool HasDictionary => Ddict != IntPtr.Zero;
		
		public IntPtr Ddict { get; private set; }
		
		public DConfig(byte[] dict)
		{
			if (dict == null)
			{
				GC.SuppressFinalize(this);
				return;
			}
			
			Ddict = Zstd.ZSTD_createDDict(dict, (size_t)dict.Length);
		}

		~DConfig()
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
			if (Ddict == IntPtr.Zero)
				return;

			Zstd.ZSTD_freeDDict(Ddict);
			Ddict = IntPtr.Zero;
		}
	}
}
