// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using OpenTK;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Graphics;

namespace osu.Game.Screens.Edit.Screens.Compose.Timeline
{
    internal class ScrollingTimelineContainer : ScrollContainer
    {
        public readonly Bindable<WorkingBeatmap> Beatmap = new Bindable<WorkingBeatmap>();

        private readonly BeatmapWaveformGraph graph;

        public ScrollingTimelineContainer()
            : base(Direction.Horizontal)
        {
            Masking = true;

            Add(graph = new BeatmapWaveformGraph
            {
                RelativeSizeAxes = Axes.Both,
                Colour = OsuColour.FromHex("222"),
                Depth = float.MaxValue
            });

            Content.AutoSizeAxes = Axes.None;
            Content.RelativeSizeAxes = Axes.Both;

            graph.Beatmap.BindTo(Beatmap);
        }

        private float minZoom = 1;
        /// <summary>
        /// The minimum zoom level allowed.
        /// </summary>
        public float MinZoom
        {
            get { return minZoom; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (minZoom == value)
                    return;
                minZoom = value;

                // Update the zoom level
                Zoom = Zoom;
            }
        }

        private float maxZoom = 30;
        /// <summary>
        /// The maximum zoom level allowed.
        /// </summary>
        public float MaxZoom
        {
            get { return maxZoom; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (maxZoom == value)
                    return;
                maxZoom = value;

                // Update the zoom level
                Zoom = Zoom;
            }
        }

        private float zoom = 1;
        /// <summary>
        /// The current zoom level.
        /// </summary>
        public float Zoom
        {
            get { return zoom; }
            set
            {
                value = MathHelper.Clamp(value, MinZoom, MaxZoom);
                if (zoom == value)
                    return;
                zoom = value;

                Content.ResizeWidthTo(Zoom);
            }
        }

        protected override bool OnWheel(InputState state)
        {
            if (!state.Keyboard.ControlPressed)
                return base.OnWheel(state);

            float relativeContentPosition = Content.ToLocalSpace(state.Mouse.NativeState.Position).X / Content.DrawSize.X;
            float position = ToLocalSpace(state.Mouse.NativeState.Position).X;

            Zoom += state.Mouse.WheelDelta;

            float scrollPos = Content.DrawSize.X * relativeContentPosition - position;
            ScrollTo(scrollPos, false);

            return true;
        }
    }
}
