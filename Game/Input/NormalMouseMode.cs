﻿using System.Collections.Generic;
using System.Linq;
using UAlbion.Core;
using UAlbion.Core.Events;
using UAlbion.Game.Events;
using Veldrid;

namespace UAlbion.Game.Input
{
    public class NormalMouseMode : Component
    {
        static readonly HandlerSet Handlers = new HandlerSet(
            H<NormalMouseMode, InputEvent>((x, e) => x.OnInput(e))
        );

        void OnInput(InputEvent e)
        {
            IList<(float, Selection)> hits = new List<(float, Selection)>();
            Raise(new ScreenCoordinateSelectEvent(e.Snapshot.MousePosition, (t, selection) => hits.Add((t, selection))));
            var orderedHits = hits.OrderBy(x => x.Item1).Select(x => x.Item2).ToList();

            if(e.Snapshot.MouseEvents.Any(x => x.MouseButton == MouseButton.Left && x.Down))
            {
                var clickEvent = new UiLeftClickEvent();
                foreach (var hit in orderedHits)
                {
                    if (!clickEvent.Propagating) break;
                    var component = hit.Target as IComponent;
                    component?.Receive(clickEvent, this);
                }
            }

            if(e.Snapshot.MouseEvents.Any(x => x.MouseButton == MouseButton.Left && !x.Down))
            {
                var releaseEvent = new UiLeftReleaseEvent();
                foreach(var hit in orderedHits)
                {
                    if (!releaseEvent.Propagating) break;
                    var component = hit.Target as IComponent;
                    component?.Receive(releaseEvent, this);
                }
            }

            if(e.Snapshot.MouseEvents.Any(x => x.MouseButton == MouseButton.Right && x.Down))
            {
                var clickEvent = new UiRightClickEvent();
                foreach (var hit in orderedHits)
                {
                    if (!clickEvent.Propagating) break;
                    var component = hit.Target as IComponent;
                    component?.Receive(clickEvent, this);
                }
            }

            if(e.Snapshot.MouseEvents.Any(x => x.MouseButton == MouseButton.Right && !x.Down))
            {
                var releaseEvent = new UiRightReleaseEvent();
                foreach(var hit in orderedHits)
                {
                    if (!releaseEvent.Propagating) break;
                    var component = hit.Target as IComponent;
                    component?.Receive(releaseEvent, this);
                }
            }
        }

        public NormalMouseMode() : base(Handlers) { }
    }
}