using Domain.Entities.Abstracts;
using Domain.Entities.RelationShip;
using Domain.ValueObjects;
using Flunt.Validations;
using System;

namespace Domain.Entities.Core;


internal class Role : Entity
{
    public UniqueName Name { get; private set; }
    public string Slug { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    private Role() { }

    public Role(UniqueName name, string slug)
    {
        Name = name;
        Slug = slug;

        AddNotifications(name);
        AddNotifications(new Contract<Role>()
            .IsNotNullOrWhiteSpace(Slug, "Slug", "Slug é obrigatório")
        );
    }
}