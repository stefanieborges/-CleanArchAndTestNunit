using Application.Contracts;
using Application.Dtos;
using Domain.Entities;
using Infrasctruture.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrasctruture.Repository
{
    public class UserRepository : IUser
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private async Task<ApplicationUser> FindUserByEmail(string email) => await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await FindUserByEmail(loginDTO.Email!);
            if (getUser == null) return new LoginResponse(false, "Usuário não encontrado!");
            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Senha, getUser.Senha);
            if (checkPassword)
                return new LoginResponse(true, "Login realizado com sucesso!", GenerateToken(getUser));
            else
                return new LoginResponse(false, "Verifique sua senha e tente novamente");
            
        }

        private string GenerateToken(ApplicationUser getUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaim = new[]
            { 
                new Claim(ClaimTypes.NameIdentifier, getUser.Id.ToString()),
                new Claim(ClaimTypes.Name, getUser.Nome!),
                new Claim(ClaimTypes.Email, getUser.Email!)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaim,
                expires: DateTime.Now.AddDays(5),
                signingCredentials : credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            var getUser = await FindUserByEmail(registerUserDTO.Email!);
            if (getUser != null)
                return new RegistrationResponse(false, "Email já cadastrado");
            _context.Usuarios.Add(new ApplicationUser
            {
                Nome = registerUserDTO.Nome,
                Email = registerUserDTO.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(registerUserDTO.Senha)
            }); 
            await _context.SaveChangesAsync();
            return new RegistrationResponse(true, "Usuário cadatrado com sucesso!");
        }
    }
}
