using ServiceStack.DataAnnotations;

namespace WatchTowerManagementE2.ServiceModel.Commands.Enums;

public enum Scheme
{
    [Description("http")]
    HTTP,

    [Description("https")]
    HTTPS
}