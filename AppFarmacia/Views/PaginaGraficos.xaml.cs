namespace AppFarmacia.Views;
using UraniumUI.Pages;
using Microcharts;
using SkiaSharp;
using AppFarmacia.ViewModels;

public partial class PaginaGraficos : UraniumContentPage
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