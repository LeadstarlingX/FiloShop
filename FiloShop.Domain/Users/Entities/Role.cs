namespace FiloShop.Domain.Users.Entities;

public class Role
{
    public static readonly Role Registered  = new (1, Roles.Registered);
    
    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    #region Properties

    public int Id { get; init; }
    public string Name { get; init; }

    #endregion
    
    
    public ICollection<User> Users { get; init; } = new List<User>();
    public ICollection<Permission> Permissions { get; init; } = new List<Permission>();
}