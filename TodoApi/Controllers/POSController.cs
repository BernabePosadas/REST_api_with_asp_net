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
        public int LastHTTPCode = 200;
        public POSController(IPOSItemValidator validator, POSContext context)
        {
            this._context = context;
            this._validator = validator;
            this._user = this.CreateUser();
        }

        [HttpGet("Inventory/getItem")]
        public async Task<ActionResult<IEnumerable<POSItems>>> GetItems(long? id)
        {
            this.LastHTTPCode = 200;
            if(_user == null || !_user.rights.ViewRights){
                this.LastHTTPCode = 401;
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
                    this.LastHTTPCode = 404;
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
                this.LastHTTPCode = 401;
                return Unauthorized();
            }
            if (!_validator.ValidateItem(pOSItems))
            {   
                this.LastHTTPCode = 400;
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            if (!POSItemsExists(pOSItems.Id))
            {
                this.LastHTTPCode = 400;
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
            this.LastHTTPCode = 204;
            return NoContent();
        }
        [HttpPost("Inventory/CreateNewItem")]
        public async Task<ActionResult<POSItems>> PostPOSItems(POSItems pOSItems)
        {
            this.LastHTTPCode = 200;
            if(_user == null || !_user.rights.CreateRights){
                this.LastHTTPCode = 400;
                return Unauthorized();
            }
            if (!_validator.ValidateItem(pOSItems))
            {
                this.LastHTTPCode = 400;
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            if (POSItemsExists(pOSItems.Id))
            {
                this.LastHTTPCode = 400;
                _validator.SetMessage("Aww Snap, Entry does not exist. Are you trying to do a PUT method?");
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            _context.Items.Add(pOSItems);
            await _context.SaveChangesAsync();
            this.LastHTTPCode = 201;
            return Created(HttpContext.Request.Path, this.Create201ResponseBody(pOSItems));
        }
        [HttpDelete("Inventory/RemoveItem")]
        public async Task<ActionResult<POSItems>> DeletePOSItems(long id)
        {
            this.LastHTTPCode = 200;
            if(_user == null || !_user.rights.ModifyRights){
                this.LastHTTPCode = 401;
                return Unauthorized();
            }
            var pOSItems = await _context.Items.FindAsync(id);
            if (pOSItems == null)
            {
                this.LastHTTPCode = 404;
                return NotFound();
            }

            _context.Items.Remove(pOSItems);
            await _context.SaveChangesAsync();
            this.LastHTTPCode = 204;
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
