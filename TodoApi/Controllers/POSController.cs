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
        private readonly ITransactionRequestValidator _cartValidator;
        private readonly UserProfile _user;
        private List<TransactionRequest> cart = new List<TransactionRequest>();
        public POSController(IPOSItemValidator validator, ITransactionRequestValidator cartValidator, POSContext context)
        {
            this._context = context;
            this._validator = validator;
            this._cartValidator = cartValidator;
            this._user = this.CreateUser();
        }

        [HttpGet("Inventory/getItemInStock")]
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
                 _validator.SetMessage("Entry does not exist. Are you trying to do a POST method?");
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            if(await this.UpdateItem(pOSItems)){
                return NoContent();
            }
            else{
                return StatusCode(500);
            }
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
                _validator.SetMessage("Entry does not exist. Are you trying to do a PUT method?");
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
        
        [HttpPost("POS/AddToCart")]
        public async Task<ActionResult<TransactionRequest>> AddToCart(TransactionRequest item){
             if(!this._cartValidator.ValidateItem(item)){
                 return BadRequest(new BadRequestObjectResult(this._cartValidator));
             }
             if(!this.POSItemsExists(item.Id)){
                  _validator.SetMessage("Item does not exist.");
                return BadRequest(new BadRequestObjectResult(_validator));
             }
             var response = await this.AddItemToCart(item);
             return NoContent();
        } 

        private bool POSItemsExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
        private bool TransactionRequestExist(long id)
        {
            return cart.Any(e => e.Id == id);
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
        private async Task<bool> UpdateItem(POSItems pOSItems){
            _context.Entry(pOSItems).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return true;
        }

        public async Task<dynamic> AddItemToCart(TransactionRequest item){
            if(!POSItemsExists(item.Id)){
                return "Item does not exist";
            }
            POSItems entry_item = _context.Items.Find(item.Id);
            int stock = (int)entry_item.InStock - (int)item.Quantity;
            if(stock < 0){
                return  "Insufficient Stock";
            }
            entry_item.InStock -= item.Quantity; 
            item = this.UpdateIfExist(item);
            await UpdateItem(entry_item);
            return true;
        }

        public TransactionRequest UpdateIfExist(TransactionRequest item){
             if(this.TransactionRequestExist(item.Id)){
                 TransactionRequest request_item = cart.Find(r => r.Id == item.Id);
                 request_item.Quantity += item.Quantity;
                 return request_item;
             }
             else{
                 return item;
             }
        }
    }
}
