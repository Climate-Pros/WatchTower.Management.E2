namespace WatchTowerManagementE2.ServiceModel.Commands.GetMultiExpandedStatus;

public class MultiExpandStatus
    {
        public string Prop { get; set; }
        public string Value { get; set; }
        public bool Alarm { get; set; }
        public bool Notice { get; set; }
        public bool Fail { get; set; }
        public bool Override { get; set; }
        public string OvTime { get; set; }
        public int OvType { get; set; }
        public string EngUnits { get; set; }
        public int DataType { get; set; }
        public string BypassTime { get; set; }
    }