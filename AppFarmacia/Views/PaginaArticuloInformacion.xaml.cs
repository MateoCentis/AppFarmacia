using AppFarmacia.ViewModels;
using UraniumUI.Pages;

namespace AppFarmacia.Views;
public partial class PaginaArticuloInformacion : UraniumContentPage
{
    private readonly PaginaArticuloInformacionViewModel viewModel;

    public PaginaArticuloInformacion(PaginaArticuloInformacionViewModel viewModel)
	{
		//Defino el gráfico acá porque no se si se puede con MVVM lo siento XD
		InitializeComponent();
		BindingContext = viewModel;
		this.viewModel = viewModel;
	}


    private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        //Por si se quiere implementar algo al scrollear y para que ande el scroll (?)
    }

}