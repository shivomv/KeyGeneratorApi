using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeyGenerator.Data;
using KeyGenerator.Models;

namespace KeyGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthsController : ControllerBase
    {
        private readonly KeyGeneratorDBContext _context;

        public UserAuthsController(KeyGeneratorDBContext context)
        {
            _context = context;
        }

        // GET: api/UserAuths
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAuth>>> GetUserAuthentication()
        {
            return await _context.UserAuthentication.ToListAsync();
        }

        // GET: api/UserAuths/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAuth>> GetUserAuth(int id)
        {
            var userAuth = await _context.UserAuthentication.FindAsync(id);

            if (userAuth == null)
            {
                return NotFound();
            }

            return userAuth;
        }

        // PUT: api/UserAuths/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAuth(int id, UserAuth userAuth)
        {
            if (id != userAuth.UserAuthID)
            {
                return BadRequest();
            }

            _context.Entry(userAuth).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAuthExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserAuths
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAuth>> PostUserAuth(UserAuth userAuth)
        {
            _context.UserAuthentication.Add(userAuth);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAuth", new { id = userAuth.UserAuthID }, userAuth);
        }

        // DELETE: api/UserAuths/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAuth(int id)
        {
            var userAuth = await _context.UserAuthentication.FindAsync(id);
            if (userAuth == null)
            {
                return NotFound();
            }

            _context.UserAuthentication.Remove(userAuth);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAuthExists(int id)
        {
            return _context.UserAuthentication.Any(e => e.UserAuthID == id);
        }
    }
}
