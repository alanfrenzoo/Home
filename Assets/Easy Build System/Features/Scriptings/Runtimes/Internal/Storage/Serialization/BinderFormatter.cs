using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace EasyBuildSystem.Runtimes.Internal.Storage.Serialization
{
    public sealed class BinderFormatter : SerializationBinder
    {
        #region Public Methods

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
            {
                Type TypeAtDeserialized = null;

                assemblyName = Assembly.GetExecutingAssembly().FullName;

                TypeAtDeserialized = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

                return TypeAtDeserialized;
            }

            return null;
        }

        #endregion Public Methods
    }
}