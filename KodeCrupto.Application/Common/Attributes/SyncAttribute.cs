namespace KodeCrypto.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SyncAttribute : Attribute
	{
		public SyncAttribute() { }
	}
}

