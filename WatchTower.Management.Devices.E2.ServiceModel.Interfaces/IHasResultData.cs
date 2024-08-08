namespace WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

public interface IHasResultData
{
}

public interface IHasResultData<T> : IHasResultData
{
    public T Result { get; set; }
}

public interface IHasData<T> 
{
    public T Data { get; set; }
}