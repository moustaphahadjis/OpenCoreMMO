﻿using NeoServer.Game.Enums.Location;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;

namespace NeoServer.Networking.Packets.Incoming
{
    public class AutoWalkPacket : IncomingPacket
    {
        public byte StepDistance { get; }
        public List<Direction> Steps { get; }

        public AutoWalkPacket(IReadOnlyNetworkMessage message)
        {
            StepDistance = message.GetByte();

            Steps = new List<Direction>(StepDistance);

            for (int i = 0; i < StepDistance; i++)
            {
                Steps.Add(ParseByte(message.GetByte()));
            }
        }

        private Direction ParseByte(byte b)
        {
            switch (b)
            {
                case 1: return Direction.East;
                case 2: return Direction.NorthEast;
                case 3: return Direction.North;
                case 4: return Direction.NorthWest;
                case 5: return Direction.West;
                case 6: return Direction.SouthWest;
                case 7: return Direction.South;
                case 8: return Direction.SouthEast;
                default: throw new ArgumentException("Invalid direction");
            }
        }
    }
}
