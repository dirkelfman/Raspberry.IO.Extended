using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raspberry.IO.GeneralPurpose;

namespace Raspberry.IO.Extended
{
    public class Button:IDisposable
    {

        

        public Button(InputPinConfiguration outputPin, IGpioConnectionDriver driver = null)
        {
            PinConfig = outputPin;
            Driver = driver ?? GpioConnectionSettings.DefaultDriver;
            Driver.Allocate(PinConfig.Pin, PinDirection.Output);
            TokenScource=  new System.Threading.CancellationTokenSource();
            Action a = Watch;
            Task.Run(a,TokenScource.Token);
           // Task.Start(Watch);
            
        }
        public Action<bool> StatusChangedAction
        {
            get { return this.PinConfig.StatusChangedAction; }
            set { this.PinConfig.StatusChangedAction = value; }
        }
        public  CancellationTokenSource TokenScource
        {
            get; set;
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
      
        public void Dispose()
        {
            Driver.Release(this.PinConfig.Pin);
            TokenScource.Dispose();
        }
    }
}
