using System;

namespace Ttlaixe.AutoConfig
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConvertToAttribute : Attribute
    {
        public Type ToType { get; }

        public ConvertToAttribute(Type toType)
        {
            ToType = toType;
        }
    }
}