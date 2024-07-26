using System.Runtime.Serialization;

namespace WatchTowerManagementE2.ServiceInterface.Commands.Types;

[DataContract]
public class Payload
{
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Method { get; set; }

    [DataMember]
    public List<object> Params { get; set; }

    public Payload(int id, string method, List<object>? @params = default)
    {
        @params ??= new List<object>();
        
        Id = id;
        Method = method;
        Params = new List<dynamic>( @params );
    }
}