using System;
using System.IO;
using System.Text;

namespace MadEditor;

public class DualWriter : TextWriter
{
    private readonly TextWriter _oldConsoleOut;
    private readonly StreamWriter _fileWriter;

    public DualWriter(TextWriter oldConsoleOut, string logFilePath)
    {
        _oldConsoleOut = oldConsoleOut;
        
        var fileStream = new FileStream(logFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        _fileWriter = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
    }
    
    public override Encoding Encoding => Encoding.UTF8;
    
    public override void Write(char value)
    {
        _oldConsoleOut.Write(value);
        _fileWriter.Write(value);
    }
    
    public override void Write(string? value)
    {
        _oldConsoleOut.Write(value);
        _fileWriter.Write(value);
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fileWriter.Dispose();
        }
        base.Dispose(disposing);
    }
}