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

namespace Recognizer
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            
            //CheckPermissions();

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
                        await UserDialogs.Instance.AlertAsync("Requires permissions", "Permission Required");
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
            }
            catch (Exception ex)
            {
                await UserDialogs.Instance.AlertAsync("An exception occured requesting Application Permissions, Cannot continue", "Permission Exception: "+ex.Message);
                Application.Current.Quit();
            }

           
        }
    }
}
