﻿using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using TK.CustomMap.iOSUnified;
//using KeyboardOverlap.Forms.Plugin.iOSUnified;

namespace StritWalk.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public event EventHandler KeyAppeared;
        public CoreGraphics.CGRect KeyFrame
        {
            get;
            set;
        }
        public bool KeyOn
        {
            get;
            set;
        }

        public void TriggerKey()
        {
            KeyAppeared(KeyFrame, null);
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            //KeyboardOverlapRenderer.Init();
            LoadApplication(new App());
            TKCustomMapRenderer.InitMapRenderer();

            return base.FinishedLaunching(app, options);
        }

    }
}
