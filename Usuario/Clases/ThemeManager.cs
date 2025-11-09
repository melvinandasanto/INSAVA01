using System;
using Usuario.Clases;

namespace Usuario.Clases
{
    public static class ThemeManager
    {
        // Almacena el tema actual (Light o Dark)
        public static Tema CurrentTheme { get; private set; } = Temas.Light;

        // Evento que notifica cuando cambia el tema
        public static event Action<Tema> ThemeChanged;

        public static void SetTheme(Tema newTheme)
        {
            if (CurrentTheme == newTheme) return;

            CurrentTheme = newTheme;
            ThemeChanged?.Invoke(CurrentTheme);
        }
    }
}
