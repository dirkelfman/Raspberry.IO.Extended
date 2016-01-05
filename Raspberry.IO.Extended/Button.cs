using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raspberry.IO.GeneralPurpose;

namespace Raspberry.IO.Extended
{
    public class Button
    {



        public Button(InputPinConfiguration outputPin, IGpioConnectionDriver driver = null)
        {
            PinConfig = outputPin;
            Driver = driver ?? GpioConnectionSettings.DefaultDriver;
            Driver.Allocate(PinConfig.Pin, PinDirection.Output);
            
        }
        void Watch ()
        {
            while (true)
            {
                try
                {
                    var time = Driver.Time(PinConfig.Pin, PinConfig.Reversed, TimeSpan.FromSeconds(60), TimeSpan.FromMilliseconds(100));
                    if ( time > 0 )
                    {
                        if ( PinConfig.StatusChangedAction != null)
                        {
                            PinConfig.StatusChangedAction(this.State);
                        }
                    }
                }
                catch (TimeoutException)
                {

                }
            }
        }
        public Button(ProcessorPin pin, IGpioConnectionDriver driver = null) : this(new InputPinConfiguration(pin), driver)
        {
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

        public IGpioConnectionDriver Driver { get; private set; }

        public InputPinConfiguration PinConfig
        {
            get; private set;
        }
        public void On()
        {
            Driver.Write(PinConfig.Pin, true);
        }
        public void Off()
        {
            Driver.Write(PinConfig.Pin, false);
        }
        public void Toggle()
        {
            var state = Driver.Read(PinConfig.Pin);
            Driver.Write(PinConfig.Pin, !state);
        }
        public void Blink(TimeSpan duration, TimeSpan? speed = null)
        {
            speed = speed ?? TimeSpan.FromMilliseconds(200);
            var max = DateTime.Now.Add(duration);
            while (DateTime.Now < max)
            {
                Toggle();
                Raspberry.Timers.Timer.Sleep(speed.Value);
            }
            Off();
        }

    }
}
