using System;
using ActivityManagementSystem.DAL;
using Microsoft.Extensions.DependencyInjection;
using ActivityManagementSystem.Domain.AppSettings;

namespace ActivityManagementSystem.BLL
{
    public class Services<T> : IServices<T>
    {
        private readonly AppSettings _appsettings;
        public Services(AppSettings appsettings)
        {
            _appsettings = appsettings;
        }

        public T Service
        {
            get
            {
                var serviceProvider = new ServiceCollection()
                   .AddSingleton(typeof(IDatabase<>), typeof(Database<>))
                    .AddSingleton(_appsettings)
                    .AddSingleton(typeof(T))
                    .BuildServiceProvider();
                return (T)Convert.ChangeType(serviceProvider
                    .GetService<T>(), typeof(T));
            }

        }
    }
}
