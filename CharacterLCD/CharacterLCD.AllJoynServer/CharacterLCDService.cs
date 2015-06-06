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
        Callant.CharacterLCD lcd = null;
        public CharacterLCDService()
        {
            lcd = new Callant.CharacterLCD();
        }
        public IAsyncOperation<CharacterLCDSendResult> SendAsync(AllJoynMessageInfo info, string interface_message)
        {
            Task<CharacterLCDSendResult> task = new Task<CharacterLCDSendResult>(() =>
            {
                lcd.WriteLCD(interface_message);
                return CharacterLCDSendResult.CreateSuccessResult();
            });

            task.Start();
            return task.AsAsyncOperation();
        }
    }
}
