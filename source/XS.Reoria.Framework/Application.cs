using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XS.Reoria.Framework
{
    public class Application : IDisposable
    {
        private volatile ApplicationState state;

        public ApplicationState State
        {
            get
            {
                return this.state;
            }

            private set
            {
                this.state = value;
            }
        }

        public bool IsAlive
        {
            get
            {
                return (this.State != ApplicationState.Stopped);
            }
        }

        public bool IsStarting
        {
            get
            {
                return (this.State == ApplicationState.Starting);
            }
        }

        public bool IsRunning
        {
            get
            {
                return (this.State == ApplicationState.Running);
            }
        }

        public bool IsStopping
        {
            get
            {
                return (this.State == ApplicationState.Stopping);
            }
        }

        public bool IsPaused
        {
            get
            {
                return (this.State == ApplicationState.Paused);
            }
        }

        public bool IsPausing
        {
            get
            {
                return (this.State == ApplicationState.Pausing);
            }
        }

        public bool IsResuming
        {
            get
            {
                return (this.State == ApplicationState.Resuming);
            }
        }

        public Application()
        {

        }

        public void Dispose()
        {
            this.Kill();
        }

        public void Start()
        {
            if (!this.IsAlive)
            {
                this.loop();
            }
        }

        public void Stop()
        {
            if (this.IsAlive)
            {
                this.State = ApplicationState.Stopping;
            }
        }

        public void Pause()
        {
            if (this.IsAlive && this.IsRunning)
            {
                this.State = ApplicationState.Pausing;
            }
        }

        public void Resume()
        {
            if (this.IsAlive && this.IsPaused)
            {
                this.State = ApplicationState.Resuming;
            }
        }

        public void Kill()
        {
            if (this.IsAlive)
            {
                this.State = ApplicationState.Stopped;
            }
        }

        private void loop()
        {
            this.State = ApplicationState.Starting;

            while (this.IsAlive)
            {
                if (this.IsRunning)
                {
                    
                }

                if (this.IsPaused)
                {

                }

                if (this.IsStarting)
                {
                    Console.WriteLine("Appliction starting.");

                    this.State = ApplicationState.Running;
                }

                if (this.IsResuming)
                {
                    Console.WriteLine("Appliction resuming.");

                    this.State = ApplicationState.Running;
                }

                if (this.IsPausing)
                {
                    Console.WriteLine("Appliction pausing.");

                    this.State = ApplicationState.Paused;
                }

                if (this.IsStopping)
                {
                    Console.WriteLine("Appliction stopping.");

                    this.State = ApplicationState.Stopped;
                }
            }
        }
    }
}