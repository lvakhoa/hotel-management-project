using System.Configuration;

namespace HotelManagement.Model;

internal class UISettings : ConfigurationSection
{
    [ConfigurationProperty("theme", DefaultValue = "Light")]
    public string Theme
    {
        get { return (string)this["theme"]; }
        set { this["theme"] = value; }
    }
}