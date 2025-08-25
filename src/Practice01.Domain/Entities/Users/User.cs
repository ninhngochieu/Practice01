using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Practice01.Domain.Entities.Users;

public class User :IdentityUser<Guid>, IBaseEntity
{
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public Guid? DeletedBy { get; set; }
}