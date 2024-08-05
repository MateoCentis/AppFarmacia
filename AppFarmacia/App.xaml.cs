namespace AppFarmacia
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitTheme();
            MainPage = new AppShell(); //La página principal es creada acá, referenciando a AppShell
                                        //Esta a su vez contiene un DataTemplate de MainPage
        }

        private void InitTheme()
        {
            Application.Current.UserAppTheme = AppTheme.Light;//Siempre en color claro 
            //Application.Current.Resources.MergedDictionaries.Add(DarkTheme);
        }
    }
}
