﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using UAlbion.Game.Events;
using UAlbion.Game.Text;

namespace UAlbion.Game.Gui
{
    public class ContextMenuEvent : GameEvent
    {
        public ContextMenuEvent(Vector2 uiPosition, ITextSource heading, IEnumerable<ContextMenuOption> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            Heading = heading ?? throw new ArgumentNullException(nameof(heading));
            UiPosition = uiPosition;
            Options = new ReadOnlyCollection<ContextMenuOption>(
                options
                    .OrderBy(x => x.Group)
                    .ToList());
        }

        public Vector2 UiPosition { get; }
        public ITextSource Heading { get; }
        public IReadOnlyList<ContextMenuOption> Options { get; }
    }
}