using api.service.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;

namespace api.service.Security
{
    public class JwtService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public GenerateTokenResult GenerateJwtToken(
            string userName,
            string email,
            string userId,
            bool? isVerified = null,
            string? fullName = null,
            string? refreshToken = null,
            IReadOnlyList<Claim>? usersClaims = null,
            IReadOnlyList<string>? rolesClaims = null,
            IReadOnlyList<string>? permissionsClaims = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User ID claim (subject) cannot be empty.", nameof(userName));

            var now = DateTime.Now;
            var jwtClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, userId),
                new(JwtRegisteredClaimNames.Name, fullName ?? ""),
                new(JwtRegisteredClaimNames.Sub, userId),
                new(JwtRegisteredClaimNames.Sid, userId),
                new(JwtRegisteredClaimNames.UniqueName, userName),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.GivenName, fullName ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(
                    JwtRegisteredClaimNames.Iat,
                    DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                ),new(CustomClaimTypes.RefreshToken, refreshToken ?? ""),
            };

            if (rolesClaims?.Any() is true)
            {
                foreach (var role in rolesClaims)
                    jwtClaims.Add(new Claim(ClaimTypes.Role, role.ToLower(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrWhiteSpace(_jwtOptions.Audience))
                jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience));

            if (permissionsClaims?.Any() is true)
            {
                foreach (var permissionsClaim in permissionsClaims)
                {
                    jwtClaims.Add(
                        new Claim(CustomClaimTypes.Permission, permissionsClaim.ToLower(CultureInfo.InvariantCulture))
                    );
                }
            }

            if (usersClaims?.Any() is true)
                jwtClaims = jwtClaims.Union(usersClaims).ToList();

            Guard.Against.NullOrEmpty(_jwtOptions.SecretKey, nameof(_jwtOptions.SecretKey));

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expireTime = now.AddSeconds(_jwtOptions.TokenLifeTimeSecond == 0 ? 300 : _jwtOptions.TokenLifeTimeSecond);
            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                notBefore: now,
                claims: jwtClaims,
                expires: expireTime,
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new GenerateTokenResult(token, expireTime);
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            Guard.Against.NullOrEmpty(token, nameof(token));
            Guard.Against.NullOrEmpty(_jwtOptions.SecretKey, nameof(_jwtOptions.SecretKey));

            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken
            );

            JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null)
            {
                throw new SecurityTokenException("Invalid access token.");
            }

            return principal;
        }
    }
}
