﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UAlbion.Core;
using UAlbion.Formats.AssetIds;
using UAlbion.Game.Entities;
using UAlbion.Game.Events;
using UAlbion.Game.Text;

namespace UAlbion.Game.Gui.Text
{
    /// <summary>
    /// A UI element containing a single TextBlock (i.e. a block of text sharing the same formatting options)
    /// </summary>
    public class TextBlockElement : UiElement
    {
        static readonly HandlerSet Handlers = new HandlerSet(
            H<TextBlockElement, SetLanguageEvent>((x,e) => x._lastVersion = 0) // Force a rebuild on next render
        );

        readonly TextBlock _block = new TextBlock();
        IText _source;
        int _lastVersion;
        Rectangle _lastExtents;

        public TextBlockElement(string literal) : base(Handlers)
        {
            _source = new DynamicText(() =>
            {
                _block.Text = literal;
                return new[] { _block };
            });
        }

        public TextBlockElement(StringId id) : base(Handlers)
        {
            _source = new DynamicText(() =>
            {
                var assets = Resolve<IAssetManager>();
                var settings = Resolve<ISettings>();
                var text =  assets.LoadString(id, settings.Gameplay.Language);
                _block.Text = text;
                return new[] { _block };
            });
        }
        public TextBlockElement(IText source) : base(Handlers) { _source = source; }
        public override string ToString() => $"TextSection {_source?.ToString() ?? _block?.ToString()}";
        public TextBlockElement Bold() { _block.Style = TextStyle.Fat; return this; }
        public TextBlockElement Color(FontColor color) { _block.Color = color; return this; }
        public TextBlockElement Left() { _block.Alignment = TextAlignment.Left; return this; }
        public TextBlockElement Center() { _block.Alignment = TextAlignment.Center; return this; }
        public TextBlockElement Right() { _block.Alignment = TextAlignment.Right; return this; }
        public TextBlockElement NoWrap() { _block.Arrangement |= TextArrangement.NoWrap; return this; }
        public TextBlockElement Source(IText source) { _source = source; _lastVersion = 0; return this; }
        public TextBlockElement LiteralString(string literal)
        {
            _source = new DynamicText(() =>
            {
                _block.Text = literal;
                return new[] { _block };
            });
            _lastVersion = 0;
            return this;
        }

        IEnumerable<TextLine> BuildLines(Rectangle extents, IEnumerable<TextBlock> blocks)
        {
            var textManager = Resolve<ITextManager>();

            var line = new TextLine();
            foreach (var block in textManager.SplitBlocksToSingleWords(blocks))
            {
                var size = textManager.Measure(block);
                if (block.Arrangement.HasFlag(TextArrangement.ForceNewLine)
                    ||
                    line.Width > 0 && line.Width + size.X > extents.Width)
                {
                    yield return line;
                    line = new TextLine();
                }

                line.Add(block, size);
            }
            yield return line;
        }

        void Rebuild(Rectangle extents)
        {
            if (extents == _lastExtents && _source.Version <= _lastVersion)
                return;
            _lastVersion = _source.Version;
            _lastExtents = extents;

            foreach(var child in Children)
                child.Detach();
            Children.Clear();

            foreach (var line in BuildLines(extents, _source.Get()))
                AttachChild(line);
        }

        public override Vector2 GetSize()
        {
            Vector2 size = Vector2.Zero;
            foreach (var child in Children.OfType<IUiElement>())
            {
                var childSize = child.GetSize();
                if (childSize.X > size.X)
                    size.X = childSize.X;

                size.Y += childSize.Y;
            }

            return size;
        }

        protected override int DoLayout(Rectangle extents, int order, Func<IUiElement, Rectangle, int, int> func)
        {
            Rebuild(extents);

            int maxOrder = order;
            var offset = 0;
            foreach (var line in Children.OfType<TextLine>())
            {
                var lineExtents = new Rectangle(extents.X, extents.Y + offset, extents.Width, line.Height);
                maxOrder = Math.Max(maxOrder, func(line, lineExtents, order + 1));
                offset += line.Height;
            }

            return order;
        }
    }
}