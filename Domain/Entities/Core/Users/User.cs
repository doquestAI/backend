using Domain.Entities.Abstracts;
using Domain.Entities.RelationShip;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Core;

internal class User : Entity
{
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public Address Address { get; private set; }
    public Password Password { get; private set; }
    public bool Active { get; private set; }
    public bool IsActiveCredentials { get; private set; }
    public ActivationToken? TokenActivate { get; private set; }
    [NotMapped]
    public string Token { get; private set; }
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;
    private User() { }

    public User(FullName fullName, Email email, Address address,
        bool active = false, Role? role = null)
    {
        FullName = fullName;
        Email = email;
        Address = address;
        Active = active;
        IsActiveCredentials = false;

        if (role is not null)
        {
            _userRoles.Add(new UserRole(Id, role.Id));
        }

        AddNotificationsFromValueObjects(fullName, email, address);
    }

    public User(Email email, Password password)
    {
        Email = email;
        Password = password;
        AddNotificationsFromValueObjects(email, password);
    }

    public void SetPassword(Password password)
    {
        AddNotificationsFromValueObjects(password);
        Password = password;
    }

    public void UpdatePassword(Password password)
    {
        AddNotificationsFromValueObjects(password);
        Password = password;
    }

    public void AssignToken(string token)
        => Token = token;

    public void SetActivationToken(ActivationToken token)
    {
        if (token == null)
            return;

        TokenActivate = token;
    }

    public void AssignActivate(bool isActivate)
    {
        Active = isActivate;
        IsActiveCredentials = isActivate;
        TokenActivate = null;
    }

    public void AssignRole(Role role)
    {
        if (role == null || _userRoles.Any(x => x.RoleId == role.Id))
            return;

        _userRoles.Add(new UserRole(Id, role.Id));
    }

    public void ClearToken()
        => TokenActivate = null;

    public void Update(
        string? FirstName,
        string? LastName,
        string? Road,
        string? NeighBordHood,
        long? Number,
        string CEP,
        string? Complement,
        bool? Active
    )
    {
        FullName = new FullName(FirstName, LastName);
        Address = new Address(Number, NeighBordHood, Road, Complement, CEP);
        this.Active = Active ?? this.Active;
        AddNotificationsFromValueObjects(FullName, Address);
    }
}
