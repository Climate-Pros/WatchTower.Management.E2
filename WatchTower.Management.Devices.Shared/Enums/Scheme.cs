using ServiceStack.DataAnnotations;

namespace WatchTower.Management.Devices.Shared.Enums;

public enum Scheme
{
    [Description("http")]
    HTTP,

    [Description("https")]
    HTTPS
}