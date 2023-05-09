using System.Collections.Generic;
using System.Linq;

namespace DemoApp
{
    public static class BaseSiteServices
    {
        private static HashSet<BaseSite> BaseSites { get; set; } = new();

        public static T FindBaseSiteByID<T>(int id) where T : BaseSite => (T)BaseSites.SingleOrDefault(item => item.ID == id && item is T);

        public static HashSet<T> FindAllBaseSitesType<T>() where T : BaseSite => new(BaseSites.OfType<T>());

        public static void AddBaseSite(BaseSite baseSite) => BaseSites.Add(baseSite);

        public static void Instance()
        {
            BaseSites = new();
        }

        public static bool IsImported => BaseSites.Count > 0;
    }
}
