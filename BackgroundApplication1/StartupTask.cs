using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using System.Diagnostics;
// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BackgroundApplication1
{
    public sealed class StartupTask : IBackgroundTask
    {
        const short LED_PIN = 26;
        const short INTERVAL = 1;
        private GpioPin pin;
        private GpioPinValue pinValue;
        private ThreadPoolTimer blinktimer;

        private BackgroundTaskDeferral deferral = null;
        private bool canceled;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("started");
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(onCanceled);
            deferral = taskInstance.GetDeferral();
            this.initGPIO();
            blinktimer_tick(null);
            blinktimer = ThreadPoolTimer.CreatePeriodicTimer(blinktimer_tick, TimeSpan.FromMilliseconds(2000));
        }

        private void onCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            canceled = true;
        }

        private void blinktimer_tick(object state)
        {
            Debug.WriteLine("blinktimer_tick");
            //return;
            if (!canceled)
            {
                // Toggele Pin Value
                if ((pinValue == GpioPinValue.High))
                {
                    pinValue = GpioPinValue.Low;
                }
                else
                {
                    pinValue = GpioPinValue.High;
                }

                pin.Write(pinValue);
            }
            else
            {
                // if the task is cancelled
                // Stop the timer And finish execution
                blinktimer.Cancel();
                deferral.Complete();
            }
        }

        private void initGPIO()
        {
            // Get the default GPIO controller for the system
            Debug.WriteLine("initGPIO");
            var gpio = GpioController.GetDefault();
            if (!(gpio == null))
            {
                // Open a connection to the GPIO Pin
                pin = gpio.OpenPin(LED_PIN);
                Debug.WriteLine(" pinValue = GpioPinValue.High;");
                // Set the pin for output
                pin.SetDriveMode(GpioPinDriveMode.Output);

                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
                Debug.WriteLine(" end of initGPIO ");
            }
            else
            {
                Debug.WriteLine("initGPIO: null");
            }

        }
    }
}
