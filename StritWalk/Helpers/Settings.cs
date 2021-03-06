﻿using System;
using Plugin.Geolocator.Abstractions;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace StritWalk
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants
        const string UserIdKey = "userid";
        static readonly string UserIdDefault = string.Empty;

        const string AuthTokenKey = "authtoken";
        static readonly string AuthTokenDefault = string.Empty;

        const string latKey = "lat";
        static readonly string latDefault = "0";

        const string lngKey = "lng";
        static readonly string lngDefault = "0";
        #endregion

        public static string AuthToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(AuthTokenKey, AuthTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AuthTokenKey, value);
            }
        }

        public static string UserDescription
        {
            get
            {
                return AppSettings.GetValueOrDefault("UserDescription", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("UserDescription", value);
            }
        }


        public static bool IsLoggedIn => !string.IsNullOrWhiteSpace(UserId);
        public static string UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault(UserIdKey, UserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UserIdKey, value);
            }
        }

        public static string lat
        {
            get => AppSettings.GetValueOrDefault(nameof(lat), latDefault);
            set => AppSettings.AddOrUpdateValue(nameof(lat), value);
        }

        public static string lng
        {
            get => AppSettings.GetValueOrDefault(nameof(lng), lngDefault);
            set => AppSettings.AddOrUpdateValue(nameof(lng), value);
        }

        public static bool listEnd
        {
            get => AppSettings.GetValueOrDefault(nameof(listEnd), false);
            set => AppSettings.AddOrUpdateValue(nameof(listEnd), value);
        }

        public static int Num_posts
        {
            get => AppSettings.GetValueOrDefault(nameof(Num_posts), 0);
            set => AppSettings.AddOrUpdateValue(nameof(Num_posts), value);
        }

        public static int Num_likes
        {
            get => AppSettings.GetValueOrDefault(nameof(Num_likes), 0);
            set => AppSettings.AddOrUpdateValue(nameof(Num_likes), value);
        }

        public static int Num_friends
        {
            get => AppSettings.GetValueOrDefault(nameof(Num_friends), 0);
            set => AppSettings.AddOrUpdateValue(nameof(Num_friends), value);
        }

        public static string Notification_id
        {
            get => AppSettings.GetValueOrDefault(nameof(Notification_id), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Notification_id), value);
        }

        public static DateTime LastBea
        {
            get => AppSettings.GetValueOrDefault(nameof(LastBea), DateTime.UtcNow.Date.AddDays(-5));
            set => AppSettings.AddOrUpdateValue(nameof(LastBea), value);
        }

    }
}
