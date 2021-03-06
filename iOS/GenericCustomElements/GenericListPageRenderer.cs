﻿using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;
using CoreGraphics;
using KeyboardOverlap.Forms.Plugin.iOSUnified;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(ItemsPage), typeof(GenericListPageRenderer))]
namespace StritWalk.iOS
{
    public class GenericListPageRenderer : PageRenderer
    {

        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        //bool _pageWasShiftedUp;
        //double _activeViewBottom;
        private bool _isKeyboardShown;
        private Rectangle originalFrame;
        AppDelegate ad;

        public GenericListPageRenderer()
        {
            ad = (AppDelegate)UIApplication.SharedApplication.Delegate;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
			UITapGestureRecognizer gesture = new UITapGestureRecognizer(() => { View.EndEditing(true); });
			View.AddGestureRecognizer(gesture);

            RegisterForKeyboardNotifications();

            //var page = Element as ContentPage;

            //if (page != null)
            //{
            //	var contentScrollView = page.Content as ScrollView;
            //	if (contentScrollView != null)
            //		return;

            //	RegisterForKeyboardNotifications();
            //}
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UnregisterForKeyboardNotifications();
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
            _isKeyboardShown = false;
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

        protected virtual void OnKeyboardShow(NSNotification notification)
        {
            if (!IsViewLoaded || _isKeyboardShown)
                return;

            _isKeyboardShown = true;
            ad.KeyOn = true;
            //var activeView = View.FindFirstResponder();

            //if (activeView == null)
            //	return;

            //var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            //var isOverlapping = activeView.IsKeyboardOverlapping(View, keyboardFrame);

            //if (!isOverlapping)
            //	return;

            //if (isOverlapping)
            //{
            //	_activeViewBottom = activeView.GetViewRelativeBottom(View);
            //	ShiftPageUp(keyboardFrame.Height, _activeViewBottom);
            //}
        }

        private void OnKeyboardHide(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            _isKeyboardShown = false;
            ad.KeyOn = false;
            //var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);

            //if (_pageWasShiftedUp)
            //{
            //	ShiftPageDown(keyboardFrame.Height, _activeViewBottom);
            //}
        }

        private void ShiftPageUp(nfloat keyboardHeight, double activeViewBottom)
        {
            var pageFrame = Element.Bounds;
            originalFrame = pageFrame;

            var newY = pageFrame.Y + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);
            var newH = pageFrame.Height + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

            //Element.LayoutTo(new Rectangle(pageFrame.X, newY,
            //pageFrame.Width, pageFrame.Height));
            //dev10n
            Element.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y, pageFrame.Width, newH));

            //_pageWasShiftedUp = true;
        }

        private void ShiftPageDown(nfloat keyboardHeight, double activeViewBottom)
        {
            //var pageFrame = Element.Bounds;

            //var newY = pageFrame.Y - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);
            //var newH = pageFrame.Height - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

            //Element.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y,
            //                               pageFrame.Width, newH));

            Element.LayoutTo(originalFrame);
            //_pageWasShiftedUp = false;
        }

        private double CalculateShiftByAmount(double pageHeight, nfloat keyboardHeight, double activeViewBottom)
        {
            return (pageHeight - activeViewBottom) - keyboardHeight;
        }
    }
}
