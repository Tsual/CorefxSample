using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtServer.Data;
using JwtServer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly UserDbContext _context;

        public JwtController(UserDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody]JwtModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var res = (from t in _context.t_users where t.Uid == viewModel.User && t.Pwd == viewModel.Password select t).ToList();
                if (res.Count != 1) return BadRequest();

                return Ok(new { token = JwtToken(res[0].Uid) });
            }
            return BadRequest();
        }

        [HttpPost("regist")]
        public IActionResult Regist([FromBody]JwtModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var res = (from t in _context.t_users where t.Uid == viewModel.User && t.Pwd == viewModel.Password select t).ToList();
                if (res.Count != 0) return BadRequest();
                var user = new Data.User { Uid = viewModel.User, Pwd = viewModel.Password };
                _context.t_users.Add(user);
                _context.SaveChanges();
                return Ok(new { token = JwtToken(user.Uid) });
            }
            return BadRequest();
        }

        static string JwtToken(string audience)
        {
            var token = new JwtSecurityToken(issuer: JwtSetting.Issuer, audience: audience,
                notBefore: DateTime.Now, expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(JwtSetting.SecretKey, SecurityAlgorithms.HmacSha256));
            JwtCache.TryPushUid(audience, DateTime.Now.AddMinutes(30));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}