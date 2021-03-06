﻿using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerOpenedContainerEventHandler
    {
        private readonly Game game;

        public PlayerOpenedContainerEventHandler(Game game)
        {

            this.game = game;
        }
        public void Execute(IPlayer player, byte containerId, NeoServer.Game.Contracts.Items.Types.IContainer container)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new OpenContainerPacket(container, containerId));
                connection.Send();
            }
        }
    }
}
