using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxShift.Services
{
    public class SettingsService
    {
        private const string DarkModeKey = "DarkMode";
        private const string GridThemeKey = "GridTheme";
        private const string AnimationsKey = "Animations";

        public bool IsDarkMode
        {
            get
            {
                return Preferences.Default.Get(DarkModeKey, false);
            }

            set
            {
                Preferences.Default.Set(DarkModeKey, value);
            }
        }

        public string GridTheme
        {
            get
            {
                return Preferences.Default.Get(GridThemeKey, "Classic");
            }

            set
            {
                Preferences.Default.Set(GridThemeKey, value);
            }
        }

        public bool AnimationsEnabled
        {
            get
            {
                return Preferences.Default.Get(AnimationsKey, true);
            }

            set
            {
               Preferences.Default.Set(AnimationsKey, value);
            }
        }

        public void ApplyTheme()
        {
            if(Application.Current == null)
            {
               return;
            }

            if (IsDarkMode)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }
    }
}
