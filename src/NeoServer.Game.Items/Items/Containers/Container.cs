﻿using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Items.Items
{
    public class Container : MoveableItem, IContainer, IItem
    {
        public event RemoveItem OnItemRemoved;
        public event AddItem OnItemAdded;
        public event UpdateItem OnItemUpdated;
        public byte SlotsUsed { get; private set; }
        public bool IsFull => SlotsUsed >= Capacity;
        public IThing Parent { get; private set; }
        public bool HasParent => Parent != null;
        public byte Capacity => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Capacity);
        public List<IItem> Items { get; }
        public IItem this[int index] => Items[index];

        public Container(IItemType type, Location location) : base(type, location)
        {
            if (!type.Attributes.HasAttribute(Enums.ItemAttribute.Capacity))
            {
                throw new ArgumentException("Capacity missing");
            }

            Items = new List<IItem>(Capacity);
        }

        public bool IsEquiped
        {
            get
            {
                IThing parent = this;
                while (true)
                {
                    if (parent is IContainer container)
                    {
                        parent = container.Parent;
                    }
                    else
                    {
                        break;
                    }
                };

                return parent is IPlayer;
            }
        }

        public void SetParent(IThing thing) => Parent = thing;

        public static bool IsApplicable(IItemType type) => type.Group == Enums.ItemGroup.GroundContainer ||
            type.Attributes.GetAttribute(Enums.ItemAttribute.Type)?.ToLower() == "container";

        public bool GetContainerAt(byte index, out IContainer container)
        {
            container = null;
            if (Items[index] is IContainer)
            {
                container = Items[index] as IContainer;
                return true;
            }

            return false;
        }

        private Result AddItem(IItem item, byte slot)
        {

            if (Capacity <= slot)
            {
                throw new ArgumentOutOfRangeException("Slot is bigger than capacity");
            }

            if (item is ICumulativeItem == false)
            {
                return AddItemToFront(item);

            }

            var cumulativeItem = item as ICumulativeItem;

            int itemToJoinSlot = GetSlotOfFirstItemNotFully(cumulativeItem);

            if (itemToJoinSlot >= 0 && item is ICumulativeItem cumulative)
            {
                //adding to a slot with a different item type

                return TryJoinCumulativeItems(cumulative, (byte)itemToJoinSlot);
            }

            return AddItemToFront(item);
        }

        private int GetSlotOfFirstItemNotFully(ICumulativeItem? cumulativeItem)
        {
            var itemToJoinSlot = -1;
            for (int slotIndex = 0; slotIndex < SlotsUsed; slotIndex++)
            {
                var itemOnSlot = Items[slotIndex];
                if (itemOnSlot.ClientId == cumulativeItem?.ClientId && (itemOnSlot as ICumulativeItem)?.Amount < 100)
                {
                    itemToJoinSlot = slotIndex;
                    break;
                }
            }

            return itemToJoinSlot;
        }

        private Result TryJoinCumulativeItems(ICumulativeItem item, byte itemToJoinSlot)
        {

            var amountToAdd = item.Amount;

            var itemToUpdate = Items[itemToJoinSlot] as ICumulativeItem;

            if (itemToUpdate.Amount == 100)
            {
                return AddItemToFront(item);
            }

            itemToUpdate.TryJoin(ref item);

            OnItemUpdated?.Invoke(itemToJoinSlot, itemToUpdate, (sbyte)(amountToAdd - (item?.Amount ?? 0))); //send update notif of the source item

            if (item != null) //item was joined on first item and remains with a amount
            {
                return AddItemToFront(item);
            }
            return new Result();
        }

        private Result AddItemToFront(IItem item)
        {
            if (SlotsUsed >= Capacity)
            {
                return new Result(InvalidOperation.TooHeavy);
            }

            Items.Insert(0, item);
            SlotsUsed++;

            if (item is IContainer container)
            {
                container.SetParent(this);
                container.OnItemUpdated += OnItemUpdated;
                container.OnItemAdded += OnItemAdded;
                container.OnItemRemoved += OnItemRemoved;
            }

            OnItemAdded?.Invoke(item);

            return new Result();
        }



        private Result AddItemToChild(IItem item, IContainer child)
        {
            if (!item.IsCumulative && child.IsFull)
            {
                return new Result(InvalidOperation.TooHeavy);
            }
            var result = child.TryAddItem(item, (byte)(child.Capacity - 1));

            if (item is IContainer container)
            {
                container.SetParent(child);
            }
            return result;
        }
        public Result TryAddItem(IItem item, byte? slot = null) => TryAddItem(item, slot ?? (byte)(Capacity - 1));
        public Result TryAddItem(IItem item, byte slot)
        {
            var itemOnSlot = Items.ElementAtOrDefault(slot);

            if (itemOnSlot is IContainer container)
            {
                return AddItemToChild(item, container);
            }

            return AddItem(item, slot);
        }

        public void MoveItem(byte fromSlotIndex, byte toSlotIndex, byte amount = 1)
        {
            var item = RemoveItem(fromSlotIndex, amount) as ICumulativeItem;

            var itemOnSlot = Items.ElementAtOrDefault(toSlotIndex);

            if (itemOnSlot?.ClientId == item.ClientId)
            {
                TryJoinCumulativeItems(item, toSlotIndex);
            }
            else
            {
                AddItemToFront(item);
            }
        }
        public Result MoveItem(byte fromSlotIndex, byte toSlotIndex)
        {
            if (GetContainerAt(toSlotIndex, out var container))
            {
                var item = RemoveItem(fromSlotIndex);
                return AddItemToChild(item, container);
            }
            if (fromSlotIndex > toSlotIndex)
            {
                var item = RemoveItem(fromSlotIndex);
                return AddItemToFront(item);
            }
            return new Result();
        }
        public IItem RemoveItem(byte slotIndex)
        {
            var item = Items[slotIndex];

            Items.RemoveAt(slotIndex);

            if (item is IContainer container)
            {
                container.SetParent(null);
                container.OnItemUpdated -= OnItemUpdated;
                container.OnItemAdded -= OnItemAdded;
                container.OnItemRemoved -= OnItemRemoved;
            }

            SlotsUsed--;

            OnItemRemoved?.Invoke(slotIndex, item);
            return item;
        }
        public IItem RemoveItem(byte slotIndex, byte amount)
        {
            var item = Items[slotIndex];

            IItem newItem = null;

            if (item is ICumulativeItem cumulative)
            {
                var amountToReduce = Math.Min(cumulative.Amount, amount);
                cumulative.Reduce(Math.Min(cumulative.Amount, amount));
                newItem = cumulative.Clone(amountToReduce);

                if (cumulative.Amount == 0)
                {
                    RemoveItem(slotIndex);
                    return newItem;
                }
            }

            OnItemUpdated?.Invoke(slotIndex, item, (sbyte)-amount);
            return newItem;
        }
    }
}
