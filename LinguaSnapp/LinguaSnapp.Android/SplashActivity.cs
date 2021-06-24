using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using AndroidX.AppCompat.App;

namespace LinguaSnapp.Droid
{
    // NoHistory means that the activity is removed from the back stack
    [Activity(
        Label = "@string/app_name",
        Theme = "@style/SplashTheme",
        MainLauncher = true,
        NoHistory = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize
        )]
    public class SplashActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            new Task(() =>
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }).Start();
        }

        public override void OnBackPressed() { }
    }
}