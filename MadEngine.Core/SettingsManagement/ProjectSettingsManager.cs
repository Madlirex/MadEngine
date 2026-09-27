using System.Reflection;
using System.Text.Json;

namespace MadEngine.Core;

public static class ProjectSettingsManager
{
    public static void LoadAllSettings()
    {
        foreach(var settings in ProjectSettingsRegistry.ProjectSettingsList)
            LoadSettings(settings);
    }

    public static void LoadSettings(ProjectSettings settings)
    {
        Type concreteType = settings.GetType();
        string dir = settings.GetFolderPath();
        string name = $"{settings.GetFileName()}.{settings.GetFileExtension()}";
        string filePath = Path.Combine(dir, name);

        if (!File.Exists(filePath))
        {
            SaveSettings(settings);
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);

            
            object? loadedData = JsonSerializer.Deserialize(json, concreteType, SerializerSettings.SerializerOptions);

            if (loadedData == null) return;
            
            foreach (PropertyInfo prop in concreteType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanWrite || !prop.CanRead) continue;
                object? value = prop.GetValue(loadedData);
                prop.SetValue(settings, value);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load {name}: {ex.Message}");
        }
    }

    public static void SaveAllSettings()
    {
        foreach(var settings in ProjectSettingsRegistry.ProjectSettingsList)
            SaveSettings(settings);
    }

    public static void SaveSettings(ProjectSettings settings)
    {
        Type concreteType = settings.GetType();
        string dir = settings.GetFolderPath();
        string name = $"{settings.GetFileName()}.{settings.GetFileExtension()}";
        
        try
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string filePath = Path.Combine(dir, name);
            
            string json = JsonSerializer.Serialize(settings, concreteType, SerializerSettings.SerializerOptions);
            
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save {name}: {ex.Message}");
        }
    }
}