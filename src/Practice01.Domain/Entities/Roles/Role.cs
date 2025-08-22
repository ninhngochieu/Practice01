using Microsoft.AspNetCore.Identity;

namespace Practice01.Domain.Entities;

public class Role : IdentityRole<Guid>, IBaseEntity
{
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public Guid? DeletedBy { get; set; }
}