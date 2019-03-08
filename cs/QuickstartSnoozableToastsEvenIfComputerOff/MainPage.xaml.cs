using QuickstartSnoozableToastsEvenIfComputerOff.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QuickstartSnoozableToastsEvenIfComputerOff
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ButtonSendToast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ToastHelper.IsToastBackgroundTaskRegistered)
                {
                    throw new InvalidOperationException("Toast background task must be registered first. Registration must have failed for some reason (since this happens in App.xaml.cs)");
                }

                // Create the toast notification
                ToastNotification notif = ToastHelper.CreateRegisterReminderToast();

                // And show it
                ToastNotificationManager.CreateToastNotifier().Show(notif);
            }
            catch
            {
                // Report back to your telemetry
            }
        }
    }
}
