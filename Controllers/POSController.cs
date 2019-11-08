using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/POSAPI")]
    [ApiController]
    public class POSController : ControllerBase
    {
        private readonly POSContext _context;

        public POSController(POSContext context)
        {
            _context = context;
        }

        // GET: api/POSAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<POSItems>>> GetItems()
        {
            return await _context.Items.ToListAsync();
        }

        // GET: api/POS/5
        [HttpGet("{id}")]
        public async Task<ActionResult<POSItems>> GetPOSItems(long id)
        {
            var pOSItems = await _context.Items.FindAsync(id);

            if (pOSItems == null)
            {
                return NotFound();
            }

            return pOSItems;
        }
        [HttpPut]
        public async Task<IActionResult> PutPOSItems(POSItems pOSItems)
        {
            if (!POSItemsExists(pOSItems.Id))
            {
                BadRequestObjectResult obj = new BadRequestObjectResult("Aww Snap, Entry does not exist. Did you mean to use POST method?");
                return BadRequest(obj);
            }
            _context.Entry(pOSItems).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                    throw;
            }

            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPOSItemsID(long id, POSItems pOSItems)
        {
            if (id != pOSItems.Id)
            {
                return BadRequest();
            }

            _context.Entry(pOSItems).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!POSItemsExists(id))
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

        // POST: api/POS
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<POSItems>> PostPOSItems(POSItems pOSItems)
        {
            _context.Items.Add(pOSItems);
            if(POSItemsExists(pOSItems.Id)){
                BadRequestObjectResult obj = new BadRequestObjectResult("Aww Snap, ID already taken. Did you mean to use PUT method?");
                return BadRequest(obj);
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPOSItems", new { id = pOSItems.Id }, pOSItems);
        }

        // DELETE: api/POS/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<POSItems>> DeletePOSItems(long id)
        {
            var pOSItems = await _context.Items.FindAsync(id);
            if (pOSItems == null)
            {
                return NotFound();
            }

            _context.Items.Remove(pOSItems);
            await _context.SaveChangesAsync();

            return pOSItems;
        }

        private bool POSItemsExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
        private bool POSItemIsValid(POSItems items){
            return true;
        }
    }
}
