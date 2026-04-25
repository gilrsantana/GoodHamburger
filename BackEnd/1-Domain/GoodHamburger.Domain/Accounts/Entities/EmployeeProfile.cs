using Flunt.Validations;

namespace GoodHamburger.Domain.Accounts.Entities;

public class EmployeeProfile: AccountProfile
{
    public string EmployeeCode { get; private set; } = null!;
    public string RoleTitle { get; private set; } = null!;

    protected EmployeeProfile() { }
    
    private EmployeeProfile(
        Guid identityId, 
        string fullName, 
        string displayName, 
        string employeeCode, 
        string roleTitle) 
        : base(identityId, fullName, displayName)
    {
        EmployeeCode = employeeCode;
        RoleTitle = roleTitle;
        Validate();
    }

    public static EmployeeProfile Create(Guid identityId, 
        string fullName, 
        string displayName, 
        string employeeCode, 
        string roleTitle)
    {
        return new EmployeeProfile(identityId, fullName.Trim(), displayName.Trim(), employeeCode.Trim(), roleTitle.Trim());
    }
    
    
    public EmployeeProfile UpdateEmployeeCode(string employeeCode)
    {
        EmployeeCode = employeeCode;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public EmployeeProfile UpdateRoleTitle(string roleTitle)
    {
        RoleTitle = roleTitle;
        Validate();
        if (IsValid)
            UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public sealed override void Validate()
    {
        AddNotifications(new Contract<EmployeeProfile>()
            .Requires()
            .IsNotNullOrEmpty(FullName, "FullName", "Deve informar o nome completo.")
            .IsNotNullOrEmpty(DisplayName, "DisplayName", "Deve informar o nome de exibição.")
            .IsUrlOrEmpty(ProfilePictureUrl, "ProfilePictureUrl", "Deve informar uma URL válida.")
            .AreNotEquals(IdentityId, Guid.Empty, "IdentityId", "Deve informar o ID de Identidade do cliente.")
            .IsNotNullOrEmpty(EmployeeCode, "EmployeeCode", "Deve informar o código do empregado.")
            .IsNotNullOrEmpty(RoleTitle, "RoleTitle", "Deve informar o título do cargo.")
        );
    }
}