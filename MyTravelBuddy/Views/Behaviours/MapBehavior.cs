using System;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using MauiMap = Microsoft.Maui.Controls.Maps.Map;
namespace MyTravelBuddy.Views.Behaviours;

public class MapBehavior : BindableBehavior<MauiMap>
{
    private MauiMap map;
    const double PIx = Math.PI;

    //todo: make marker size bigger (best would be flexible marker size depending on zoom level)
    //todo: add line between destinations

    public static readonly BindableProperty IsReadyProperty =
        BindableProperty.CreateAttached(nameof(IsReady),
            typeof(bool),
            typeof(MapBehavior),
            default(bool),
            BindingMode.Default,
            null,
            OnIsReadyChanged);

    public bool IsReady
    {
        get => (bool)GetValue(IsReadyProperty);
        set => SetValue(IsReadyProperty, value);
    }

    private static void OnIsReadyChanged(BindableObject view, object oldValue, object newValue)
    {
        var mapBehavior = view as MapBehavior;

        if (mapBehavior != null)
        {
            if (newValue is bool)
                mapBehavior.ChangePosition();
        }
    }

    public static readonly BindableProperty PlacesProperty =
        BindableProperty.CreateAttached(nameof(Places),
            typeof(IEnumerable<Place>),
            typeof(MapBehavior),
            default(IEnumerable<Place>),
            BindingMode.Default,
            null,
            OnPlacesChanged);


    public IEnumerable<Place> Places
    {
        get => (IEnumerable<Place>)GetValue(PlacesProperty);
        set => SetValue(PlacesProperty, value);
    }

    private static void OnPlacesChanged(BindableObject view, object oldValue, object newValue)
    {
        var mapBehavior = view as MapBehavior;

        if (mapBehavior != null)
        {
            mapBehavior.ChangePosition();

            mapBehavior.DrawLocation();
        }
    }

    private void DrawLocation()
    {
        map.MapElements.Clear();

        if (Places == null || !Places.Any())
            return;

        foreach (var place in Places)
        {
            var distance = Distance.FromMeters(50);

            Circle circle = new Circle()
            {
                Center = place.Location,
                Radius = distance,
                StrokeColor = Color.FromArgb("#88FF0000"),
                StrokeWidth = 8,
                FillColor = Color.FromArgb("#88FFC0CB")
                
            };

            map.MapElements.Add(circle);
        }
    }

    private void ChangePosition()
    {
        if (!IsReady || Places == null || !Places.Any())
            return;

        if (Places.Count() == 1)
        {
            var place = Places.First();
            var distance = Distance.FromKilometers(5);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(place.Location, distance));
        }
        else
        {
            //move such that all locations are in view
            var maxLat = Places.Max(x => x.Location.Latitude);
            var maxLong = Places.Max(x => x.Location.Longitude);
            var minLat = Places.Min(x => x.Location.Latitude);
            var minLong = Places.Min(x => x.Location.Longitude);

            //calculate center points
            double centerLat = (maxLat + minLat) / 2;
            double centerLng = (maxLong + minLong) / 2;
            var center = new Location(centerLat, centerLng);

            // Calculate radius
            var maxDistance = 0.0;

            //calculate max distance from center point
            foreach (var loc in Places.Select(x => x.Location).ToList())
            {
                var dist = DistanceBetweenPlaces(loc.Longitude, loc.Latitude, centerLng, centerLat);
                maxDistance = Math.Max(maxDistance, dist);
            }

            var margin = GetMargin(maxDistance);

            var radius = Distance.FromKilometers(maxDistance + margin);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, radius));

            DrawPolyLine();
        }    
    }

    private void DrawPolyLine()
    {
        if (Places == null || !Places.Any())
            return;

        if(Places.Count() == 2)
        {     
            // instantiate a polyline
            Polyline polyline = new Polyline
            {
                StrokeColor = Colors.Blue,
                StrokeWidth = 4,
                Geopath =
                {
                    Places.ElementAt(0).Location,
                    Places.ElementAt(1).Location,

                }
            };

            // Add the Polyline to the map's MapElements collection
            map.MapElements.Add(polyline);
        }
    }

    //Variable extra margin depending on distance of points
    private static double GetMargin(double maxDistance)
    {
        return Math.Floor(maxDistance / 10);
    }

    /// Convert degrees to Radians
    /// </summary>
    /// <param name="x">Degrees</param>
    /// <returns>The equivalent in radians</returns>
    private static double Radians(double x)
    {
        return x * PIx / 180;
    }

    // cos(d) = sin(φА)·sin(φB) + cos(φА)·cos(φB)·cos(λА − λB),
    //  where φА, φB are latitudes and λА, λB are longitudes
    // Distance = d * R
    private static double DistanceBetweenPlaces(double lon1, double lat1, double lon2, double lat2)
    {
        double R = 6371; // km

        double sLat1 = Math.Sin(Radians(lat1));
        double sLat2 = Math.Sin(Radians(lat2));
        double cLat1 = Math.Cos(Radians(lat1));
        double cLat2 = Math.Cos(Radians(lat2));
        double cLon = Math.Cos(Radians(lon1) - Radians(lon2));

        double cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;

        double d = Math.Acos(cosD);

        double dist = R * d;

        return dist;
    }

    protected override void OnAttachedTo(MauiMap bindable)
    {
        base.OnAttachedTo(bindable);
        map = bindable;
    }

    protected override void OnDetachingFrom(MauiMap bindable)
    {
        base.OnDetachingFrom(bindable);
        map = null;
    }
}
