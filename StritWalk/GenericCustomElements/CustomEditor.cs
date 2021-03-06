﻿using System;
using System.Linq;
using Xamarin.Forms;
namespace StritWalk
{
    public class CustomEditor : Editor
    {
		bool sized = false;
		public double lineHeight = 0;

		public static BindableProperty PlaceholderProperty
        = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CustomEditor));

		public static BindableProperty PlaceholderColorProperty
        = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(CustomEditor), Color.FromHex("#888888"));

		public static BindableProperty ReadyProperty
		= BindableProperty.Create(nameof(Ready), typeof(string), typeof(CustomEditor));

		public string Placeholder
		{
			get { return (string)GetValue(PlaceholderProperty); }
			set { SetValue(PlaceholderProperty, value); }
		}

		public Color PlaceholderColor
		{
			get { return (Color)GetValue(PlaceholderColorProperty); }
			set { SetValue(PlaceholderColorProperty, value); }
		}

		public bool Ready
		{
			get { return (bool)GetValue(ReadyProperty); }
			set { SetValue(ReadyProperty, value); }
		}

		//protected override void OnSizeAllocated(double width, double height)
		//{
		//	if (!sized)
		//	{
		//		int count = Text.Count(c => c == '\n');
		//		lineHeight = (height / (count + 1));
		//		sized = true;
		//	}
		//	base.OnSizeAllocated(width, height);
		//}

    }
}
