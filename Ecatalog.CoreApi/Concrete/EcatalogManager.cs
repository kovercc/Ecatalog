using Ecatalog.CoreApi.Abstract;

namespace Ecatalog.CoreApi.Concrete
{
    /// <summary>
    /// Manager for choose implementation of Ecatalog.CoreApi.Abstract.IEcatalogCoreApi
    /// </summary>
    public static class EcatalogManager
    {
        /// <summary>
        /// Get ECatalogInMemory class for implement Ecatalog.CoreApi.Abstract.IEcatalogCoreApi
        /// </summary>
        /// <returns>ECatalogInMemory class</returns>
        public static IEcatalogCoreApi GetCatalogInMamory()
        {
            return new EcatalogInMemory();
        }

        /// <summary>
        /// Get EcatalogInDb class for implement Ecatalog.CoreApi.Abstract.IEcatalogCoreApi
        /// </summary>
        /// <returns>EcatalogInDb class</returns>
        public static IEcatalogCoreApi GetCatalogInDb()
        {
            return new EcatalogInDb();
        }
    }
}
