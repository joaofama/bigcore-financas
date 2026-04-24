namespace Financas.Infrastructure.Configurations
{
    public class MongoDbSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;

        public string ConnectionString =>
            $"mongodb://{User}:{Password}@{Host}:{Port}/?authSource=admin";
    }
}
