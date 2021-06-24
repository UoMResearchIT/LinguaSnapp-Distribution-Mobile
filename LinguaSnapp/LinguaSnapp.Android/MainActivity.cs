using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace LinguaSnapp.Droid
{
    [Activity(
        Label = "@string/app_name",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize
        )]
    public class MainActivity : MDS.Essentials.Platforms.Droid.EssentialActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            try
            {
                PdfSharp.Xamarin.Forms.Droid.Platform.Init();
            }
            catch (Exception) { }
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            foreach (var s in permissions) System.Diagnostics.Debug.WriteLine($"{s}");

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override int GetNotificationIconId()
        {
            return Resource.Drawable.notification_template_icon_bg;
        }

        public override int GetAppCompatDialogStyleId()
        {
            return Resource.Style.AppCompatDialogStyle;
        }

        public override int GetAppCompatBottomSheetStyleId()
        {
            return Resource.Style.AppCompatBottomSheetStyle;
        }
    }
}