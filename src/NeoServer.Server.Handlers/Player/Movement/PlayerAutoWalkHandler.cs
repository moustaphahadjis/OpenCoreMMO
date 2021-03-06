﻿using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerAutoWalkHandler : PacketHandler
    {
        private readonly Game game;

        public PlayerAutoWalkHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var autoWalk = new AutoWalkPacket(message);

            if (game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature player))
            {
                game.Dispatcher.AddEvent(new Event(() => PlayerWalkCommand.Execute((IPlayer)player, game, autoWalk.Steps.ToArray())));
            }
        }
    }
}
