using homefinderpro.LandlordViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace homefinderpro.landlord;

public partial class LandlordMaps : ContentPage
{
    public LandlordMaps()
    {
        InitializeComponent();
    }


    private void SetupCircleOnMap(Map map, Location center, Distance radius)
    {
        var circle = new Circle
        {
            Center = center,
            Radius = radius,
            StrokeColor = Color.FromArgb("#88FF0000"),
            StrokeWidth = 8,
        };
        map.MapElements.Add(circle);
    }






}

