﻿using UAlbion.Core;
using UAlbion.Formats.Config;
using UAlbion.Game.Events;

namespace UAlbion.Game.Scenes
{
    public interface IMenuScene : IScene { }
    public class MenuScene : GameScene, IMenuScene
    {
        bool _clockWasRunning;
        public MenuScene() : base(SceneId.MainMenu, new OrthographicCamera())
        { }

        protected override void Subscribed()
        {
            _clockWasRunning = Resolve<IClock>().IsRunning;
            if (_clockWasRunning)
                Raise(new StopClockEvent());

            Raise(new PushMouseModeEvent(MouseMode.Normal));
            Raise(new PushInputModeEvent(InputMode.MainMenu));
        }

        protected override void Unsubscribed()
        {
            Raise(new PopMouseModeEvent());
            Raise(new PopInputModeEvent());
            if (_clockWasRunning)
                Raise(new StartClockEvent());
        }
    }
}
