using ChipsMMO.Models.Dynamo;
using ChipsMMO.Models.Exceptions;
using ChipsMMO.Models.Misc;
using ChipsMMO.Models.Request;
using ChipsMMO.Models.Response;
using System.Threading.Tasks;

namespace ChipsMMO.Services
{
    public class AccountService
    {
        private readonly DynamoService _dynamoService;
        private readonly EncryptionService _encryptionService;
        private readonly TokenService _tokenService;

        public AccountService(DynamoService dynamoService, EncryptionService encryptionService, TokenService tokenService)
        {
            _dynamoService = dynamoService;
            _encryptionService = encryptionService;
            _tokenService = tokenService;
        }

        public async Task CreateAccount(CreateAccountRequest request)
        {
            if (!await UsernameExists(request.Username))
            {
                var item = new AccountInfo
                {
                    Username = request.Username,
                    Banned = 0,
                    ELO = 1000,
                    Email = request.Email,
                    Password = _encryptionService.Encrypt(request.Password),
                    Verified = 0
                };

                await _dynamoService.Upsert(item);
            } else
            {
                throw new AccountAlreadyExists();
            } 
        }

        public async Task<TokenResponse> Login(LoginRequest loginRequest)
        {
            if (await LoginInfoIsValid(loginRequest.Username, loginRequest.Password))
            {
                string accessToken = _tokenService.GenerateJSONAccessToken(loginRequest.Username);
                string refreshToken = _tokenService.GenerateJSONRefreshToken(loginRequest.Username);

                var newRefreshTokenInfo = new RefreshTokenInfo()
                {
                    Username = loginRequest.Username,
                    RefreshToken = refreshToken
                };
                await _dynamoService.Upsert(newRefreshTokenInfo);

                return new TokenResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };  
            } else {
                throw new InvalidLoginCredentials();
            }
        }

        public async Task<AccountInfoResponse> GetAccountInfo(string username)
        {
            var item = await _dynamoService.Get<AccountInfo>(username);
            if (item == null)
                throw new AccountNotFound();

            return new AccountInfoResponse()
            {
                Banned = item.Banned,
                ELO = item.ELO,
                Email = item.Email,
                Username = item.Username,
                Verified = item.Verified
            };
        }

        public async Task Logout(string username)
        {
            await _dynamoService.Delete<RefreshTokenInfo>(username);
        }

        public async Task<TokenResponse> UpdateAccessToken(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = refreshTokenRequest.RefreshToken;
            if (_tokenService.IsRefreshTokenValid(refreshToken))
            {
                var username = _tokenService.GetUserNameFromRefreshToken(refreshToken);
                var refreshTokenInfo = await _dynamoService.Get<RefreshTokenInfo>(username);

                if (refreshTokenRequest.RefreshToken == refreshTokenInfo?.RefreshToken)
                {
                    return new TokenResponse()
                    {
                        AccessToken = _tokenService.GenerateJSONAccessToken(username),
                        RefreshToken = refreshTokenRequest.RefreshToken
                    };
                }
                else
                {
                    throw new RefreshTokenDoesNotMatch();
                }
            }
            else
            {
                throw new RefreshTokenInvalid();
            }
        }

        private async Task<bool> UsernameExists(string username)
        {
            return (await _dynamoService.Get<AccountInfo>(username)) != null;
        }

        private async Task<bool> LoginInfoIsValid(string username, string password)
        {
            var accountInfo = await _dynamoService.Get<AccountInfo>(username);

            EncryptedPassword encryptedPassword = _encryptionService.Encrypt(password);

            return (encryptedPassword.EncryptedData == accountInfo?.Password?.EncryptedData) && (encryptedPassword.IV == accountInfo?.Password?.IV);
        }
    }
}