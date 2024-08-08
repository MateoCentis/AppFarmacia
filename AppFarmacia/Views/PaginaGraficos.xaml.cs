namespace AppFarmacia.Views;
using Microcharts;
using SkiaSharp;

public partial class PaginaGraficos : ContentPage
{
	public PaginaGraficos()
	{
		InitializeComponent();
	}

    private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
		//Por si se quiere implementar algo al scrollear y para que ande el scroll (?)
    }
}