﻿using System;
using System.Collections.Generic;
using System.Numerics;
using UAlbion.Core;
using UAlbion.Core.Events;
using UAlbion.Core.Visual;
using UAlbion.Formats.AssetIds;
using UAlbion.Formats.Parsers;
using UAlbion.Game.Events;

namespace UAlbion.Game.Entities
{
    public class LargeNpcSprite : Component
    {
        public enum Animation
        {
            WalkN,
            WalkE,
            WalkS,
            WalkW,
            SitN,
            SitE,
            SitS,
            SitW,
            UpperBody
        }

        static readonly IDictionary<Animation, int[]> Frames = new Dictionary<Animation, int[]>
        {
            { Animation.WalkN, new[] { 0,1,2 } },
            { Animation.WalkE, new[] { 3,4,5 } },
            { Animation.WalkS, new[] { 6,7,8 } },
            { Animation.WalkW, new[] { 9,10,11 } },
            { Animation.SitN,  new[] { 12 } },
            { Animation.SitE,  new[] { 13 } },
            { Animation.SitS,  new[] { 14 } },
            { Animation.SitW,  new[] { 15 } },
            { Animation.UpperBody, new[] { 16 } },
        };

        static readonly IList<Handler> Handlers = new Handler[]
        {
            new Handler<LargeNpcSprite, RenderEvent>((x,e) => x.Render(e)),
            new Handler<LargeNpcSprite, WorldCoordinateSelectEvent>((x, e) => x.Select(e)),
            new Handler<LargeNpcSprite, SetTileSizeEvent>((x,e) => x._tileSize = new Vector2(e.TileSize.X, e.TileSize.Y))
        };

        readonly LargeNpcId _id;
        readonly MapNpc.Waypoint[] _waypoints;
        Vector2 _position;
        Vector2 _tileSize;
        Vector2 _size = Vector2.One;
        Animation _animation;
        int _frame;
        public Vector3 Normal => Vector3.UnitZ;

        public LargeNpcSprite(LargeNpcId id, MapNpc.Waypoint[] waypoints, Assets assets) : base(Handlers)
        {
            _id = id;
            _waypoints = waypoints;
            _position = new Vector2(waypoints[0].X, waypoints[0].Y);
            var texture = assets.LoadTexture(_id);
            if (texture != null)
            {
                texture.GetSubImageDetails(_frame, out var size, out _, out _, out _);
                _size = size; // TODO: Update to handle variable sized sprites
            }
        }

        void Select(WorldCoordinateSelectEvent e)
        {
            float denominator = Vector3.Dot(Normal, e.Direction);
            if (Math.Abs(denominator) < 0.00001f)
                return;

            var pixelPosition = _position * _tileSize;
            float t = Vector3.Dot(new Vector3(pixelPosition, 0.0f) - e.Origin, Normal) / denominator;
            if (t < 0)
                return;

            var intersectionPoint = e.Origin + t * e.Direction;
            int x = (int)(intersectionPoint.X - pixelPosition.X);
            int y = (int)(intersectionPoint.Y - pixelPosition.Y);

            if (x < 0 || x >= _size.X ||
                y < 0 || y >= _size.Y)
                return;

            e.RegisterHit(t, $"NPC {_id}", this);
        }

        void Render(RenderEvent e)
        {
            var positionLayered = new Vector3(_position * _tileSize, DrawLayer.Characters1.ToZCoordinate(_position.Y));
            var npcSprite = new SpriteDefinition<LargeNpcId>(
                _id,
                0,
                positionLayered,
                (int)DrawLayer.Characters1,
                0);

            e.Add(npcSprite);
        }
    }
}