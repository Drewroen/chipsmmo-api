using Amazon.DynamoDBv2.DataModel;
using ChipsMMO.Models.Misc;

namespace ChipsMMO.Models.Dynamo
{
    [DynamoDBTable("ChipsMMOAccounts")]
    public class AccountInfo
    {
        [DynamoDBHashKey]
        public string Username { get; set; }
        public byte Banned { get; set; }
        public string Email { get; set; }
        public EncryptedPassword Password { get; set; }
        public short ELO { get; set; }
        public byte Verified { get; set; }
    }
}