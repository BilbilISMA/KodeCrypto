using System.Reflection;

namespace KodeCrypto.Application.Common.Extensions
{
    public static class AttributeExtensions
	{
        public static List<MethodInfo> GetMethodsWithAttribute(this Type type, Type attributeType)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            List<MethodInfo> methodsWithAttribute = methods
                .Where(method => method.GetCustomAttributes(attributeType, false).Any())
                .ToList();

            return methodsWithAttribute;
        }
    }
}

