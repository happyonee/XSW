﻿using System;
using System.Collections.Generic;
using MapKit;
using CoreLocation;
using Foundation;
namespace StritWalk.iOS.CustomMaps.Cluster
{
	public class FBClusteringManager
	{
		public delegate double RespondsToSelectorHandler(FBClusteringManager sender);
		public event FBClusteringManager.RespondsToSelectorHandler RespondsToSelector;

		private FBQuadTree _tree = null;

		public FBClusteringManager() : base()
		{
		}

		public FBClusteringManager(List<IMKAnnotation> annotations) : base()
		{
			AddAnnotations(annotations);
		}

		public void SetAnnotations(List<IMKAnnotation> annotations)
		{
			_tree = null;
			AddAnnotations(annotations);
		}

		public void AddAnnotations(List<IMKAnnotation> annotations)
		{
			if (_tree == null)
				_tree = new FBQuadTree();

			lock (this)
			{
				foreach (IMKAnnotation annotation in annotations)
				{
					_tree.InsertAnnotation(annotation);
				}
			}
		}

		public void RemoveAnnotations(List<IMKAnnotation> annotations)
		{
			if (_tree == null)
				return;

			lock (this)
			{
				foreach (IMKAnnotation annotation in annotations)
				{
					_tree.RemoveAnnotation(annotation);
				}
			}
		}

		public List<IMKAnnotation> ClusteredAnnotationsWithinMapRect(MKMapRect rect, double zoomScale)
		{
			return ClusteredAnnotationsWithinMapRect(rect, zoomScale, null);
		}

		public List<IMKAnnotation> ClusteredAnnotationsWithinMapRect(MKMapRect rect, double zoomScale, Dictionary<IMKAnnotation, bool> filter)
		{
			double cellSize = FBCellSizeForZoomScale(zoomScale);
			if (RespondsToSelector != null)
			{
				cellSize *= RespondsToSelector(this);
			}
            double scaleFactor = zoomScale / cellSize;

            int minX = (int)Math.Floor((rect.MinX) * scaleFactor);
            int maxX = (int)Math.Floor((rect.MaxX) * scaleFactor);
            int minY = (int)Math.Floor((rect.MinY) * scaleFactor);
            int maxY = (int)Math.Floor((rect.MaxY) * scaleFactor);

            //System.Diagnostics.Debug.WriteLine("zoomscale: " + zoomScale);
            //System.Diagnostics.Debug.WriteLine("scale factor: " + scaleFactor);
            //System.Diagnostics.Debug.WriteLine("rect.minx: " + rect.MinX);
            //System.Diagnostics.Debug.WriteLine("rect.maxx: " + rect.MaxX);
            //System.Diagnostics.Debug.WriteLine("minx: " + minX);
            //System.Diagnostics.Debug.WriteLine("maxx: " + maxX);

            minX -= 5;
            minY -= 5;
            maxX += 5;
            maxY += 5;

			//System.Diagnostics.Debug.WriteLine("minx: " + minX);
			//System.Diagnostics.Debug.WriteLine("maxx: " + maxX);

			List<IMKAnnotation> clusteredAnnotations = new List<IMKAnnotation>();

			lock (this)
			{
				for (int x = minX; x <= maxX; x++)
				{
					for (int y = minY; y <= maxY; y++)
					{
						MKMapRect mapRect = new MKMapRect(x / scaleFactor, y / scaleFactor, 1.0 / scaleFactor, 1.0 / scaleFactor);
						FBBoundingBox mapBox = FBUtils.FBBoundingBoxForMapRect(mapRect);

						double totalLatitude = 0;
						double totalLongitude = 0;

						List<IMKAnnotation> annotations = new List<IMKAnnotation>();
                        string annotations_string = "";

						_tree.EnumerateAnnotationsInBox(mapBox, delegate (IMKAnnotation annotation)
							{
								if (filter == null || filter[annotation])
								{
									totalLatitude += annotation.Coordinate.Latitude;
									totalLongitude += annotation.Coordinate.Longitude;
									annotations.Add(annotation);
                                    //annotations_string += annotation.GetTitle() + ", ";
								}
							});

						int count = annotations.Count;
						if (count == 1)
							clusteredAnnotations.AddRange(annotations);
						if (count > 1)
						{
                            annotations_string = count + " posts here";
							CLLocationCoordinate2D coordinate = new CLLocationCoordinate2D(totalLatitude / count, totalLongitude / count);
                            FBAnnotationCluster cluster = new FBAnnotationCluster(coordinate, annotations_string);
							cluster.Annotations = annotations;
							clusteredAnnotations.Add(cluster);
						}

					}
				}
			}

			return clusteredAnnotations;
		}

		public List<IMKAnnotation> AllAnnotations()
		{
			List<IMKAnnotation> annotations = new List<IMKAnnotation>();

			lock (this)
			{
				_tree.EnumerateAnnotations(delegate (IMKAnnotation annotation)
					{
						annotations.Add(annotation);
					});
			}

			return annotations;
		}

		public void DisplayAnnotations(List<IMKAnnotation> annotations, MKMapView mapView)
		{
			List<IMKAnnotation> before = new List<IMKAnnotation>();
			foreach (IMKAnnotation annotation in mapView.Annotations)
				before.Add(annotation);
			//            MKUserLocation userLocation = mapView.UserLocation;
			//            if (userLocation != null)
			//                before.Remove(userLocation);
			List<IMKAnnotation> after = new List<IMKAnnotation>(annotations);
			List<IMKAnnotation> toKeep = new List<IMKAnnotation>(before);
			toKeep = FBUtils.Intersect(toKeep, after);
			List<IMKAnnotation> toAdd = new List<IMKAnnotation>(after);
			toAdd.RemoveAll((IMKAnnotation obj) =>
				{
					return toKeep.Contains(obj);
				});
			List<IMKAnnotation> toRemove = new List<IMKAnnotation>(before);
			toRemove.RemoveAll((IMKAnnotation obj) =>
				{
					return after.Contains(obj);
				});

			NSOperationQueue.MainQueue.AddOperation(delegate ()
				{
					mapView.AddAnnotations(toAdd.ToArray());
					mapView.RemoveAnnotations(toRemove.ToArray());
				}
			);

		}



		public int FBZoomScaleToZoomLevel(double scale)
		{
			double totalTilesAtMaxZoom = MKMapSize.World.Width / 256.0;
			int zoomLevelAtMaxZoom = (int)Math.Log(totalTilesAtMaxZoom, 2);
			int zoomLevel = (int)Math.Max(0, zoomLevelAtMaxZoom + Math.Floor(Math.Log(scale, 2) + 0.5));

			return zoomLevel;
		}

		public float FBCellSizeForZoomScale(double zoomScale)
		{
			int zoomLevel = FBZoomScaleToZoomLevel(zoomScale);

			switch (zoomLevel)
			{
				case 13:
				case 14:
				case 15:
					return 64;
				case 16:
				case 17:
				case 18:
					return 32;
				case 19:
					return 16;

				default:
					return 88;
			}
		}

	}
}
