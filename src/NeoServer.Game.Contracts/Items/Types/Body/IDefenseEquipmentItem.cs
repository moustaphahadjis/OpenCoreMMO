﻿using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Item;
using System.Collections.Immutable;

namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IDefenseEquipmentItem : IBodyEquipmentItem
    {
        ImmutableDictionary<DamageType, byte> DamageProtection { get; }
        ushort DefenseValue => Metadata.Attributes.HasAttribute(ItemAttribute.WeaponDefendValue) ?
            Metadata.Attributes.GetAttribute<byte>(ItemAttribute.WeaponDefendValue) : Metadata.Attributes.GetAttribute<byte>(ItemAttribute.ArmorValue);

    }
}
