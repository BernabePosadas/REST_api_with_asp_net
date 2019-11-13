using System;
using Xunit;
using TodoApi.Models;
using Test.MockItems;
using Microsoft.EntityFrameworkCore;
using TodoApi.ConcreteImplementations;
using TodoApi.Controllers; 
using Microsoft.AspNetCore.Mvc;

namespace Test
{
    public class UnitTest1
    {
        private MockItem _itemGenerator;
        public UnitTest1(){
           this._itemGenerator = new MockItem();
        }
        [Fact]
        public void TestAddItemFails()
        {
              Assert.Equal(true, true);
        }
        private POSContext GenerateMockContextData(){
              var options = new DbContextOptionsBuilder<POSContext>()
                      .UseInMemoryDatabase("TodoList").Options;
              POSContext context = new POSContext(options);
              context.Items.Add(this._itemGenerator.GenerateMockItem2());
              context.Items.Add(this._itemGenerator.GenerateMockItem3());
              return context;
        }
    }
}
