using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Interfaces;

namespace TodoApi.Controllers
{
    [Route("api/POSAPI")]
    [ApiController]
    public class POSController : ControllerBase
    {
        private readonly POSContext _context;
        private readonly IPOSItemValidator _validator;
        private readonly UserProfile _user;
        public POSController(IPOSItemValidator validator, POSContext context)
        {
            this._context = context;
            this._validator = validator;
            this._user = this.CreateUser();
        }

        [HttpGet("Inventory/getItem")]
        public async Task<ActionResult<IEnumerable<POSItems>>> GetItems(long? id)
        {
            if(_user == null || !_user.rights.ViewRights){
                return Unauthorized();
            }
            if (!id.HasValue)
            {
                return await _context.Items.ToListAsync();
            }
            else
            {
                var pOSItems = await _context.Items.FindAsync(id);

                if (pOSItems == null)
                {
                    return NotFound();
                }
                List<POSItems> item = new List<POSItems>();
                item.Add(pOSItems);
                IEnumerable<POSItems> list = item;
                return item;
            }
        }
        [HttpPut("Inventory/UpdateItem")]
        public async Task<IActionResult> PutPOSItems(POSItems pOSItems)
        {
            if(_user == null || !_user.rights.ModifyRights){
                return Unauthorized();
            }
            if (!_validator.ValidateItem(pOSItems))
            {   
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            if (!POSItemsExists(pOSItems.Id))
            {
                 _validator.SetMessage("Aww Snap, Entry does not exist. Are you trying to do a POST method?");
                return BadRequest(new BadRequestObjectResult(_validator));
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
        [HttpPost("Inventory/CreateNewItem")]
        public async Task<ActionResult<POSItems>> PostPOSItems(POSItems pOSItems)
        {
            if(_user == null || !_user.rights.CreateRights){
                return Unauthorized();
            }
            if (!_validator.ValidateItem(pOSItems))
            {
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            if (POSItemsExists(pOSItems.Id))
            {
                _validator.SetMessage("Aww Snap, Entry does not exist. Are you trying to do a PUT method?");
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            _context.Items.Add(pOSItems);
            await _context.SaveChangesAsync();
            return Created(HttpContext.Request.Path, this.Create201ResponseBody(pOSItems));
        }
        [HttpDelete("Inventory/RemoveItem")]
        public async Task<ActionResult<POSItems>> DeletePOSItems(long id)
        {
            if(_user == null || !_user.rights.ModifyRights){
                return Unauthorized();
            }
            var pOSItems = await _context.Items.FindAsync(id);
            if (pOSItems == null)
            {
                return NotFound();
            }

            _context.Items.Remove(pOSItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool POSItemsExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

        // Create a User
        // Currently setting it as a dummy info as there is no way of adding users yet.

        private UserProfile CreateUser(){

            AccessRights rights = new AccessRights();
            rights.ModifyRights = true;
            rights.CreateRights = true;
            rights.ViewRights = true;
            UserProfile user = new UserProfile();
            user.Name = "Dummy User";
            user.rights = rights;
            user.wallet = 10000;
            return user; 
        }
        private Response201Body Create201ResponseBody(POSItems data){

            Response201Body response = new Response201Body();
            response.ExceutedBy = _user.Name;
            response.data = data;
            response.timestamp = DateTime.Now;
            return response;

        }
    }
}
