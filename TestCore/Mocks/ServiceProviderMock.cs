using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xaml;

namespace Tests.Mocks
{
    public class ServiceProviderMock : ITypeDescriptorContext
    {
        ProvideValueTargetMock provideValueTargetMock;
        IXamlTypeResolver xamlTypeResolverMock;
        bool useNullXamlSchemaContextProvider;

        public ServiceProviderMock(object TargetObject, object TargetProperty, Dictionary<string, Type> resolvedTypes)
        {
            provideValueTargetMock = new ProvideValueTargetMock
            {
                TargetObject = TargetObject,
                TargetProperty = TargetProperty
            };

            xamlTypeResolverMock = new XamlTypeResolverMock(resolvedTypes);
        }

        public ServiceProviderMock WithNullXamlSchemaContextProvider()
        {
            useNullXamlSchemaContextProvider = true;
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IXamlTypeResolver))
                return xamlTypeResolverMock;

            if (serviceType == typeof(IProvideValueTarget))
                return provideValueTargetMock;

            if (serviceType == typeof(IXamlSchemaContextProvider) && useNullXamlSchemaContextProvider)
                return null;

            throw new NotSupportedException("test doesn't support type " + serviceType.FullName);
        }

        public IContainer Container
        {
            get { throw new NotImplementedException(); }
        }

        public object Instance
        {
            get { throw new NotImplementedException(); }
        }

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public bool OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class XamlTypeResolverMock: IXamlTypeResolver
    {
        private Dictionary<string, Type> resolvedTypes;

        public XamlTypeResolverMock(Dictionary<string, Type> resolvedTypes)
        {
            this.resolvedTypes = resolvedTypes == null ? new Dictionary<string, Type>() : resolvedTypes;
        }
        public Type Resolve(string qualifiedTypeName)
        {
            Type foundedType = null;
            resolvedTypes.TryGetValue(qualifiedTypeName, out foundedType);

            return foundedType;
        }
    }
}
