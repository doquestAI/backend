using Domain;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using System;

namespace Infrastructure.Repositories;

internal sealed class PictureRepository(CoreDbContext
     context) : BaseRepository<Picture>(context), IPictureRepository;