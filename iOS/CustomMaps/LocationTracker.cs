﻿using System;
using System.Diagnostics;
using CoreLocation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(StritWalk.iOS.LocationTracker))]

namespace StritWalk.iOS
{
    public class LocationTracker : ILocationTracker
    {
        CLLocationManager locationManager;

        public event EventHandler<GeographicLocation> LocationChanged;

        public LocationTracker()
        {
            locationManager = new CLLocationManager();

            // Request authorization for iOS 8 and above.
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locationManager.RequestWhenInUseAuthorization();
            }

            locationManager.LocationsUpdated +=
                (object sender, CLLocationsUpdatedEventArgs args) =>
                {
                    Debug.WriteLine("Location is updating");
                    CLLocationCoordinate2D coordinate = args.Locations[0].Coordinate;
                    EventHandler<GeographicLocation> handler = LocationChanged;

                    if (handler != null)
                    {
                        handler(this, new GeographicLocation(coordinate.Latitude,
                                                             coordinate.Longitude));
                    }
                };
        }
        public void StartTracking()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                locationManager.StartUpdatingLocation();
            }
        }

        public void PauseTracking()
        {
            locationManager.StopUpdatingLocation();
        }

        public void SingleTracking()
        {

        }
    }
}
