﻿using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerCancelAutoWalkHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IMap map;



        public PlayerCancelAutoWalkHandler(Game game, IMap map)
        {
            this.game = game;
            this.map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var autoWalk = new AutoWalkPacket(message);

            if (game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature player))
            {

                game.Dispatcher.AddEvent(new Event(player.StopWalking));
            }
        }
    }
}
