using System.Runtime.Serialization;

namespace WatchTower.Management.Devices.E3.ServiceModel.Types.GetSystemInventory;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class Ap
{
    [DataMember(Name="appname")]
    public string Appname { get; set; }

    [DataMember(Name="appstatus")]
    public string Appstatus { get; set; }

    [DataMember(Name="apptype")]
    public string Apptype { get; set; }

    [DataMember(Name="category")]
    public string Category { get; set; }

    [DataMember(Name="commissionable")]
    public int Commissionable { get; set; }

    [DataMember(Name="device")]
    public bool Device { get; set; }

    [DataMember(Name="iid")]
    public string Iid { get; set; }

    [DataMember(Name="name")]
    public string Name { get; set; }

    [DataMember(Name="DevAddr")]
    public string DevAddr { get; set; }

    [DataMember(Name="Route")]
    public string Route { get; set; }

    [DataMember(Name="devicetype")]
    public string Devicetype { get; set; }
}