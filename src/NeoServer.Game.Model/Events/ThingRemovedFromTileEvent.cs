
using NeoServer.Game.Contracts;


namespace NeoServer.Game.Events
{
    public class ThingRemovedFromTileEvent
    {
        public ThingRemovedFromTileEvent(INormalTile tile, byte stackPosition)
        {
            Tile = tile;
            StackPosition = stackPosition;
        }

        public INormalTile Tile { get; }
        public byte StackPosition { get; }

        public string EventId => throw new System.NotImplementedException();

        public uint RequestorId => throw new System.NotImplementedException();

        public string ErrorMessage => throw new System.NotImplementedException();
    }
}