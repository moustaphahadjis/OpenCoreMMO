﻿using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public class MagicalWeaponItem : MoveableItem, IMagicalWeaponItem
    {
        public MagicalWeaponItem(IItemType itemType) : base(itemType)
        {
            //DamageType = itemType.GetAttribute<ushort>(ItemAttribute.Attack);
            //Defense = itemType.GetAttribute<byte>(ItemAttribute.Defense);
            //ElementalDamage = itemType.GetWeaponElementDamage();
            //ExtraDefense = itemType.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);
            //TwoHanded = itemType.GetAttribute(ItemAttribute.BodyPosition) == "two-handed";
            //MinimumLevelRequired = itemType.GetAttribute<ushort>(ItemAttribute.MinimumLevel);
            //Weight = itemType.GetAttribute<float>(ItemAttribute.Weight);
        }

        public float Weight { get; }

        public DamageType DamageType { get; }

        public byte AverageDamage { get; }

        public byte ManaWasting { get; }

        public byte Range { get; }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public ushort MinimumLevelRequired { get; }

        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.WeaponType) == "wand";

    }
}