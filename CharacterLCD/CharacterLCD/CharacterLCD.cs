﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Callant
{
    public class CharacterLCD
    {
        private GpioController gpio = null;
        private GpioPin gpRS = null; // 25
        private int RS = 25;
        private GpioPin gpE = null; // 24
        private int E = 24;
        private const int DATA0 = 27;
        private const int DATA1 = 22;
        private const int DATA2 = 5;
        private const int DATA3 = 6;
        private const int DATA4 = 13;
        private const int DATA5 = 26;
        private const int DATA6 = 18;
        private const int DATA7 = 16;

        private GpioPin[] gpData = null;
        private int[] DATA = new int[8] { 27, 22, 5, 6, 13, 26, 18, 16 };


        public CharacterLCD(int rs = 25, int e = 24, int data0 = DATA0, int data1 = DATA1, int data2 = DATA2, int data3 = DATA3, int data4 = DATA4, int data5 = DATA5, int data6 = DATA6, int data7 = DATA7)
        {
            this.InitGPIO();

            this.RS = rs;
            this.E = e;
            this.DATA[0] = data0;
            this.DATA[1] = data1;
            this.DATA[2] = data2;
            this.DATA[3] = data3;
            this.DATA[4] = data4;
            this.DATA[5] = data5;
            this.DATA[6] = data6;
            this.DATA[7] = data7;

            this.InitAllPins();

            InitLCD();
        }
        private void InitGPIO()
        {
            this.gpio = GpioController.GetDefault();

            if (gpio == null)
                throw new NullReferenceException();
        }
        private void InitPin(ref GpioPin pin, int pinNr)
        {
            pin = this.gpio.OpenPin(pinNr);
            if (pin == null)
                throw new ArgumentNullException();

            pin.SetDriveMode(GpioPinDriveMode.Output);
            //pin.Write(GpioPinValue.Low);
        }
        private void InitAllPins()
        {
            this.InitPin(ref gpRS, RS);
            this.InitPin(ref gpE, E);

            gpData = new GpioPin[8];
            for (int i = 0; i < 8; i++)
            {
                this.InitPin(ref this.gpData[i], this.DATA[i]);
            }
        }
        private void SetPin(ref GpioPin pin, GpioPinValue value)
        {
            pin.Write(value);
        }

        public void InitLCD()
        {
            // FunctionSet
            this.SendInstruction(56);

            this.Wait();

            // DisplayOn
            this.SendInstruction(15);

            //this.ClearLCD();
        }
        public void ClearLCD()
        {
            this.Wait();
            this.SendInstruction(1);
            this.Wait();
        }

        //EHoogInstructie(E = I, RS = 0, RW = 0) 
        private void EHighInstruction()
        {
            //this.WriteData(0);
            this.gpE.Write(GpioPinValue.High);
            this.gpRS.Write(GpioPinValue.Low);
        }
        //ELaagInstructie(E = 0, RS = 0, RW = 0)
        private void ELowInstruction()
        {
            this.gpE.Write(GpioPinValue.Low);
            this.gpRS.Write(GpioPinValue.Low);
            //this.WriteData(0);
        }
        //EHoogData(E = I, RS = I, RW = 0)
        private void EHighData()
        {
            //this.WriteData(0);
            this.gpE.Write(GpioPinValue.High);
            this.gpRS.Write(GpioPinValue.High);
        }
        //ELaagData(E = 0, RS = I, RW = 0)
        private void ELowData()
        {
            this.gpE.Write(GpioPinValue.Low);
            this.gpRS.Write(GpioPinValue.High);
            //this.WriteData(0);
        }
        private void WriteData(short value)
        {
            //string v = Convert.ToString(value, 2).PadLeft(8, '0');

            char[] charArray = Convert.ToString(value, 2).PadLeft(8, '0').ToCharArray();
            Array.Reverse(charArray);
            //string valueString = new string(charArray);

            for (int i = 0; i < 8; i++)
            {
                char v = charArray[i];
                if (v == '1')
                {
                    // write GpioPinValue.High to pin
                    this.SetPin(ref gpData[i], GpioPinValue.High);
                }
                else
                {
                    // write GpioPinValue.Low to pin
                    this.SetPin(ref gpData[i], GpioPinValue.Low);
                }
            }
        }
        private void Wait()
        {
            Task.Delay(TimeSpan.FromMilliseconds(1));
        }
        public void Dispose()
        {
            this.gpRS.Dispose();
            this.gpE.Dispose();
            foreach (GpioPin p in gpData)
            {
                p.Dispose();
            }
        }
        private void SendInstruction(short bit)
        {
            this.EHighInstruction();
            this.Wait();
            this.WriteData(bit);
            this.Wait();
            this.ELowInstruction();
        }
        public void WriteLCD(string message)
        {
            this.ClearLCD();

            int i = 0;
            foreach (char c in message)
            {
                i++;

                //if (i == 17)
                //NewLine();

                this.EHighData();
                this.Wait();
                this.WriteData((short)c);
                this.Wait();
                this.ELowData();
                this.Wait();
            }

            //SendInstruction(31);
        }
    }
}
