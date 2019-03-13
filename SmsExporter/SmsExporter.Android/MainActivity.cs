using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SmsExporter.Droid
{
    [Activity(Label = "短信导出", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App(new SmsReader()
            {
                GetCursorFunc = (uri, columns, selection) => this.ManagedQuery(uri, columns, selection, null, null)
            }));
            
            //请求权限
            RequestPermissions(new string[]{
                Android.Manifest.Permission.ReadSms,
                Android.Manifest.Permission.WriteExternalStorage
            }, 0);
        }
    }
}