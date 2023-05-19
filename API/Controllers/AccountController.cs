using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SQLitePCL;
using API.Interfaces;

namespace API.Controllers;

public class AccountController : BaseAPIController
{
    private readonly DataContext _context;
    
    private readonly ITokenService _tokenService;

    public AccountController(DataContext context, ITokenService tokenService)
    {
            _tokenService = tokenService;
            _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
                var user = await _context.Users.SingleOrDefaultAsync(x =>x.UserName == loginDTO.Username);

                if(user == null) return Unauthorized("Invalid username");

                using var hmac = new HMACSHA512(user.PasswordSalt);

                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

                for(int i=0;i<computedHash.Length;i++){
                    if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
                }
                return new UserDTO
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user) 
                };
        }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO regsiterDTO)
    {
        if(await UserExists(regsiterDTO.Username)) return BadRequest("Username already exists");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
                UserName= regsiterDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regsiterDTO.Password)),
                PasswordSalt = hmac.Key
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
        
    } 
    public async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
