﻿using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public readonly struct MagicFieldItem : IItem
    {
        public MagicFieldItem(IItemType type, Location location)
        {
            Metadata = type;
            Location = location;
            Type = 0;

            Type = ParseFieldType(type.Attributes.GetAttribute(ItemAttribute.Field));
        }

        public Location Location { get; }

        public MagicFieldType Type { get; }

        public IItemType Metadata { get; }

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.Type) == "magicfield";
        
        private MagicFieldType ParseFieldType(string type) => type switch
        {
            "fire" => MagicFieldType.Fire,
            _ => MagicFieldType.None
        };

    }
}
