using System;
using System.Threading.Tasks;

namespace ActivityManagementSystem.BLL
{
    public interface IServices<out T>
    {
        T Service { get; }

        
    }
}
