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
        private readonly POSContext _context;
        private readonly POSItemValidator _validator;
        private readonly TransactionRequestValidator _cartValidator;
        private readonly TransactionContext _cart;
        public AddingToCartTest()
        {
            this._context = AddingToCartTest.GenerateMockContextData();
            this._validator = new POSItemValidator();
            this._cartValidator = new TransactionRequestValidator();
            this._cart = AddingToCartTest.GenerateTransactionContext();
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
        private static POSContext GenerateMockContextData()
        {
            var options = new DbContextOptionsBuilder<POSContext>()
                    .UseInMemoryDatabase("TodoList").Options;
            POSContext context = new POSContext(options);
            context = AddingToCartTest.AddItem(context, MockItem.GenerateMockItem1());
            context = AddingToCartTest.AddItem(context,MockItem.GenerateMockItem2());
            context = AddingToCartTest.AddItem(context, MockItem.GenerateMockItem3());     
            return context;
        }
        private static TransactionContext GenerateTransactionContext(){
            var options = new DbContextOptionsBuilder<TransactionContext>()
                    .UseInMemoryDatabase("TransactionList").Options;
            TransactionContext context = new TransactionContext(options);
            return context;
        }
        private static POSContext AddItem(POSContext context, POSItems item)
        {
            if(!context.Items.Any(e => e.Id == item.Id)){
                context.Add(item);
                context.SaveChanges();
            }
            return context;
        }
    }
}
