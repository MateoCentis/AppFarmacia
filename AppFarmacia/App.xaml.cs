using System.Globalization;

namespace AppFarmacia
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-ES");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-ES");
            InitTheme();
            MainPage = new AppShell(); //La página principal es creada acá, referenciando a AppShell
                                        //Esta a su vez contiene un DataTemplate de MainPage
        }

        private void InitTheme()
        {
            Application.Current.UserAppTheme = AppTheme.Light;//Siempre en color claro 
            //Application.Current.Resources.MergedDictionaries.Add(DarkTheme);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

#if WINDOWS
            window.Created += (s, e) => CenterAndResizeWindow(window);
#endif

            return window;
        }

#if WINDOWS
        private void CenterAndResizeWindow(Window window)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window.Handler.PlatformView);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            // Obtén el área de trabajo y la resolución actual de la pantalla
            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
            var screenWidth = displayArea.WorkArea.Width;
            var screenHeight = displayArea.WorkArea.Height;

            // Define el tamaño de la ventana según la resolución
            var windowWidth = screenWidth * 0.9;  // Por defecto, el 80% del ancho de la pantalla
            var windowHeight = screenHeight * 0.9;  // Por defecto, el 80% del alto de la pantalla

            // Ajusta según la resolución
            if (screenWidth >= 2560)  // 2K
            {
                windowWidth = 2300;
                windowHeight = 1288;
            }
            else if (screenWidth >= 1920)  // 1920x1080
            {
                windowWidth = 1800;
                windowHeight = 1012;
            }
            else if (screenWidth >= 1280)  // HD
            {
                 windowWidth = screenWidth;
                 windowHeight = screenHeight;
            }

            // Centra la ventana en la pantalla
            var centerPosition = new Windows.Graphics.PointInt32(
                (screenWidth - (int)windowWidth) / 2,
                (screenHeight - (int)windowHeight) / 2
            );

            // Mueve y redimensiona la ventana
            appWindow.MoveAndResize(new Windows.Graphics.RectInt32(centerPosition.X, centerPosition.Y, (int)windowWidth, (int)windowHeight));
        }
#endif
    }
}

