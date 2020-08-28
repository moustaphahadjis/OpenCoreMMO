﻿
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.World.Map;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Game.World.Tests
{
    public class PathFinderTest
    {
        [Fact]
        public void Find()
        {
            var sut = new PathFinder();
            sut.Find(new Location(100, 100, 7), new Location(105, 109, 7));
        }
    }
}
