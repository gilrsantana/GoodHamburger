using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Domain.Accounts.Entities;

public abstract class AccountProfile : Entity
{
    public Guid IdentityId { get; private set; }
    public string FullName { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string? ProfilePictureUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    protected AccountProfile() { }

    protected AccountProfile(Guid identityId, string fullName, string displayName, string? profilePictureUrl = null)
    {
        IdentityId = identityId;
        FullName = fullName;
        DisplayName = displayName;
        ProfilePictureUrl = profilePictureUrl;
    }

    protected void UpdateProfilePicture(string url) => ProfilePictureUrl = url;
    protected void UpdateFullName(string fullName) => FullName = fullName.Trim();
    protected void UpdateDisplayName(string displayName) => DisplayName = displayName;
    protected void ToggleActivate() => IsActive = !IsActive;
}