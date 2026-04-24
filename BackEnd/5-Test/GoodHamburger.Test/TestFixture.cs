using GoodHamburger.Database.Context;
using GoodHamburger.Infrastructure.Repositories.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Goodhamburger.Test;

public class TestFixture : IDisposable
{
    public ApplicationDbContext Context { get; }
    public IngredientRepository IngredientRepository { get; }

    public TestFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
        IngredientRepository = new IngredientRepository(Context);

        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
