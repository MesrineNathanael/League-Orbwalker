using Gumayusi_Orbwalker.Core.League.Commons;
using Gumayusi_Orbwalker.Core.League.OrbWalker;
using Gumayusi_Orbwalker.Core.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gumayusi_Orbwalker.Core
{
    public class LeagueHolder
    {
        public OrbWalkerLogic OrbWalker;

        private KeyInjection _keyInjection;

        private MouseInputs _mouseInputs;

        public AppSettings Settings;

        public LeagueHolder()
        {
            _keyInjection = new KeyInjection();
            _mouseInputs = new MouseInputs();

            OrbWalker = new OrbWalkerLogic(_keyInjection, _mouseInputs);
            OrbWalker.Start();

            Settings = new AppSettings();

            Settings.LoadConfig();
        }

        public void LoadSettingsAndApply()
        {
            Settings.LoadConfig();
            ApplySettings();
        }

        public void SaveSettingsAndApply()
        {
            Settings.SaveConfig();
            ApplySettings();
        }

        public void ApplySettings()
        {
            OrbWalker.SetEnemyHpColorBar(Settings.Config.EnemyHpColorHtml);
        }
    }
}
