using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace QuickstartSnoozableToastsEvenIfComputerOff.Helpers
{
    public static class ToastHelper
    {
        public const string ACTION_REGISTER = "register";
        public const string ACTION_SNOOZE = "snooze";
        public const string INPUT_SNOOZEMINUTES = "snoozeMinutes";

        public const string TOAST_ACTION_BACKGROUND_TASK = "ToastActionBackgroundTask";
        public const string TOAST_TIME_TRIGGER_BACKGROUND_TASK = "ToastTimeTriggerBackgroundTask";

        public const string TAG_REGISTER_REMINDER = "registerReminder";

        public static bool IsToastBackgroundTaskRegistered { get; private set; }

        public static ToastNotification CreateRegisterReminderToast()
        {
            // Generate the toast content
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Be sure to register your new device!"
                            },
                            new AdaptiveText()
                            {
                                Text = "Click this notification to start the registration process."
                            }
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastSelectionBox(INPUT_SNOOZEMINUTES)
                        {
                            DefaultSelectionBoxItemId = "15",
                            Items =
                            {
                                new ToastSelectionBoxItem("15", "15 minutes"),
                                new ToastSelectionBoxItem("1440", "1 day"),
                                new ToastSelectionBoxItem("4320", "3 days")
                            }
                        }
                    },
                    Buttons =
                    {
                        // Snoozing will activate our background task
                        new ToastButton("Snooze", ACTION_SNOOZE)
                        {
                            ActivationType = ToastActivationType.Background
                        },

                        // Dismissing will delete the notification
                        new ToastButtonDismiss()
                    }
                },
                Launch = ACTION_REGISTER,
                Scenario = ToastScenario.Reminder
            };

            // Create the toast
            var toast = new ToastNotification(toastContent.GetXml())
            {
                Tag = TAG_REGISTER_REMINDER
            };

            return toast;
        }

        public static void RegisterToastActionBackgroundTask()
        {
            try
            {
                // Register a background task to handle the toast button click action
                BackgroundTaskHelper.Register(TOAST_ACTION_BACKGROUND_TASK, new ToastNotificationActionTrigger());

                IsToastBackgroundTaskRegistered = true;
            }
            catch
            {
                // Report to telemetry
            }
        }

        public static void HandleToastActionBackgroundActivation(ToastNotificationActionTriggerDetail details)
        {
            try
            {
                switch (details.Argument)
                {
                    case ACTION_SNOOZE:
                        HandleBackgroundSnoozeActivation(details);
                        break;
                }
            }
            catch
            {
                // Report to telemetry
            }
        }

        public static void HandleTimeTriggerBackgroundActivation()
        {
            try
            {
                // Create the register reminder notification
                ToastNotification notif = CreateRegisterReminderToast();

                // And re-show it
                ToastNotificationManager.CreateToastNotifier().Show(notif);
            }
            catch
            {
                // Report to telemetry
            }
        }

        private static void HandleBackgroundSnoozeActivation(ToastNotificationActionTriggerDetail details)
        {
            string snoozeMinutesTxt = (string)details.UserInput[INPUT_SNOOZEMINUTES];
            uint snoozeMinutes = uint.Parse(snoozeMinutesTxt);

            // Register a time trigger background task to fire at the requested snooze time, which will then pop a new notification
            BackgroundTaskHelper.Register(TOAST_TIME_TRIGGER_BACKGROUND_TASK, new TimeTrigger(snoozeMinutes, oneShot: true));
        }
    }
}
