namespace ChipsMMO.Models.Response
{
    public class AccountInfoResponse
    {
        public string Username { get; set; }
        public byte Banned { get; set; }
        public string Email { get; set; }
        public short ELO { get; set; }
        public byte Verified { get; set; }
    }
}
