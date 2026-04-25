using GoodHamburger.Database.Context;
using GoodHamburger.Shared.Entities.Base;

namespace GoodHamburger.Infrastructure.Repositories;

public class GenericRepository<T> : BaseRepository<T> where T : Entity
{
    public GenericRepository(ApplicationDbContext context) : base(context)
    {
    }
}
