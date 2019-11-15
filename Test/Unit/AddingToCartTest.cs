using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TodoApi.Models;
using Test.MockItems;
using Microsoft.EntityFrameworkCore;
using TodoApi.ConcreteImplementations;
using TodoApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Test
{
    public class AddingToCartTest
    {
        private MockItem _itemGenerator;
        private POSContext _context;
        private POSItemValidator _validator;
        private TransactionRequestValidator _cartValidator;
        private TransactionContext _cart;
        public AddingToCartTest()
        {
            this._itemGenerator = new MockItem();
            this._context = this.GenerateMockContextData();
            this._validator = new POSItemValidator();
            this._cartValidator = new TransactionRequestValidator();
            this._cart = this.GenerateTransactionContext();
        }
        [Fact]
        public void TestAddToCart()
        {
            TransactionRequest requestItem = new TransactionRequest();
            requestItem.Id = 1;
            requestItem.Quantity = 10;
            var controller = new POSController(this._validator, this._cartValidator, this._context, this._cart);
            var response = controller.AddItemToCart(requestItem);
            Assert.IsType<bool>(response.Result);
        }
        [Fact]
        public void TestAddToCartExcessQuantity()
        {
            TransactionRequest requestItem = new TransactionRequest();
            requestItem.Id = 2;
            requestItem.Quantity = 10;
            var controller = new POSController(this._validator, this._cartValidator, this._context, this._cart);
            var response = controller.AddItemToCart(requestItem);
            string message = response.Result.ToString();
            Assert.True(message.Equals("Insufficient Stock"));
        }
        [Fact]
        public void TestAddToCartDoesNotExist()
        {
            TransactionRequest requestItem = new TransactionRequest();
            requestItem.Id = 5;
            requestItem.Quantity = 10;
            var controller = new POSController(this._validator, this._cartValidator, this._context, this._cart);
            var response = controller.AddItemToCart(requestItem);
            string message = response.Result.ToString();
            Assert.True(message.Equals("Item does not exist"));
        }
        private POSContext GenerateMockContextData()
        {
            var options = new DbContextOptionsBuilder<POSContext>()
                    .UseInMemoryDatabase("TodoList").Options;
            POSContext context = new POSContext(options);
            context = this.AddItem(context, this._itemGenerator.GenerateMockItem1());
            context = this.AddItem(context, this._itemGenerator.GenerateMockItem2());
            context = this.AddItem(context, this._itemGenerator.GenerateMockItem3());     
            return context;
        }
        private TransactionContext GenerateTransactionContext(){
            var options = new DbContextOptionsBuilder<TransactionContext>()
                    .UseInMemoryDatabase("TransactionList").Options;
            TransactionContext context = new TransactionContext(options);
            return context;
        }
        private POSContext AddItem(POSContext context, POSItems item)
        {
            if(!context.Items.Any(e => e.Id == item.Id)){
                context.Add(item);
                context.SaveChanges();
            }
            return context;
        }
    }
}
