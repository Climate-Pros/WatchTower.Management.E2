using System.Runtime.Serialization;

namespace WatchTower.Management.Devices.E3.ServiceModel.Types.GetSession;

[DataContract]
public class Result
{
    [DataMember(Name = "sid")]
    public string SID { get; set; }
}