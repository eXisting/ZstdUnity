using System;
using System.Runtime.InteropServices;
using size_t = System.UIntPtr;

namespace Algorithms
{
    public static class ZstdWizard
    {
        private const string DllName = "zstd";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createCCtx();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint ZSTD_freeCCtx(IntPtr cctx);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint ZSTD_compressCCtx(IntPtr ctx, IntPtr dst, nuint dstCapacity, IntPtr src, nuint srcSize, int compressionLevel);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createDCtx();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint ZSTD_freeDCtx(IntPtr cctx);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint ZSTD_decompressDCtx(IntPtr ctx, IntPtr dst, nuint dstCapacity, IntPtr src,
            nuint srcSize);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint ZSTD_compressBound(nuint srcSize);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint ZSTD_CCtx_setParameter(IntPtr cctx, ZSTD_cParameter param, int value);

#region Dictionary

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZDICT_trainFromBuffer(byte[] dictBuffer, size_t dictBufferCapacity, byte[] samplesBuffer, size_t[] samplesSizes, uint nbSamples);
        
#region Compression

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createCDict(byte[] dict, size_t dictSize, int compressionLevel);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_freeCDict(IntPtr cdict);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_compress_usingCDict(IntPtr cctx, IntPtr dst, size_t dstCapacity, IntPtr src, size_t srcSize, IntPtr cdict);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_CCtx_refCDict(IntPtr cctx, IntPtr cdict);
        
#endregion

#region Decompression

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createDDict(byte[] dict, size_t dictSize);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_freeDDict(IntPtr ddict);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_decompress_usingDDict(IntPtr dctx, IntPtr dst, size_t dstCapacity, IntPtr src, size_t srcSize, IntPtr ddict);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t ZSTD_DCtx_refDDict(IntPtr cctx, IntPtr cdict);
        
#endregion

#endregion
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ZSTD_maxCLevel();
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ZSTD_minCLevel();
        
        public enum ZSTD_cParameter
        {
            ZSTD_c_compressionLevel = 1,
        }
    }
}
