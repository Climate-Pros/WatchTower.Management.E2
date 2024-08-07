using ServiceStack.DataAnnotations;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.Enums;

public enum Scheme
{
    [Description("http")]
    HTTP,

    [Description("https")]
    HTTPS
}