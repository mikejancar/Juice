using System;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Power;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Juice
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            RequestAggregateBatteryReport();
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
        }

        private void RequestAggregateBatteryReport()
        {
            var aggBattery = Battery.AggregateBattery;
            var report = aggBattery.GetReport();

            UpdateReportUI(BatteryReportPanel, report, aggBattery.DeviceId);
        }

        private void UpdateReportUI(StackPanel sp, BatteryReport report, string DeviceID)
        {
            BatteryStatus.Text = string.Format("Status: {0}", report.Status);

            if ((report.FullChargeCapacityInMilliwattHours == null) ||
                (report.RemainingCapacityInMilliwattHours == null))
            {
                BatteryBar.IsEnabled = false;
                BatteryCharge.Text = "N/A";
            }
            else
            {
                BatteryBar.IsEnabled = true;
                BatteryBar.Maximum = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
                BatteryBar.Value = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);

                double percentCharged = (BatteryBar.Value / BatteryBar.Maximum) * 100;
                BatteryBar.Foreground = SetBarColor(percentCharged);
                BatteryCharge.Text = string.Format("{0:F2}%", percentCharged);
            }
        }

        private Brush SetBarColor(double percentCharged)
        {
            if (percentCharged > 50)
            {
                return new SolidColorBrush(Color.FromArgb(255, 15, 105, 209));
            }
            else if (percentCharged > 20)
            {
                return new SolidColorBrush(Color.FromArgb(255, 242, 236, 44));
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(255, 242, 44, 44));
            }
        }

        async private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RequestAggregateBatteryReport();
            });
        }
    }
}