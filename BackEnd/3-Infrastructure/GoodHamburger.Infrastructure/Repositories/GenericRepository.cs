using GoodHamburger.Database.Context;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Shared.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class GenericRepository<T> : BaseRepository<T> where T : Entity
{
    public GenericRepository(ApplicationDbContext context) : base(context)
    {
    }
}
