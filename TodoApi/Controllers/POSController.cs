using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace TodoApi.Controllers
{
    [Route("api/POSAPI")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class POSController : ControllerBase
    {
        private readonly POSContext _context;
        private readonly IPOSItemValidator _validator;
        private readonly ITransactionRequestValidator _cartValidator;
        private readonly UserProfile _user;
        private readonly TransactionContext cart;
        public POSController(IPOSItemValidator validator, ITransactionRequestValidator cartValidator, POSContext context, TransactionContext cart)
        {
            this._context = context;
            this._validator = validator;
            this._cartValidator = cartValidator;
            this.cart = cart;
            this._user = this.CreateUser();
        }

        /// <summary>
        /// Fetches items currently on stock.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET 
        ///     {
        ///        "id": 1
        ///     }
        /// </remarks>
        /// <param name="id">The ID of the requested item, if not given will retrieve all items on stock</param>
        /// <returns>The requested item</returns>
        /// <response code="200">Returns item requested</response>
        /// <response code="401">Returned if request is denied due to account restrictions</response> 
        /// <response code="404">Returned if no item was found</response> 
        [HttpGet("Inventory/getItemInStock")]
        public async Task<ActionResult<IEnumerable<POSItems>>> GetItems(long? id)
        {
            if (_user == null || !_user.rights.ViewRights)
            {
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
        
        /// <summary>
        /// Fetches items currently on cart.
        /// </summary>
        /// <returns>The requested item</returns>
        /// <response code="200">Returns item requested</response>
        /// <response code="401">Returned if request is denied due to account restrictions</response> 
        public async Task<ActionResult<IEnumerable<POSItems>>> GetItemsInCart()
        {
            if (_user == null || !_user.rights.ViewRights)
            {
                return Unauthorized();
            }
            return await _context.Items.ToListAsync();
        }

        /// <summary>
        /// Updates Item in stock.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT 
        ///     {
        ///        "Id": 1,
        ///        "Name" : "PS1",
        ///        "price": 20,
        ///        "InStock": 10
        ///     }
        /// </remarks>
        /// <returns>HTTP 204 response</returns>
        /// <response code="204">Indicates Operation was successfull</response>
        /// <response code="401">Returned if request is denied due to account restrictions</response> 
        /// <response code="400">Returned if there is a problem with details</response> 
        [HttpPut("Inventory/UpdateItem")]
        public async Task<IActionResult> PutPOSItems(POSItems pOSItems)
        {
            if (_user == null || !_user.rights.ModifyRights)
            {
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
            if (await this.UpdateItem(pOSItems))
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Adds item in stock.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST 
        ///     {
        ///        "Id": 1,
        ///        "Name" : "PS1",
        ///        "price": 20,
        ///        "InStock": 10
        ///     }
        /// </remarks>
        /// <returns>HTTP 201 Response Message</returns>
        /// <response code="201">Indicates Operation was successfull</response>
        /// <response code="401">Returned if request is denied due to account restrictions</response> 
        /// <response code="400">Returned if there is a problem with details</response> 
        [HttpPost("Inventory/CreateNewItem")]
        public async Task<ActionResult<POSItems>> PostPOSItems(POSItems pOSItems)
        {
            if (_user == null || !_user.rights.CreateRights)
            {
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

        /// <summary>
        /// Removes item in stock.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE 
        ///     {
        ///        "id": 1
        ///     }
        /// </remarks>
        /// <returns>HTTP 204 response</returns>
        /// <response code="201">Indicates Operation was successfull</response>
        /// <response code="401">Returned if request is denied due to account restrictions</response> 
        /// <response code="400">Returned if there is a problem with details</response> 
        [HttpDelete("Inventory/RemoveItem")]
        public async Task<ActionResult<POSItems>> DeletePOSItems(long id)
        {
            if (_user == null || !_user.rights.ModifyRights)
            {
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
        /// <summary>
        /// Adds item in cart.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST
        ///     {
        ///        "id": 1,
        ///        "Quantity" : 10
        ///     }
        /// </remarks>
        /// <returns>HTTP 204 Response</returns>
        /// <response code="204">Indicates Operation was successfull</response>
        /// <response code="401">Returned if request is denied due to account restrictions</response> 
        /// <response code="400">Returned if there is a problem with details</response> 
        [HttpPost("AddToCart")]
        public async Task<ActionResult<TransactionRequest>> AddToCart(TransactionRequest item)
        {
            if (!this._cartValidator.ValidateItem(item))
            {
                return BadRequest(new BadRequestObjectResult(this._cartValidator));
            }
            if (!this.POSItemsExists(item.Id))
            {
                _validator.SetMessage("Item does not exist.");
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            var response = await this.AddItemToCart(item);
            if (response.GetType() == typeof(string))
            {
                _validator.SetMessage(response);
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            return NoContent();
        }

        /// <summary>
        /// Cart Checkout.
        /// </summary>
        /// <returns>HTTP 204 Response</returns>
        /// <response code="204">Indicates Operation was successfull</response>
        /// <response code="401">Returned if request is denied due to account restrictions</response> 
        /// <response code="400">Returned if there is a problem with details</response> 
        [HttpPost("Checkout")]
        public async Task<ActionResult<TransactionRequest>> Checkout()
        {
            decimal price = 0;
            Receipt receipt = new Receipt();
            var cart = await this.cart.Items.ToListAsync();
            foreach (var cart_item in cart)
            {
                POSItems item_data = await _context.Items.FindAsync(cart_item.Id);
                decimal item_price = item_data.price * cart_item.Quantity;
                price += item_price;
                receipt.purchased_items.Add(this.create_purchase_item_entry(item_price, item_data, cart_item));

            }
            if (this._user.wallet < price)
            {
                _validator.SetMessage("Insufficient Funds.");
                return BadRequest(new BadRequestObjectResult(_validator));
            }
            receipt.total_price = price;
            this._user.wallet -= price;
            receipt.remaining_balance = this._user.wallet;
            return Ok(receipt);
        }
        private PurchasedItem create_purchase_item_entry(decimal price, POSItems item, TransactionRequest request)
        {
            PurchasedItem item_entry = new PurchasedItem();
            item_entry.Id = item.Id;
            item_entry.item_name = item.Name;
            item_entry.Quantity = request.Quantity;
            item_entry.price = price;
            return item_entry;
        }
        private bool POSItemsExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
        private bool TransactionRequestExist(long id)
        {
            return cart.Items.Any(e => e.Id == id);
        }


        // Create a User
        // Currently setting it as a dummy info as there is no way of adding users yet.
        private UserProfile CreateUser()
        {
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
        private Response201Body Create201ResponseBody(POSItems data)
        {
            Response201Body response = new Response201Body();
            response.ExceutedBy = _user.Name;
            response.data = data;
            response.timestamp = DateTime.Now;
            return response;
        }
        private async Task<bool> UpdateItem(POSItems pOSItems)
        {
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

        [NonAction]
        public async Task<dynamic> AddItemToCart(TransactionRequest item)
        {
            if (!POSItemsExists(item.Id))
            {
                return "Item does not exist";
            }
            POSItems entry_item = _context.Items.Find(item.Id);
            int stock = (int)entry_item.InStock - (int)item.Quantity;
            if (stock < 0)
            {
                return "Insufficient Stock";
            }
            entry_item.InStock -= item.Quantity;
            this.UpdateIfExist(item);
            await UpdateItem(entry_item);
            return true;
        }
        [NonAction]
        public async void UpdateIfExist(TransactionRequest item)
        {
            if (this.TransactionRequestExist(item.Id))
            {
                TransactionRequest request_item = cart.Items.Find(item.Id);
                request_item.Quantity += item.Quantity;
                cart.Entry(request_item).State = EntityState.Modified;
                try
                {
                    await cart.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                this.cart.Items.Add(item);
                await this.cart.SaveChangesAsync();
            }
        }
    }
}
