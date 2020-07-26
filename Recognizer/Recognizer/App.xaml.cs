using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Recognizer.Services;
using Recognizer.Views;
using System.Collections.Generic;
using Android.Content.PM;
using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Permission = Plugin.Permissions.Abstractions.Permission;
using Recognizer.Setup;

namespace Recognizer
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            
            CheckPermissions();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private async void CheckPermissions()
        {
            List<Permission> permissions = new List<Permission>();

            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await UserDialogs.Instance.AlertAsync("Requires permissions to photos library", "Permission Required");
                    }
                    permissions.Add(Permission.Storage);
                }

                // Request any permissions which we do not currently have.
                var permissionsstatus = await CrossPermissions.Current.RequestPermissionsAsync(permissions.ToArray());
                foreach (var kvp in permissionsstatus)
                {
                    if (kvp.Value != PermissionStatus.Granted)
                    {
                        await UserDialogs.Instance.AlertAsync("One or more required permissions were not granted, cannot continue", "Permission Required");
                        Application.Current.Quit();
                    }
                }

                List<string> lstSettings = new List<string>();
                if(string.IsNullOrEmpty(Settings.EndpointSetting))
                {
                    lstSettings.Add("No Azure endpoint for Azure Cognitive Services Vision defined - go to Configuration");
                }
                if (string.IsNullOrEmpty(Settings.KeyOcrSetting))
                {
                    lstSettings.Add("No key for Azure Cognitive Services Vision defined - go to Configuration");
                }
                if (string.IsNullOrEmpty(Settings.KeySpeechSetting))
                {
                    lstSettings.Add("No key for Azure Cognitive Services Speech defined - go to Configuration");
                }
                if (string.IsNullOrEmpty(Settings.LanguageSetting))
                {
                    lstSettings.Add("No language for Azure Services defined - go to Configuration");
                }
                if (string.IsNullOrEmpty(Settings.RegionSetting))
                {
                    lstSettings.Add("No Azure region defined - go to Configuration");
                }
                if(lstSettings.Count>=1)
                {
                    Settings.NoSetupDefined = true;
                    string setErr = string.Empty;
                    foreach(string s in lstSettings)
                    {
                        setErr = setErr + s + Environment.NewLine;
                    }
                    UserDialogs.Instance.Alert(setErr, "No settings defined");
                }
                else
                {
                    Settings.NoSetupDefined = false;
                }
            }
            catch (Exception ex)
            {
                await UserDialogs.Instance.AlertAsync("An exception occured requesting Application Permissions, Cannot continue", "Permission Exception: "+ex.Message);
                Application.Current.Quit();
            }

           
        }
    }
}
