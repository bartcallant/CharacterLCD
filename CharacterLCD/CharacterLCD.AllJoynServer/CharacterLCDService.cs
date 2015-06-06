using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Callant;
using Windows.Foundation;
using System.Diagnostics;

namespace CharacterLCD.AllJoynServer
{
    public sealed class CharacterLCDService : ICharacterLCDService
    {
        public IAsyncOperation<CharacterLCDSendResult> SendAsync(AllJoynMessageInfo info, string interface_message)
        {
            Task<CharacterLCDSendResult> task = new Task<CharacterLCDSendResult>(() =>
            {
                Callant.CharacterLCD lcd = new Callant.CharacterLCD();
                lcd.WriteLCD(interface_message);
                return CharacterLCDSendResult.CreateSuccessResult();
            });

            task.Start();
            return task.AsAsyncOperation();
        }
    }
}
