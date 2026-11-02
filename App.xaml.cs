using Microsoft.Extensions.DependencyInjection;

namespace CalculatorApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Uygulama açılırken kayıtlı temayı alır.
            string savedTheme = Preferences.Default.Get("SelectedTheme", "System");

            // Kaydedilen temayı uygular.
            if (savedTheme == "Light")
            {
                UserAppTheme = AppTheme.Light;
            }
            else if (savedTheme == "Dark")
            {
                UserAppTheme = AppTheme.Dark;
            }
            else
            {
                UserAppTheme = AppTheme.Unspecified;
            }

        }

        // Uygulamayı AppShell sayfasıyla başlatır.
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}