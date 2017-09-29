﻿using StritWalk;
using StritWalk.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System;
using Cirrious.FluentLayouts.Touch;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(ExpandableEditor), typeof(ExpandableEditorRenderer))]
namespace StritWalk.iOS
{
    public class ExpandableEditorRenderer : EditorRenderer
    {

        private UILabel _placeholderLabel;
        ExpandableEditor element;
        double oneLine;
        bool toStart = true;
        bool keyOn;
        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        Xamarin.Forms.Rectangle originalFrame;
        Xamarin.Forms.Rectangle originalWithKeyFrame;
        //Xamarin.Forms.Rectangle originalPageFrame;
        //Xamarin.Forms.Rectangle originalListFrame;
        bool firstassignment;
        //nfloat scrolly;
        CGRect originalKeyFrame;
        CGRect originalRect;
        int numlines = 1;
        int framelines = 0;
        AppDelegate ad;
        CGSize requestSize;

        public ExpandableEditorRenderer()
        {
            ad = (AppDelegate)UIApplication.SharedApplication.Delegate;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Editor> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            if (e.OldElement != null)
            {
                ad.KeyAppeared -= KeyRaise;
                UnregisterForKeyboardNotifications();
                element.TextChanged -= Element_TextChanged;
            }

            if (e.NewElement != null)
            {
                element = (ExpandableEditor)Element;
                ad.KeyAppeared += KeyRaise;
                RegisterForKeyboardNotifications();
                //CreatePlaceholderLabel((ExpandableEditor)Element, Control);
                Control.InputAccessoryView = null;
                Control.Ended += OnEnded;
                Control.Changed += OnChanged;
                Control.Started += OnFocused;
                Control.Text = element.Placeholder;
                Control.TextColor = UIColor.Gray;
                Control.ScrollEnabled = false;
                element.TextChanged += Element_TextChanged;
                //Control.EnablesReturnKeyAutomatically = true;
                Control.ReturnKeyType = UIReturnKeyType.Send;

                //Control.Superview.TranslatesAutoresizingMaskIntoConstraints = false;
                //Control.TranslatesAutoresizingMaskIntoConstraints = false;
                //Control.Frame = new CGRect(Control.Frame.X, 0, Control.Frame.Width, Control.Frame.Height);
                //LayoutSubviews();

                Control.ShouldChangeText = (textView, range, text) =>
                {
                    if (text.Equals("\n"))
                    {
                        Control.Text = "";
                        //Control.EndEditing(true);
                        element?.InvokeCompleted();
                        return false;
                    }
                    return true;
                };
            }
        }

        void KeyRaise(object sender, EventArgs args)
        {
            originalKeyFrame = (CGRect)sender;
            originalFrame = Element.Bounds;
            //Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, (originalFrame.Y - originalKeyFrame.Height), originalFrame.Width, originalFrame.Height));
        }

        void CreatePlaceholderLabel(ExpandableEditor element, UITextView parent)
        {
            _placeholderLabel = new UILabel
            {
                Text = element.Placeholder,
                TextColor = element.PlaceholderColor.ToUIColor(),
                BackgroundColor = UIColor.Clear,
                //Font = UIFont.FromName(element.FontFamily, (nfloat)element.FontSize)
            };
            _placeholderLabel.Font.WithSize((nfloat)element.FontSize);
            _placeholderLabel.SizeToFit();

            parent.AddSubview(_placeholderLabel);

            parent.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            parent.AddConstraints(
                _placeholderLabel.AtLeftOf(parent, 7),
                _placeholderLabel.WithSameCenterY(parent)
            );
            parent.LayoutIfNeeded();

            _placeholderLabel.Hidden = parent.HasText;
        }

        void OnEnded(object sender, EventArgs args)
        {
            //if (!((UITextView)sender).HasText && _placeholderLabel != null)
            //_placeholderLabel.Hidden = false;

            if (string.IsNullOrEmpty(Control.Text) || string.IsNullOrWhiteSpace(Control.Text) || Control.Text == element.Placeholder)
            {
                Control.Text = element.Placeholder;
                Control.TextColor = UIColor.Gray;
            }

            //Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalFrame.Y, originalFrame.Width, originalFrame.Height));
        }

        void OnChanged(object sender, EventArgs args)
        {
            //if (_placeholderLabel != null)
            //_placeholderLabel.Hidden = ((UITextView)sender).HasText;
        }

        void OnFocused(object sender, EventArgs args)
        {
            if (Control.Text == element.Placeholder)
            {
                Control.Text = "";
            }
            Control.TextColor = UIColor.Black;

            if (toStart)
                oneLine = Math.Round(Control.ContentSize.Height / Control.Font.LineHeight);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Control.Ended -= OnEnded;
                Control.Changed -= OnChanged;
                Control.Started -= OnFocused;
                ad.KeyAppeared -= KeyRaise;
                element.TextChanged -= Element_TextChanged;
                UnregisterForKeyboardNotifications();
            }
        }

        void RegisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver == null)
                _keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardShow);
            if (_keyboardHideObserver == null)
                _keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHide);
        }

        void UnregisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
                _keyboardShowObserver.Dispose();
                _keyboardShowObserver = null;
            }
            if (_keyboardHideObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
                _keyboardHideObserver.Dispose();
                _keyboardHideObserver = null;
            }
        }

        protected virtual async void OnKeyboardShow(NSNotification notification)
        {
            if (!firstassignment)
            {
                originalKeyFrame = UIKeyboard.FrameEndFromNotification(notification);
                originalFrame = Element.Bounds;
                originalRect = Control.Frame;
                firstassignment = true;
            }

            var currentFrame = Element.Bounds;
            var currentRect = Control.Frame;
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, originalKeyFrame.Height + (nfloat)currentRect.Height - (nfloat)originalRect.Height, 0);
            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;

            BeginAnimations("AnimateForKeyboard");
            SetAnimationBeginsFromCurrentState(true);
            SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            var newFrame = new CGRect(originalRect.X, (originalRect.Y - originalKeyFrame.Height - currentRect.Height + originalRect.Height), originalRect.Width, currentRect.Height);
            Control.Frame = newFrame;
            CommitAnimations();

            await Task.Delay(100);
            Element.Layout(new Xamarin.Forms.Rectangle(originalFrame.X, (originalFrame.Y - originalKeyFrame.Height - currentFrame.Height + originalFrame.Height), originalFrame.Width, currentFrame.Height));
            LayoutSubviews();

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }

            keyOn = true;
        }

        protected virtual async void OnKeyboardHide(NSNotification notification)
        {
            keyOn = false;
            var currentFrame = Element.Bounds;
            var currentRect = Control.Frame;
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, (nfloat)currentFrame.Height - (nfloat)originalFrame.Height, 0);
            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;

            BeginAnimations("AnimateForKeyboard");
            SetAnimationBeginsFromCurrentState(true);
            SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            var newFrame = new CGRect(originalRect.X, originalRect.Y - currentRect.Height + originalRect.Height, originalRect.Width, currentRect.Height);
            newFrame = new CGRect(originalRect.X, originalRect.Y + originalKeyFrame.Height, originalRect.Width, currentRect.Height);
            Control.Frame = newFrame;
            CommitAnimations();

            await Task.Delay(100);
            Element.Layout(new Xamarin.Forms.Rectangle(originalFrame.X, originalFrame.Y - currentFrame.Height + originalFrame.Height, originalFrame.Width, currentFrame.Height));
            LayoutSubviews();

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }
        }

        void AgumentView3()
        {
            //definizione listview
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            //aumento riga
            //originalWithKeyFrame = Element.Bounds;
            framelines++;
            numlines++;

            var currentRect = Control.Frame;
            var currentFrame = Element.Bounds;

            var newy = ((originalFrame.Y) - originalKeyFrame.Height) - (originalFrame.Height - 0) * framelines;
            newy = originalRect.Y - originalKeyFrame.Height;
            newy = originalFrame.Y - originalKeyFrame.Height + originalFrame.Height - requestSize.Height;
            newy = originalFrame.Y - originalKeyFrame.Height + originalFrame.Height - requestSize.Height;
            var newh = (originalFrame.Height * framelines) + (originalFrame.Height - 0);
            newh = requestSize.Height;

            originalWithKeyFrame = new Xamarin.Forms.Rectangle(originalRect.X, newy - Control.Frame.Y, originalRect.Width, newh);
            var newFrame = new CGRect(originalRect.X, currentRect.Y, originalRect.Width, newh);
            //newFrame = currentRect;
            newFrame.Size = new CGSize(currentRect.Size.Width, requestSize.Height);
            //Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalWithKeyFrame.Y - (originalFrame.Height - 16), originalWithKeyFrame.Width, originalWithKeyFrame.Height + (originalFrame.Height - 16)));

            if (requestSize.Height < 100)
            {
                Element.LayoutTo(originalWithKeyFrame, 50, Easing.Linear);
                //Control.Frame = newFrame;

                UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, listcontrol.ContentInset.Bottom + ((nfloat)requestSize.Height - 0), 0);
                contentinsets = new UIEdgeInsets(0, 0, originalKeyFrame.Height + ((nfloat)requestSize.Height - (nfloat)originalFrame.Height), 0);
                listcontrol.ContentInset = contentinsets;
                listcontrol.ScrollIndicatorInsets = contentinsets;

                Control.ScrollEnabled = false;
                element.ScrollReady = false;            
            }
            else
            {
                Console.WriteLine("maggiore di scroll limit");
                newh = Element.Bounds.Height - 16;
                newy = Element.Bounds.Y - 16;
                originalWithKeyFrame = new Xamarin.Forms.Rectangle(Element.Bounds.X, Element.Bounds.Y, Element.Bounds.Width, newh);
                //Element.LayoutTo(originalWithKeyFrame);
                Control.ScrollEnabled = true;
                element.ScrollReady = true;
            }

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }

        void Element_TextChanged(object sender, TextChangedEventArgs e1)
        {
            requestSize = Control.SizeThatFits(new CGSize(Control.Frame.Width, 99999));

            if (keyOn)
                AgumentView3();

            //placeholder
            if (Control.Text == element.Placeholder)
            {
                Control.TextColor = UIColor.Gray;
                element.Ready = false;
            }
            else
            {
                element.Ready = true;
            }
        }

        public static UIView ConvertFormsToNative(Xamarin.Forms.View view, CGRect size)
        {
            var renderer = RendererFactory.GetRenderer(view);

            renderer.NativeView.Frame = size;

            renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
            renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

            renderer.Element.Layout(size.ToRectangle());

            var nativeView = renderer.NativeView;

            nativeView.SetNeedsLayout();

            return nativeView;
        }

    }
}