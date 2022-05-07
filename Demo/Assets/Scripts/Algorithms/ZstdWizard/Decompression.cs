using System;
using System.Runtime.InteropServices;
using size_t = System.UIntPtr;

namespace Algorithms.ZstdWizard
{
	public class Decompression : IDisposable
	{
		private readonly DConfig _config;
		private IntPtr _dctx;
		
		public Decompression(DConfig config)
		{
			_config = config;
			_dctx = Zstd.ZSTD_createDCtx();
		}

		public byte[] Decompress(byte[] data)
		{
			var src = new ReadOnlySpan<byte>(data);
			
			var estimatedSize = EstimatedFrameSizeOf(src);

			var dst = new byte[estimatedSize];
			
			int decompressedSize;
			if (_config.HasDictionary)
				decompressedSize = (int)Zstd.ZSTD_decompressDCtx(_dctx, 
					ref MemoryMarshal.GetReference((Span<byte>)dst), (size_t)dst.Length, 
					ref MemoryMarshal.GetReference(src), (size_t)src.Length);
			else
				decompressedSize = (int)Zstd.ZSTD_decompress_usingDDict(_dctx, 
					ref MemoryMarshal.GetReference((Span<byte>)dst), (size_t)dst.Length, 
					ref MemoryMarshal.GetReference(src), (size_t)src.Length, _config.Ddict);

			if(estimatedSize != (ulong)decompressedSize)
				throw new Exception("Got different decompressed size than estimated");

			return dst;
		}

		private static ulong EstimatedFrameSizeOf(ReadOnlySpan<byte> src)
		{
			var size = Zstd.ZSTD_getFrameContentSize(ref MemoryMarshal.GetReference(src), (size_t)src.Length);
			if (size == unchecked(0UL - 1) || size == unchecked(0UL - 2)) 
				throw new ArgumentException();
			
			return size;
		}
		
		~Decompression()
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
			if(_dctx == IntPtr.Zero)
				return;

			Zstd.ZSTD_freeDCtx(_dctx);
			_dctx = IntPtr.Zero;
		}
	}
}
