using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Callant;
using Windows.Devices.AllJoyn;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace CharacterLCD.AllJoynServer
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        ThreadPoolTimer timer;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to start one or more asynchronous methods 
            //
            _deferral = taskInstance.GetDeferral();

            CharacterLCDProducer producer = new CharacterLCDProducer(new AllJoynBusAttachment());
            producer.Service = new CharacterLCDService();
            producer.Start();
            timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(50000));
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            
        }
    }
}
