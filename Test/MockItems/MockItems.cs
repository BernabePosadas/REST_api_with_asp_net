using System;
using TodoApi.Models;

namespace  Test.MockItems
{
    public static class MockItem{
        public static POSItems GenerateMockItem1(){
            POSItems item = new POSItems();
            item.Id = 1;
            item.Name = "PS1";
            item.price = 20;
            item.InStock = 10;
            return item;
        }
        public static POSItems GenerateMockItem2(){
            POSItems item = new POSItems();
            item.Id = 2;
            item.Name = "PS2";
            item.price = 40;
            item.InStock = 5;
            return item;
        }
        public static POSItems GenerateMockItem3(){
            POSItems item = new POSItems();
            item.Id = 3;
            item.Name = "PS3";
            item.price = 100;
            item.InStock = 10;
            return item;
        }
    }
}