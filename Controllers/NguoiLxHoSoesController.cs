using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ttlaixe.Models;

namespace Ttlaixe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiLxHoSoesController : ControllerBase
    {
        private readonly GplxCsdtContext _context;

        public NguoiLxHoSoesController(GplxCsdtContext context)
        {
            _context = context;
        }

        // GET: api/NguoiLxHoSoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiLxHoSo>>> GetNguoiLxHoSos()
        {
            return await _context.NguoiLxHoSos.ToListAsync();
        }

        // GET: api/NguoiLxHoSoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NguoiLxHoSo>> GetNguoiLxHoSo(string id)
        {
            var nguoiLxHoSo = await _context.NguoiLxHoSos.FindAsync(id);

            if (nguoiLxHoSo == null)
            {
                return NotFound();
            }

            return nguoiLxHoSo;
        }

        // PUT: api/NguoiLxHoSoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNguoiLxHoSo(string id, NguoiLxHoSo nguoiLxHoSo)
        {
            if (id != nguoiLxHoSo.MaDk)
            {
                return BadRequest();
            }

            _context.Entry(nguoiLxHoSo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NguoiLxHoSoExists(id))
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

        // POST: api/NguoiLxHoSoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NguoiLxHoSo>> PostNguoiLxHoSo(NguoiLxHoSo nguoiLxHoSo)
        {
            _context.NguoiLxHoSos.Add(nguoiLxHoSo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NguoiLxHoSoExists(nguoiLxHoSo.MaDk))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNguoiLxHoSo", new { id = nguoiLxHoSo.MaDk }, nguoiLxHoSo);
        }

        // DELETE: api/NguoiLxHoSoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNguoiLxHoSo(string id)
        {
            var nguoiLxHoSo = await _context.NguoiLxHoSos.FindAsync(id);
            if (nguoiLxHoSo == null)
            {
                return NotFound();
            }

            _context.NguoiLxHoSos.Remove(nguoiLxHoSo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NguoiLxHoSoExists(string id)
        {
            return _context.NguoiLxHoSos.Any(e => e.MaDk == id);
        }
    }
}
