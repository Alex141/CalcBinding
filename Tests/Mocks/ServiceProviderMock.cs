using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Mocks
{
    public class ServiceProviderMock : IServiceProvider
    {
        ProvideValueTargetMock provideValueTargetMock;

        public ServiceProviderMock(object TargetObject, object TargetProperty)
        {
            provideValueTargetMock = new ProvideValueTargetMock
            {
                TargetObject = TargetObject,
                TargetProperty = TargetProperty
            };
        }

        public object GetService(Type serviceType)
        {
            return provideValueTargetMock;
        }
    }
}
