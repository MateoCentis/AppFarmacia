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
		//Defino el gráfico acá porque no se si se puede con MVVM lo siento XD
		InitializeComponent();
		BindingContext = viewModel;
		this.viewModel = viewModel;
		CargarGrafico();
	}

	//Habría que hacer un gráfico de precios y otro de demanda
	private void CargarGrafico()
	{
        //Definir las entries del gráfico
        //var demandas = this.viewModel.ObtenerDemandaMensual();//Así se deberían obtener las demandas mensuales pero no está implementado aún
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
		{   //Data
			Entries = entries,
			//Lineas
			LineMode = LineMode.Straight,
			LineSize = 8,
			//Puntos
			PointMode = PointMode.Square,
			PointSize = 18,
			//Labels
			LabelTextSize = 24,
			ValueLabelTextSize = 16,
			LabelOrientation = Orientation.Horizontal,
			ValueLabelOrientation = Orientation.Horizontal,
			ValueLabelOption = ValueLabelOption.TopOfElement,
			// Lineas
			ShowYAxisLines = true,
			ShowYAxisText = true,
			YAxisPosition = Position.Left,
			// Colores
			EnableYFadeOutGradient = false,
			BackgroundColor = SKColor.Parse("FFFFFF"),
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

    private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        //Por si se quiere implementar algo al scrollear y para que ande el scroll (?)
    }

}