namespace MadEngine.Core;

public class PerformanceSettings : ProjectSettings
{
    public int MaxFps { get; set; } = 60;
    
    public override string GetFileName() => "performance";
}