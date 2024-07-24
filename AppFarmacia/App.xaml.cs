namespace AppFarmacia
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell(); //La página principal es creada acá, referenciando a AppShell
                                        //Esta a su vez contiene un DataTemplate de MainPage
        }
    }
}
