using System;
namespace ActivityManagementSystem.CrossCutting.Resources
{
    public interface ISharedResource
    {
        string ErrorCommon { get; }
        string ErrorDbConnect { get; }
        string ErrorInMethod { get; }
        string InfoMethodParams { get; }
        string InfoProcessEnd { get; }
        string InfoProcessStart { get; }
        string InfoParamsReceived { get; }
        string InfoResultReceived { get; }
        string InfoProcessUpdate { get; }
    }
}
