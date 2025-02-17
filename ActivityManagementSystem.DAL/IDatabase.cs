using System;
namespace ActivityManagementSystem.DAL
{
    public interface IDatabase<out T> : IDisposable
    {
        T Repository { get; }
    }
}
