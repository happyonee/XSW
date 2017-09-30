﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using StritWalk;
using StritWalk.Droid;

[assembly: ExportRenderer(typeof(ExpandableEditor), typeof(ExpandableEditorRenderer))]
namespace StritWalk.Droid
{
    public class ExpandableEditorRenderer : EditorRenderer
    {
        ExpandableEditor element;

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
                return;

            if (e.OldElement != null)
            {

            }

            if (e.NewElement != null)
            {
                element = (ExpandableEditor)Element;
                Control.Hint = element.Placeholder;
                Control.SetHintTextColor(element.PlaceholderColor.ToAndroid());

                Control.TextChanged += (object sender, Android.Text.TextChangedEventArgs e1) =>
                {
                    
                    Control.Hint = element.Placeholder;
                    //Element.LayoutTo(new Rectangle(Element.X, Element.Y - Element.Height, Element.Width, Element.Height * 2));
                };

                Control.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
                Control.InputType = Android.Text.InputTypes.TextFlagMultiLine;

            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //Control.SetMaxLines(4);
        }
    }
}
