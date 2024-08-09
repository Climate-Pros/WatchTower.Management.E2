using ServiceStack;

namespace WatchTower.Management.ServiceModel;

//[ExcludeMetadata]
//[Restrict(InternalOnly = true)]
[Tag("System")]
public class SendEmail : IReturn<EmptyResponse>
{
    public string To { get; set; }
    public string? ToName { get; set; }
    public string Subject { get; set; }
    public string? BodyText { get; set; }
    public string? BodyHtml { get; set; }
}
