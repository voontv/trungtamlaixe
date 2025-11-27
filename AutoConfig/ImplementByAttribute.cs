using System;

namespace Ttlaixe.AutoConfig
{
    public class ImplementByAttribute : Attribute
    {
        public Type Type { get; }

        public ImplementByAttribute(Type type)
        {
            Type = type;
        }
    }
}