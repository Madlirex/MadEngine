using System.IO.Compression;
using System.Text;

namespace MadEngine.Core.AssetManagement;

public static class AssetPacker
{
    private const byte ObfuscationKey = 0x5A;

    public static void PackAsset(string jsonText, string targetBinaryFilePath)
    {
        byte[] rawBytes = Encoding.UTF8.GetBytes(jsonText);

        using var outputStream = new FileStream(targetBinaryFilePath, FileMode.Create, FileAccess.Write);
        using var memoryStream = new MemoryStream();
        
        using (var compressionStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            compressionStream.Write(rawBytes, 0, rawBytes.Length);
        }

        byte[] compressedBytes = memoryStream.ToArray();
        
        for (int i = 0; i < compressedBytes.Length; i++)
        {
            compressedBytes[i] ^= ObfuscationKey;
        }
        
        outputStream.Write(compressedBytes, 0, compressedBytes.Length);
    }

    public static string UnpackAsset(string binaryFilePath)
    {
        if (!File.Exists(binaryFilePath))
            throw new FileNotFoundException($"Asset missing: {binaryFilePath}");
        
        byte[] compressedBytes = File.ReadAllBytes(binaryFilePath);
        
        for (int i = 0; i < compressedBytes.Length; i++)
        {
            compressedBytes[i] ^= ObfuscationKey;
        }
        
        using var memoryStream = new MemoryStream(compressedBytes);
        using var decompressionStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var reader = new StreamReader(decompressionStream, Encoding.UTF8);

        return reader.ReadToEnd();
    }
}
