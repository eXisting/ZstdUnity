using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Algorithms;
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
        DictionaryTest(Characters(100));
        DictionaryTest(Characters(100 * 10));
        DictionaryTest(Characters(100 * 1000));
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
    
    unsafe void ZstdTest(byte[] src)
    {
        try
        {
            var watch = new System.Diagnostics.Stopwatch();

            var cctx = ZstdWizard.ZSTD_createCCtx();
            var dctx = ZstdWizard.ZSTD_createDCtx();

            var dest = new byte[ZstdWizard.ZSTD_compressBound((nuint)src.Length)];
            var uncompressed = new byte[src.Length];
            
            fixed (byte* dstPtr = dest)
            fixed (byte* srcPtr = src)
            fixed (byte* uncompressedPtr = uncompressed)
            {
                watch.Start();
                var compressedLength = ZstdWizard.ZSTD_compressCCtx(cctx, (IntPtr)dstPtr, (nuint)dest.Length, (IntPtr) srcPtr, (nuint)src.Length,
                    (int)ZstdWizard.ZSTD_cParameter.ZSTD_c_compressionLevel);
                watch.Stop();

                var compressionTook = watch.Elapsed;
                
                watch.Restart();
                var decompressedLength = ZstdWizard.ZSTD_decompressDCtx(dctx, (IntPtr)uncompressedPtr, (nuint) uncompressed.Length, (IntPtr)dstPtr, compressedLength);
                watch.Stop();
                
                var decompressionTook = watch.Elapsed;
                
                Debug.Log($"Compressed to size: {compressedLength} bytes. Decompressed to size: {decompressedLength} bytes. Original {src.Length} bytes");
                Debug.Log($"Data input bytes: {src.Length}. Compression time: {compressionTook.ToString()}ms. Decompression time: {decompressionTook.ToString()}ms");
                label.text = $"{compressedLength} {decompressedLength} {src.Length}";
            }
                
            ZstdWizard.ZSTD_freeCCtx(cctx);
            ZstdWizard.ZSTD_freeDCtx(dctx);
            
        }
        catch (Exception e)
        {
            label.text = e.Message;
            Debug.LogError(e);
        }
    }

    unsafe void DictionaryTest(byte[] src)
    {
        var trainedData = Resources.Load<TextAsset>("sample_dict").bytes;
        //var trainedData = File.ReadAllBytes();
        
        try
        {
            var watch = new System.Diagnostics.Stopwatch();
            
            var dict = ZstdWizard.ZSTD_createCDict(trainedData, (size_t)trainedData.Length, 1);
            var cctx = ZstdWizard.ZSTD_createCCtx();
            var dctx = ZstdWizard.ZSTD_createDCtx();
            var ccctx = ZstdWizard.ZSTD_CCtx_refCDict(cctx, dict);
            var ddctx = ZstdWizard.ZSTD_DCtx_refDDict(dctx, dict);

            var dest = new byte[ZstdWizard.ZSTD_compressBound((nuint)src.Length)];
            var uncompressed = new byte[src.Length];
            fixed (byte* dictPtr = trainedData)
            fixed (byte* dstPtr = dest)
            fixed (byte* srcPtr = src)
            fixed (byte* uncompressedPtr = uncompressed)
            {
                watch.Start();
                var compressedLength = ZstdWizard.ZSTD_compress_usingCDict(cctx, (IntPtr)dstPtr, (size_t)dest.Length, (IntPtr) srcPtr, (size_t)src.Length, (IntPtr)dictPtr);
                watch.Stop();

                var compressionTook = watch.Elapsed;
                
                watch.Restart();
                var decompressedLength = ZstdWizard.ZSTD_compress_usingCDict(dctx, (IntPtr)uncompressedPtr, (size_t) uncompressed.Length, 
                (IntPtr)dstPtr, compressedLength, (IntPtr)dictPtr);
                watch.Stop();
                
                var decompressionTook = watch.Elapsed;
                
                Debug.Log($"Compressed to size: {compressedLength} bytes. Decompressed to size: {decompressedLength} bytes. Original {src.Length} bytes");
                Debug.Log($"Data input bytes: {src.Length}. Compression time: {compressionTook.ToString()}ms. Decompression time: {decompressionTook.ToString()}ms");
                label.text = $"{compressedLength} {decompressedLength} {src.Length}";
            }
                
            ZstdWizard.ZSTD_freeCCtx(cctx);
            ZstdWizard.ZSTD_freeDCtx(dctx);
            
        }
        catch (Exception e)
        {
            label.text = e.Message;
            Debug.LogError(e);
        }
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
        var dictSize = (int)ZstdWizard
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
