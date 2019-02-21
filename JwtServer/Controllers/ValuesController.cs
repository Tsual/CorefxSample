using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly UserDbContext _context;

        public ValuesController(UserDbContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return _context.t_values.Select(t=>t.value).Take(20).ToArray();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return _context.t_values.Find(id).value;
        }

        // POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            var lit=_context.t_values.Find(id) ;
            if (lit == null) BadRequest();
            else
            {
                lit.value = value;
                _context.t_values.Update(lit);
                _context.SaveChanges();
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var lit = _context.t_values.Find(id);
            if (lit == null) BadRequest();
            else
            {
                _context.t_values.Remove(lit);
                _context.SaveChanges();
            }
        }
    }
}
