using System;

namespace TenisApp.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ObservableAttribute : System.Attribute
    {
    }
}