using System;
using UnityEngine;
using System.Buffers;
using System.Runtime.InteropServices;
using size_t = System.UIntPtr;

namespace Algorithms.ZstdWizard
{
	internal sealed class Compression : IDisposable
	{
		private readonly CConfig _config;

		private IntPtr _cctx;

		public Compression(CConfig config)
		{
			_config = config;
			_cctx = Zstd.ZSTD_createCCtx();
			
			Zstd.ZSTD_CCtx_setParameter(_cctx, ZSTD_cParameter.ZSTD_c_compressionLevel, _config.CompressionLevel);
			
			if (_config.DictionaryInUse)
				Zstd.ZSTD_CCtx_refCDict(_cctx, _config.Cdict);
		}
		
		public byte[] Compress(byte[] bytes)
		{
			var src = new ReadOnlySpan<byte>(bytes);
			var shared = ArrayPool<byte>.Shared;

			var compressionBound = (ulong)Zstd.ZSTD_compressBound((size_t)src.Length);
			var dst = shared.Rent((int)Math.Min(0x7FFFFFC7, compressionBound));

			try
			{
				var dstSize = CppCompression(src, new Span<byte>(dst));

				var result = new byte[dstSize];
				Array.Copy(dst, result, dstSize);
				return result;
			}
			catch (Exception e)
			{
				Debug.LogError($"Error during compression: {e}");
				shared.Return(dst);
			}
			finally
			{
				shared.Return(dst);
			}
			
			return dst;
		}
		
		private int CppCompression(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			UIntPtr dstSize;

			if (_config.DictionaryInUse)
				dstSize = Zstd.ZSTD_compress_usingCDict(_cctx, 
					ref MemoryMarshal.GetReference(dst), (size_t)dst.Length, 
					ref MemoryMarshal.GetReference(src), (size_t)src.Length, _config.Cdict);
			else
				dstSize = Zstd.ZSTD_compressCCtx(_cctx, 
					ref MemoryMarshal.GetReference(dst), (size_t)dst.Length,
					ref MemoryMarshal.GetReference(src), (size_t)src.Length, _config.CompressionLevel);

			return (int)dstSize;
		}
		
		~Compression()
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
			if (_cctx == IntPtr.Zero)
				return;

			Zstd.ZSTD_freeCCtx(_cctx);
			_cctx = IntPtr.Zero;
		}
	}
}
