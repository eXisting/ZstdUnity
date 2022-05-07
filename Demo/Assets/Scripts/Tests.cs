using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Algorithms;
using Algorithms.ZstdWizard;
using TMPro;
using UnityEngine;
using static System.Linq.Enumerable;
using Random = UnityEngine.Random;
using size_t = System.UIntPtr;

public class Tests : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    
    private readonly string _dictPath = $"{Application.streamingAssetsPath}/dict.txt";

    // [DllImport("__Internal")]
    // private static extern int CountLettersInString([MarshalAs(UnmanagedType.LPWStr)]string str);

    void Start()
    {
        Wrapped(Characters(100));
        Wrapped(Characters(100 * 10));
        Wrapped(Characters(100 * 1000));
    }

    // void Lz4DllTest(byte[] src)
    // {
    //     var watch = new System.Diagnostics.Stopwatch();
    //     
    //     watch.Start();
    //     var compressed = lz4.Compress(src, 1);
    //     watch.Stop();
    //     
    //     var compressionTook = watch.Elapsed;
    //     
    //     watch.Restart();
    //     var decompressed = lz4.Decompress(compressed);
    //     watch.Stop();
    //     
    //     var decompressionTook = watch.Elapsed;
    //     
    //     Debug.Log($"Compressed to size: {compressed.Length} bytes. Decompressed to size: {decompressed.Length} bytes. Original {src.Length} bytes");
    //     Debug.Log($"Data input bytes: {src.Length}. Compression time: {compressionTook.ToString()}ms. Decompression time: {decompressionTook.ToString()}ms");
    // }

    // void NativeNet(byte[] src)
    // {
    //     var watch = new System.Diagnostics.Stopwatch();
    //     
    //     var memory = new byte[src.Length];
    //     watch.Start();
    //     var encoded = BrotliEncoder.TryCompress(
    //         src,
    //         memory,
    //         out var compressed
    //     );
    //     watch.Stop();
    //     
    //     var compressionTook = watch.Elapsed;
    //     
    //     var target = new byte[memory.Length];
    //     
    //     watch.Restart();
    //     BrotliDecoder.TryDecompress(memory, target, out var decodedBytes);
    //     watch.Stop();
    //     
    //     var decompressionTook = watch.Elapsed;
    //     
    //     Debug.Log($"Compressed to size: {compressed} bytes. Decompressed to size: {decodedBytes} bytes. Original {src.Length} bytes");
    //     Debug.Log($"Data input bytes: {src.Length}. Compression time: {compressionTook.ToString()}ms. Decompression time: {decompressionTook.ToString()}ms");
    // }
    
    // unsafe void ZstdTest(byte[] src)
    // {
    //     try
    //     {
    //         var watch = new System.Diagnostics.Stopwatch();
    //
    //         var cctx = ZstdWizard.ZSTD_createCCtx();
    //         var dctx = ZstdWizard.ZSTD_createDCtx();
    //
    //         var dest = new byte[ZstdWizard.ZSTD_compressBound((nuint)src.Length)];
    //         var uncompressed = new byte[src.Length];
    //         
    //         fixed (byte* dstPtr = dest)
    //         fixed (byte* srcPtr = src)
    //         fixed (byte* uncompressedPtr = uncompressed)
    //         {
    //             watch.Start();
    //             var compressedLength = ZstdWizard.ZSTD_compressCCtx(cctx, (IntPtr)dstPtr, (nuint)dest.Length, (IntPtr) srcPtr, (nuint)src.Length,
    //                 (int)ZSTD_cParameter.ZSTD_c_compressionLevel);
    //             watch.Stop();
    //
    //             var compressionTook = watch.Elapsed;
    //             
    //             watch.Restart();
    //             var decompressedLength = ZstdWizard.ZSTD_decompressDCtx(dctx, (IntPtr)uncompressedPtr, (nuint) uncompressed.Length, (IntPtr)dstPtr, compressedLength);
    //             watch.Stop();
    //             
    //             var decompressionTook = watch.Elapsed;
    //             
    //             Debug.Log($"Compressed to size: {compressedLength} bytes. Decompressed to size: {decompressedLength} bytes. Original {src.Length} bytes");
    //             Debug.Log($"Data input bytes: {src.Length}. Compression time: {compressionTook.ToString()}ms. Decompression time: {decompressionTook.ToString()}ms");
    //             label.text = $"{compressedLength} {decompressedLength} {src.Length}";
    //         }
    //             
    //         ZstdWizard.ZSTD_freeCCtx(cctx);
    //         ZstdWizard.ZSTD_freeDCtx(dctx);
    //         
    //     }
    //     catch (Exception e)
    //     {
    //         label.text = e.Message;
    //         Debug.LogError(e);
    //     }
    // }

    // unsafe void DictionaryTest(byte[] src)
    // {
    //     var trainedData = Resources.Load<TextAsset>("sample_dict").bytes;
    //     var span = new Span<byte>(trainedData);
    //     try
    //     {
    //         var watch = new System.Diagnostics.Stopwatch();
    //         
    //         var cdict = ZstdWizard.ZSTD_createCDict(trainedData, (size_t)trainedData.Length, 3);
    //         var ddict = ZstdWizard.ZSTD_createDDict(trainedData, (size_t)trainedData.Length);
    //         
    //         var cctx = ZstdWizard.ZSTD_createCCtx();
    //         var dctx = ZstdWizard.ZSTD_createDCtx();
    //         
    //         ZstdWizard.ZSTD_CCtx_refCDict(cctx, cdict);
    //         ZstdWizard.ZSTD_DCtx_refDDict(dctx, ddict);
    //         
    //         var dstCapacity = Math.Min(Consts.MaxByteArrayLength, ZstdWizard.ZSTD_compressBound((nuint)src.Length));
    //         var dst = new byte[dstCapacity];
    //         var uncompressed = new byte[src.Length];
    //         
    //         fixed (byte* srcPtr = src)
    //         fixed (byte* dstPtr = dst)
    //         fixed (byte* uncompressedPtr = uncompressed)
    //         {
    //             watch.Start();
    //             var compressedLength = ZstdWizard.ZSTD_compress_usingCDict(cctx, 
    //                 (IntPtr) dstPtr, (size_t)dst.Length, 
    //                 (IntPtr) srcPtr, (size_t)src.Length, cctx);
    //             watch.Stop();
    //             
    //             var compressionTook = watch.Elapsed;
    //             
    //             watch.Restart();
    //             var decompressedLength = ZstdWizard.ZSTD_compress_usingCDict(dctx, 
    //                 (IntPtr)uncompressedPtr, (size_t) uncompressed.Length, 
    //             (IntPtr)dstPtr, compressedLength, dctx);
    //             watch.Stop();
    //
    //             var r = (size_t)compressedLength;
    //             var rr = (int)r;
    //             var decompressionTook = watch.Elapsed;
    //             
    //             Debug.Log($"Compressed to size: {rr} bytes. Decompressed to size: {decompressedLength.ToUInt32()} bytes. Original {src.Length} bytes");
    //             Debug.Log($"Data input bytes: {src.Length}. Compression time: {compressionTook.ToString()}ms. Decompression time: {decompressionTook.ToString()}ms");
    //             label.text = $"{compressedLength} {decompressedLength} {src.Length}";
    //         }
    //         
    //         ZstdWizard.ZSTD_freeCCtx(cctx);
    //         ZstdWizard.ZSTD_freeDCtx(dctx);
    //         ZstdWizard.ZSTD_freeCDict(cdict);
    //         ZstdWizard.ZSTD_freeDDict(ddict);
    //     }
    //     catch (Exception e)
    //     {
    //         label.text = e.Message;
    //         Debug.LogError(e);
    //     }
    // }

    void Wrapped(byte[] src)
    {
        var watch = new System.Diagnostics.Stopwatch();
        var trainedData = Resources.Load<TextAsset>("dict").bytes;
        using var options = new CConfig(trainedData, compressionLevel: 5);
        using var compressor = new Compression(options);
        
        watch.Start();
        var compressedData = compressor.Compress(src);
        watch.Stop();
        
        var compressionTook = watch.Elapsed;
        Debug.Log($"Compressed to size: {compressedData.Length} bytes. Original {src.Length}bytes");
        Debug.Log($"Data input bytes: {src.Length}. Compression time: {compressionTook.ToString()}ms");
        
        using var dconfig = new DConfig(trainedData);
        using var decompressor = new Decompression(dconfig);
        var decompressedData = decompressor.Decompress(compressedData);
        
        Debug.Log($"Decompressed to size: {decompressedData.Length} bytes. Original {src.Length}bytes");
    }
    
    
    private byte[] TrainDict()
    {
        // zstd default value
        const int dictCapacity = 112640;
        
        // Generate random data set 
        var buffer = Range(0, 1000).Select(x => unchecked((byte)(x * x))).ToArray();
        var data = Range(0, 100)
            .Select(x => buffer.Skip(x).Take(200 - x).ToArray())
            .ToArray();
        
        // Convert into proper type for zstd
        using var ms = new MemoryStream();
        var dataSizes = data.Select(x =>
        {
            ms.Write(x, 0, x.Length);
            return (size_t)x.Length;
        }).ToArray();

        var dictBuffer = new byte[dictCapacity];
        var dictSize = (int)Zstd
            .ZDICT_trainFromBuffer(dictBuffer, (size_t)dictCapacity, ms.GetBuffer(), dataSizes, (uint)dataSizes.Length);

        if(dictCapacity != dictSize)
            Array.Resize(ref dictBuffer, dictSize);
        
        Debug.Log($"Dictionary trained: {dictBuffer.Length} bytes");

        // write it for future use
        File.WriteAllBytes(_dictPath, dictBuffer);
        
        return dictBuffer;
    }
    
    private byte[] HugeFile() => 
        File.ReadAllBytes($"{Application.streamingAssetsPath}/dickens");

    private byte[] Characters(int size)
    {
        var randomStr = string.Empty;
        for (int i = 0; i < size; i++)
        {
            randomStr += Random.Range(0, 9).ToString();
        }
        var src = Encoding.UTF8.GetBytes(randomStr);

        return src;
    }
}
