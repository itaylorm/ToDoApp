using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthenticationController(IConfiguration config)
    {
        _config = config;
    }

    public record AuthenticationData(string? UserName, string? Password);
    public record UserData(int Id, string UserName, string FirstName, string LastName);

    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
    {
        var user = ValidateCredentials(data);
        if(user is null)
        {
            return Unauthorized();
        }
        string? token = GenerateToken(user);
        if(token is null)
        {
            return Unauthorized();
        }
        return Ok(token);
    }

    private string? GenerateToken(UserData user)
    {
        var securityKey = _config.GetValue<string>("Authentication:SecretKey");
        if(securityKey is not null)
        {
            var symetricKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(securityKey));
            var signingCredentials = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName));
            claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));
            claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));

            var token = new JwtSecurityToken(
                    _config.GetValue<string>("Authentication:Issuer"),
                    _config.GetValue<string>("Authentication:Audience"),
                   claims,
                   DateTime.UtcNow,
                   DateTime.UtcNow.AddMinutes(1),
                   signingCredentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        return null;
    }

    private UserData? ValidateCredentials(AuthenticationData data)
    {
        // NON PRODUCTION CODE - REPLACE THIS WITH A CALL TO AUTH SYSTEM

        if(string.IsNullOrEmpty(data.UserName) || string.IsNullOrEmpty(data.Password))
        {
            return null;
        }
        else if(CompareValues(data.UserName.ToLower(), "taylor") &&
                CompareValues(data.Password, "Test123"))
        {
            return new UserData(1, data.UserName, "Taylor", "Maxwell");
        }
        else if (CompareValues(data.UserName.ToLower(), "marcia") &&
            CompareValues(data.Password, "Test123"))
        {
            return new UserData(2, data.UserName, "Marcia", "Maxwell");
        }
        else if (CompareValues(data.UserName.ToLower(), "kim") &&
            CompareValues(data.Password, "Test123"))
        {
            return new UserData(3, data.UserName, "Kim", "Hull");
        }
        return null;
    }

    private bool CompareValues(string? actual, string? expected)
    {
        if(actual is not null)
        {
            if(expected is not null)
            {
                if(actual.Equals(expected))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
