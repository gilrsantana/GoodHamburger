using GoodHamburger.Domain.Catalog.Enums;

namespace GoodHamburger.Application.Services.Ordering.Helper;

public class HashSetEqualityComparer : IEqualityComparer<HashSet<MenuCategoryType>>
{
    public bool Equals(HashSet<MenuCategoryType>? x, HashSet<MenuCategoryType>? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        
        return x.SetEquals(y);
    }

    public int GetHashCode(HashSet<MenuCategoryType> obj)
    {
        if (obj == null) return 0;
        
        int hash = 17;
        foreach (var item in obj.OrderBy(x => x))
        {
            hash = hash * 31 + item.GetHashCode();
        }
        return hash;
    }
}