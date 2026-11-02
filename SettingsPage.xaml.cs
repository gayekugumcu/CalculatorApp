namespace CalculatorApp;
public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        // Daha önce seçilen temayı yükler.
        string savedTheme = Preferences.Default.Get("SelectedTheme", "System");

        if (savedTheme == "Light")
        {
            LightThemeRadio.IsChecked = true;
        }
        else if (savedTheme == "Dark")
        {
            DarkThemeRadio.IsChecked = true;
        }
        else
        {
            SystemThemeRadio.IsChecked = true;
        }

        // Tema seçeneklerindeki değişiklikleri takip eder.
        LightThemeRadio.CheckedChanged += ThemeRadio_CheckedChanged;
        DarkThemeRadio.CheckedChanged += ThemeRadio_CheckedChanged;
        SystemThemeRadio.CheckedChanged += ThemeRadio_CheckedChanged;
    }

    // Seçilen temayı uygular ve kaydeder.
    private void ThemeRadio_CheckedChanged(
        object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        if (LightThemeRadio.IsChecked)
        {
            Application.Current!.UserAppTheme = AppTheme.Light;
            Preferences.Default.Set("SelectedTheme", "Light");
        }
        else if (DarkThemeRadio.IsChecked)
        {
            Application.Current!.UserAppTheme = AppTheme.Dark;
            Preferences.Default.Set("SelectedTheme", "Dark");
        }
        else if (SystemThemeRadio.IsChecked)
        {
            // Sistem seçilirse Windows'un tema ayarı kullanılır.
            Application.Current!.UserAppTheme = AppTheme.Unspecified;
            Preferences.Default.Set("SelectedTheme", "System");
        }
    }
}