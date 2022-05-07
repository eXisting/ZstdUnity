// ReSharper disable InconsistentNaming

using System;
using System.Runtime.InteropServices;
using size_t = System.UIntPtr;

namespace Algorithms.ZstdWizard
{
	internal static class Zstd
	{
		private const string DllName = "zstd";

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZDICT_trainFromBuffer(byte[] dictBuffer, size_t dictBufferCapacity, byte[] samplesBuffer, size_t[] samplesSizes, uint nbSamples);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ZSTD_getFrameContentSize(ref byte src, size_t srcSize);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_CCtx_setParameter(IntPtr cctx, ZSTD_cParameter param, int value);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_CCtx_refCDict(IntPtr cctx, IntPtr cdict);
		
#region Compression

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_createCCtx();
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_freeCCtx(IntPtr cctx);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_compressCCtx(IntPtr ctx, ref byte dst, size_t dstCapacity, ref byte src, size_t srcSize, int compressionLevel);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_createCDict(byte[] dict, size_t dictSize, int compressionLevel);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_freeCDict(IntPtr cdict);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_compress_usingCDict(IntPtr cctx, ref byte dst, size_t dstCapacity, ref byte src, size_t srcSize, IntPtr cdict);

#endregion

#region Decompression

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_createDCtx();
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_freeDCtx(IntPtr cctx);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_compressBound(size_t srcSize);

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_decompressDCtx(IntPtr ctx, ref byte dst, size_t dstCapacity, ref byte src, size_t srcSize);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ZSTD_createDDict(byte[] dict, size_t dictSize);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_freeDDict(IntPtr ddict);
		
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern size_t ZSTD_decompress_usingDDict(IntPtr dctx, ref byte dst, size_t dstCapacity, ref byte src, size_t srcSize, IntPtr ddict);
		
#endregion
	}

	public enum ZSTD_cParameter
	{
		ZSTD_c_compressionLevel = 100,
	}
}
