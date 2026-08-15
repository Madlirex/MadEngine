using System.Reflection;
using System.Text.RegularExpressions;
using MadEngine.Core;

namespace MadEditor;

public static class StringExtensions
{
    public static string Prettify(this string str)
    {
        if(string.IsNullOrEmpty(str)) return string.Empty;
        
        string result = str.TrimStart('_');
        result = result.Replace('_', ' ');
        
        result = Regex.Replace(result, @"([a-z0-9])([A-Z])", "$1 $2");
        result = Regex.Replace(result, @"([A-Z]+)([A-Z][a-z])", "$1 $2");
        
        result = char.ToUpper(result[0]) + result[1..];
        
        return Regex.Replace(result, @"\s+", " ").Trim();
    }

    public static string GetCustomName(this object obj)
    {
        var type = obj.GetType();
        var attribute = type.GetCustomAttribute<CustomNameAttribute>();
        
        return attribute != null ? attribute.Name : type.Name.Prettify();
    }
    
    public static string GetCustomName(this MemberInfo member)
    {
        var attribute = member.GetCustomAttribute<CustomNameAttribute>();
        
        return attribute != null ? attribute.Name : member.Name.Prettify();
    }

    public static string GetCustomName(this Type type)
    {
        var attribute = type.GetCustomAttribute<CustomNameAttribute>();

        return attribute != null ? attribute.Name : type.Name.Prettify();
    }
}