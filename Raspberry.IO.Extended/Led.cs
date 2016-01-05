using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raspberry.IO.GeneralPurpose;

namespace Raspberry.IO.Extended
{
    public class Led
    {
      
        
       
        public Led (OutputPinConfiguration outputPin, IGpioConnectionDriver driver = null)
        {
            PinConfig = outputPin;
            Driver = driver ?? GpioConnectionSettings.DefaultDriver;
            Driver.Allocate(PinConfig.Pin, PinDirection.Output);

        }
        
        public Led(ProcessorPin pin , IGpioConnectionDriver driver = null): this( new OutputPinConfiguration(pin), driver)
        {
        }

        public IGpioConnectionDriver Driver { get; private set; }

        public OutputPinConfiguration PinConfig
        {
            get; private set;
        }
        public void On()
        {
            this.State = true;
        }
        public void Off()
        {
            this.State = false;
        }
        public void Toggle()
        {
            var state = Driver.Read(PinConfig.Pin);
            Driver.Write(PinConfig.Pin, !state);
        }
        public bool State
        {
            get
            {
                return Driver.Read(PinConfig.Pin);
            }
            set
            {
                Driver.Write(PinConfig.Pin, value);
            }
        }

        public void Blink ( TimeSpan duration , TimeSpan? speed = null)
        {
            speed = speed ??  TimeSpan.FromMilliseconds(200);
            var max = DateTime.Now.Add(duration);
            while ( DateTime.Now < max)
            {
                Toggle();
                Raspberry.Timers.Timer.Sleep(speed.Value);
            }
            Off();
        }

    }
}
