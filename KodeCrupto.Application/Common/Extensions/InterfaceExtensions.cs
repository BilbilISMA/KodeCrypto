namespace KodeCrypto.Application.Common.Extensions
{
    public static class InterfaceExtensions
	{
        public static IEnumerable<T> GetImplementations<T>(this Type interfaceType) where T : class
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"{interfaceType.Name} is not an interface.");

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .Select(type => (T)Activator.CreateInstance(type))
                .ToList();
        }
    }
}

