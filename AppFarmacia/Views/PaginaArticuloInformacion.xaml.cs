using AppFarmacia.ViewModels;
using Microcharts;
using SkiaSharp;

namespace AppFarmacia.Views;
public partial class PaginaArticuloInformacion : ContentPage
{
    private readonly PaginaArticuloInformacionViewModel viewModel;
    private readonly List<string> Meses = new List<string>
        {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };
    public PaginaArticuloInformacion(PaginaArticuloInformacionViewModel viewModel)
	{
		//Defino el gráfico acá porque no sabría como hacerlo con MVVM lo siento XD
		InitializeComponent();
		BindingContext = viewModel;
		this.viewModel = viewModel;
		CargarGrafico();
	}

	//Habría que hacer un gráfico de precios y otro de demanda
	private void CargarGrafico()
	{
        //Definir las entries del gráfico
        //var demandas = this.viewModel.ObtenerDemandaMensual();
        var demandas = new List<float> { 10, 30, 40, 50, 10, 20, 30, 90, 100, 120, 140, 150 };
        var entries = new List<ChartEntry>();

		for(int i = 0; i < this.Meses.Count; ++i)
		{
			var entry = new ChartEntry(demandas[i])
			{
				Label = Meses[i],
				ValueLabel = demandas[i].ToString(),
				Color = SKColor.Parse(GetColorForIndex(i))
			};
			entries.Add(entry);
		}

		//creo el tipo de chart
		chartView.Chart = new LineChart
		{
			Entries = entries,
			LabelTextSize = 24,
			ValueLabelTextSize = 16,
			LineSize = 8,
			BackgroundColor = SKColor.Parse("FFFFFF")
		};

	}

    private string GetColorForIndex(int index)
    {
        var colors = new List<string>
            {
                "#ff0000", "#00ff00", "#0000ff", "#ffff00", "#ff00ff", "#00ffff",
                "#ff9900", "#009900", "#990099", "#3399ff", "#ff3333", "#33ff33"
            };

        return colors[index % colors.Count];
    }
}