using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Markup;

namespace HotelManagement.Themes
{
    public static class ThemesController
    {
        public static ThemeTypes CurrentTheme { get; set; }

        public enum ThemeTypes
        {
            Light, Dark
        }

        public static ResourceDictionary ThemeDictionary
        {
            get { return Application.Current.Resources.MergedDictionaries[0]; }
            set { Application.Current.Resources.MergedDictionaries[0] = value; }
        }
        
        private static void ChangeTheme(Uri uri)
        {
            ThemeDictionary = new ResourceDictionary() { Source = uri };
        }
        public static void SetTheme(ThemeTypes theme)
        {
            string themeName = null;
            CurrentTheme = theme;
            switch (theme)
            {
                case ThemeTypes.Dark:
                    themeName = "Dark"; 
                    break;
                case ThemeTypes.Light: 
                    themeName = "Light"; 
                    break;
            }

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                {
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
                }
            }
            catch { }
        }
    }
}