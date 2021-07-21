using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Threading.Tasks;

namespace ChipsMMO.Services
{
    public class DynamoService
    {
        private readonly DynamoDBContext _dynamoDBContext;

        public DynamoService(AmazonDynamoDBClient dynamo)
        {
            _dynamoDBContext = new DynamoDBContext(dynamo);
        }

        public async Task Upsert<T>(T item)
        {
            await _dynamoDBContext.SaveAsync(item);
        }

        public async Task<T> Get<T>(string hashValue)
        {
            return await _dynamoDBContext.LoadAsync<T>(hashValue);
        }

        public async Task Delete<T>(string hashValue)
        {
            await _dynamoDBContext.DeleteAsync<T>(hashValue);
        }
    }
}