using System.Runtime.Serialization;

namespace WatchTower.Management.Devices.Shared.Types;

[DataContract]
public class Payload<TRequest> where TRequest : class
{
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public TRequest Request { get; set; }

    public Payload( TRequest request )
    {
        Request = request;
    }
}