namespace KodeCrypto.Domain.Entities.Identity
{
	public class UserRole
	{
        public string Name { get; set; }
        public int UserId { get; set; }
        public string AppEntitiesRestrictions { get; set; }
        public string PermissionRestrictions { get; set; }
    }
}

