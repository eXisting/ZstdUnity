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
    
    private readonly string _cSharTrainedDict = $"{Application.streamingAssetsPath}/dict.txt";

    private void Start()
    {
        Test(RandomCharacterOf(100));
        Test(RandomCharacterOf(100 * 10));
        Test(RandomCharacterOf(100 * 1000));
    }

    private static void Test(byte[] src)
    {
        var watch = new System.Diagnostics.Stopwatch();
        var trainedData = Resources.Load<TextAsset>("dict").bytes;
        
        using var cConfig = new CConfig(trainedData, 5);
        using var compression = new Compression(cConfig);
        
        watch.Start();
        var compressedData = compression.Compress(src);
        watch.Stop();
        
        var compressionTook = watch.Elapsed;

        using var dConfig = new DConfig(trainedData);
        using var decompression = new Decompression(dConfig);
        
        watch.Restart();
        var decompressedData = decompression.Decompress(compressedData);
        watch.Stop();
        
        var decompressionTook = watch.Elapsed;
        
        Debug.Log($"Compressed to size: {compressedData.Length} bytes. Decompressed to size: {decompressedData.Length} bytes. Original {src.Length}bytes");
        Debug.Log($"Data input bytes: {src.Length}. Compression time: {compressionTook.ToString()}ms. Decompression time: {decompressionTook.ToString()}ms");
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
        var dictSize = (int)Zstd.ZDICT_trainFromBuffer(dictBuffer, 
            (size_t)dictCapacity, ms.GetBuffer(), 
            dataSizes, (uint)dataSizes.Length);

        if(dictCapacity != dictSize)
            Array.Resize(ref dictBuffer, dictSize);
        
        Debug.Log($"Dictionary trained: {dictBuffer.Length} bytes");

        // write it for future use
        File.WriteAllBytes(_cSharTrainedDict, dictBuffer);
        
        return dictBuffer;
    }
    
    private byte[] HugeFile() => 
        File.ReadAllBytes($"{Application.streamingAssetsPath}/dickens");

    private byte[] RandomCharacterOf(int size)
    {
        var randomStr = string.Empty;
        for (var i = 0; i < size; i++) 
            randomStr += Random.Range(0, 9).ToString();
        
        return Encoding.UTF8.GetBytes(randomStr);
    }
}
