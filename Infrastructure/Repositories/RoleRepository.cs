using Domain.Entities.Core;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

internal class RoleRepository(CoreDbContext context) : BaseRepository<Role>(context),
     IRoleRepository;