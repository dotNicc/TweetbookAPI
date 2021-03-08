using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TweetBook.Data;
using TweetBook.Domain;
using TweetBook.Options;

namespace TweetBook.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly JwtSettings jwtSettings;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly DataContext dataContext;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, DataContext dataContext)
        {
            this.userManager = userManager;
            this.jwtSettings = jwtSettings;
            this.tokenValidationParameters = tokenValidationParameters;
            this.dataContext = dataContext;
        }
        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await this.userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] {"User with this email address already exists"}
                };
            }
            
            var newUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = email
            };

            var createdUser = await this.userManager.CreateAsync(newUser, password);
            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }
            
            await this.userManager.AddClaimAsync(newUser, new Claim("tags.view", "true"));

            return await GenerateAuthenticationResultForUserAsync(newUser);
        }
        
        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] {"User does not exist."}
                };
            }

            var userHasValidPassword = await this.userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] {"Authentication failed."}
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            ClaimsPrincipal validatedToken = GetPrincipalFromExpiredToken(token);

            if (validatedToken == null) 
                return new AuthenticationResult {Errors = new[] {"Invalid token."}};

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);
            var storedRefreshToken = await this.dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            string jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (expiryDateTimeUtc > DateTime.UtcNow ||
                storedRefreshToken == null ||
                DateTime.UtcNow > storedRefreshToken.ExpiryDate ||
                storedRefreshToken.Invalidated ||
                storedRefreshToken.Used ||
                storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult {Errors = new[] {"Can't refresh this token."}};
            }

            storedRefreshToken.Used = true;
            this.dataContext.Update(storedRefreshToken);
            await this.dataContext.SaveChangesAsync();

            var user = await this.userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var validationParametersMinusValidateLifetime = this.tokenValidationParameters.Clone();
            validationParametersMinusValidateLifetime.ValidateLifetime = false;
            
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParametersMinusValidateLifetime, out SecurityToken validatedToken);
                return !IsJwtWithValidSecurityAlgorithm(validatedToken) ? null : claimsPrincipal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
        
        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var key = Encoding.ASCII.GetBytes(this.jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("id", user.Id)
            };

            var userClaims = await this.userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
            
            var userRoles = await this.userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                
                var role = await this.userManager.FindByNameAsync(userRole);
                if(role == null) 
                    continue;
                
                var roleClaims = await this.userManager.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims)
                {
                    if(claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(this.jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await this.dataContext.RefreshTokens.AddAsync(refreshToken);
            await this.dataContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
    }
}