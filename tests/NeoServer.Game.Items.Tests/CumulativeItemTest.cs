﻿using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Game.Items.Tests
{
    public class CumulativeItemTest
    {
        [Fact]
        public void TryJoin_When_Ids_Are_Not_Same_Returns_False()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sut = new CumulativeItem(type, new Location(100, 100, 7), 50);

            var type2 = new ItemType();
            type.SetClientId(102);
            ICumulativeItem itemToJoin = new CumulativeItem(type2, new Location(100, 100, 7), 40);

            var result = sut.TryJoin(ref itemToJoin);

            Assert.False(result);
            Assert.Equal(40, itemToJoin.Amount);
            Assert.Equal(50, sut.Amount);
        }
        [Fact]
        public void TryJoin_When_Sum_Of_Amount_Less_Than_100_Outs_Null()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sut = new CumulativeItem(type, new Location(100, 100, 7), 50);

            ICumulativeItem itemToJoin = new CumulativeItem(type, new Location(100, 100, 7), 40);
            sut.TryJoin(ref itemToJoin);

            Assert.Null(itemToJoin);
            Assert.Equal(90, sut.Amount);
        }
        [Fact]
        public void TryJoin_When_Sum_Of_Amount_Bigger_Than_100_Outs_Item_With_Exceeding_Amount()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sup = new CumulativeItem(type, new Location(100, 100, 7), 50);

            ICumulativeItem itemToJoin = new CumulativeItem(type, new Location(100, 100, 7), 70);
            sup.TryJoin(ref itemToJoin);

            Assert.Equal(20, itemToJoin.Amount);
            Assert.Equal(100, sup.Amount);
        }
        [Fact]
        public void TryJoin_When_Item_To_Join_Has_Amount_100()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sup = new CumulativeItem(type, new Location(100, 100, 7), 50);

            ICumulativeItem itemToJoin = new CumulativeItem(type, new Location(100, 100, 7), 100);
            sup.TryJoin(ref itemToJoin);

            Assert.Equal(50, itemToJoin.Amount);
            Assert.Equal(100, sup.Amount);
        }
        [Fact]
        public void TryJoin_When_Both_Has_Amount_100()
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sup = new CumulativeItem(type, new Location(100, 100, 7), 100);

            ICumulativeItem itemToJoin = new CumulativeItem(type, new Location(100, 100, 7), 100);
            sup.TryJoin(ref itemToJoin);

            Assert.Equal(100, itemToJoin.Amount);
            Assert.Equal(100, sup.Amount);
        }

        [Theory]
        [InlineData(30, 50, 1500)]
        [InlineData(30, 1, 30)]
        [InlineData(1, 100, 100)]
        [InlineData(10, 10, 100)]
        [InlineData(9.6, 10, 96.0)]
        public void Weight_Returns_Total_Item_Weight(float weight, byte amount, float totalWeight)
        {
            var type = new ItemType();
            type.SetClientId(100);
            type.Attributes.SetAttribute(Enums.ItemAttribute.Weight, weight);


            var sup = new CumulativeItem(type, new Location(100, 100, 7), amount);
            Assert.Equal(totalWeight, sup.Weight);

        }


        [Theory]
        [InlineData(100, 30, 70, 30)]
        [InlineData(100, 100, 0, 100)]
        [InlineData(100, 0, 100, 0)]
        public void Split_Returns_New_Item(byte itemAmount, byte amountToSplit, byte itemRemainingAmount, byte secondItemAmount  )
        {
            var type = new ItemType();
            type.SetClientId(100);

            var sup = new CumulativeItem(type, new Location(100, 100, 7), itemAmount);

            var secondItem = sup.Split(amountToSplit);

            Assert.Equal(itemRemainingAmount, sup.Amount);
            Assert.Equal(secondItemAmount, secondItem.Amount);
        }
    }
}