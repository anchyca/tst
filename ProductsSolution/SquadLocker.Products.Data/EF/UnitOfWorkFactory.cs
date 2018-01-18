using System.Data.Entity.Core.EntityClient;

namespace SquadLocker.Products.Data.EF
{
    public class UnitOfWorkFactory
    {
        public static UnitOfWork CreateFromConnectionString(string connectionString)
        {
            var modelFirstConnectionString = CreateModelFirstConnectionString(connectionString);
            return new Data.EF.UnitOfWork(new Data.Entities.ProductsGeneratedContext(modelFirstConnectionString));
        }


        public static string CreateModelFirstConnectionString(string connectionString)
        {
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            entityBuilder.ProviderConnectionString = connectionString;
            entityBuilder.Metadata = @"res://SquadLocker.Products.Data/Entities.ProductsModels.csdl|res://SquadLocker.Products.Data/Entities.ProductsModels.ssdl|res://SquadLocker.Products.Data/Entities.ProductsModels.msl";
            var modelFirstConnectionString = entityBuilder.ToString();
            return modelFirstConnectionString;
        }
    }
}
