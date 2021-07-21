using Amazon.DynamoDBv2.DataModel;

namespace ChipsMMO.Models.Dynamo
{
    [DynamoDBTable("ChipsMMORefreshTokens")]
    public class RefreshTokenInfo
    {
        [DynamoDBHashKey]
        public string Username { get; set; }
        public string RefreshToken { get; set; }
    }
}