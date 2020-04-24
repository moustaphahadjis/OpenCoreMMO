﻿using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IBodyEquipmentItem : IMoveableThing, IPickupableItem
    {
        bool Pickupable => true;

        ImmutableHashSet<VocationType> AllowedVocations { get; }
        ushort MinimumLevelRequired { get; }
        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }

    }
}