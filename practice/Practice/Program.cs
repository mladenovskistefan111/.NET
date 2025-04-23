using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Practice
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Press any key to start device...");
            Console.ReadKey();

            IDevice device = new Device();

            device.RunDevice();

            Console.ReadKey();

        }

        public class Device : IDevice
        {
            const double Warning_Level = 27;
            const double Emergency_Level = 75;

            public double WarningTemperatureLevel => Warning_Level;

            public double EmergencyTemperatureLevel => Emergency_Level;

            public void HandleEmergency()
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Sending out notifications to emergency service personel...");
                ShutDownDevice();
                System.Console.WriteLine();
            }

            private void ShutDownDevice()
            {
                System.Console.WriteLine("Shutting down device...");
            }
            public void RunDevice()
            {
                System.Console.WriteLine("Device is running...");

                ICoolingMechanism coolingMechanism = new CoolingMechanism();
                IHeatSensor heatSensor = new HeatSensor(Warning_Level, Emergency_Level);
                IThermostat thermostat = new Thermostat(this, heatSensor, coolingMechanism);

                thermostat.RunThermostat();
            }
        }
        public class Thermostat : IThermostat
        {
            private ICoolingMechanism _coolingMechanism = null;
            private IHeatSensor _heatSensor = null;
            private IDevice _device = null;

            private const double WarningLevel = 27;
            private const double EmergencyLevel = 75;

            public Thermostat(IDevice device, IHeatSensor heatSensor, ICoolingMechanism coolingMechanism)
            {
                _device = device;
                _heatSensor = heatSensor;
                _coolingMechanism = coolingMechanism;
            }

            private void WireUpEventsToEventHandler()
            {
                _heatSensor.TemperatureReachesWarningLevelEventHandler += HeatSensor_TemperatureReachesWarningLevelEventHandler;
                _heatSensor.TemperatureReachesEmergencyLevelEventHandler += HeatSensor_TemperatureReachesEmergencyLevelEventHandler;
                _heatSensor.TemperatureFallsBelowWarningLevelEventHandler += HeatSensor_TemperatureFallsBelowWarningLevelEventHandler;
            }

            private void HeatSensor_TemperatureReachesWarningLevelEventHandler(object sender, TemperatureEventArgs e)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine();
                System.Console.WriteLine($"Warning alert!! Warning level is between {_device.WarningTemperatureLevel} and {_device.EmergencyTemperatureLevel}");
                _coolingMechanism.On();
                Console.ResetColor();
            }

            private void HeatSensor_TemperatureReachesEmergencyLevelEventHandler(object sender, TemperatureEventArgs e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine();
                System.Console.WriteLine($"Emergency alert!! Warning level is {_device.WarningTemperatureLevel} and above.");
                _device.HandleEmergency();
                Console.ResetColor();
            }

            private void HeatSensor_TemperatureFallsBelowWarningLevelEventHandler(object sender, TemperatureEventArgs e)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine();
                System.Console.WriteLine($"Information alert!! Temperature falls below warning level (Warning level is between {_device.WarningTemperatureLevel} and {_device.EmergencyTemperatureLevel})");
                _coolingMechanism.Off();
                Console.ResetColor();
            }

            public void RunThermostat()
            {
                System.Console.WriteLine("Thermostat is running...");
                WireUpEventsToEventHandler();
                _heatSensor.RunHeatSensor();
            }
        }

        public interface IThermostat
        {
            void RunThermostat();
        }
        public interface IDevice
        {
            double WarningTemperatureLevel { get; }
            double EmergencyTemperatureLevel { get; }
            void RunDevice();
            void HandleEmergency();
        }

        public class CoolingMechanism : ICoolingMechanism
        {
            public void Off()
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Switch cooling mechanism off...");
                System.Console.WriteLine();
            }

            public void On()
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Switch cooling mechanism on...");
                System.Console.WriteLine();
            }
        }
        public interface ICoolingMechanism
        {
            void On();
            void Off();
        }
        public class HeatSensor : IHeatSensor
        {
            double _warningLevel = 0;
            double _emergencyLevel = 0;

            bool _hasReachedWarningTemperature = false;

            protected EventHandlerList _listEventDelegates = new EventHandlerList();

            static readonly object _temperatureReachesWarningLevelKey = new object();
            static readonly object _temperatureReachesEmergencyLevelKey = new object();
            static readonly object _temperatureFallsBelowWarningLevelKey = new object();

            private double[] _temperatureData = null;

            public HeatSensor(double _warningLevel, double _emergencyLevel)
            {
                this._warningLevel = _warningLevel;
                this._emergencyLevel = _emergencyLevel;

                SeedData(); 
            }

            private void MonitorTemperature()
            {
                foreach (double temperature in _temperatureData)
                {
                    Console.ResetColor();
                    System.Console.WriteLine($"DateTime: {DateTime.Now}, Temperature: {temperature}");

                    if (temperature >= _emergencyLevel)
                    {
                        TemperatureEventArgs e = new TemperatureEventArgs
                        {
                            Temperature = temperature,
                            CurrentDateTime = DateTime.Now
                        };
                        OnTemperatureReachesEmergencyLevel(e);
                    }
                    else if (temperature >= _warningLevel)
                    {
                        _hasReachedWarningTemperature = true;
                        TemperatureEventArgs e = new TemperatureEventArgs
                        {
                            Temperature = temperature,
                            CurrentDateTime = DateTime.Now
                        };
                        OnTemperatureReachesWarningLevel(e);
                    }
                    else if (temperature < _warningLevel && _hasReachedWarningTemperature)
                    {
                        _hasReachedWarningTemperature = false;
                        TemperatureEventArgs e = new TemperatureEventArgs
                        {
                            Temperature = temperature,
                            CurrentDateTime = DateTime.Now
                        };
                        OnTemperatureFallsBelowWarningLevel(e);
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            }
            private void SeedData()
            {
                _temperatureData = new double[] {16, 17, 18, 19, 20, 24, 26, 29, 22, 24, 23, 21, 25, 65, 96, };
            }

            protected void OnTemperatureReachesWarningLevel(TemperatureEventArgs e)
            {
                EventHandler<TemperatureEventArgs> handler = (EventHandler<TemperatureEventArgs>)_listEventDelegates[_temperatureReachesWarningLevelKey];

                if (handler != null)
                {
                    handler(this, e);
                }
            }

              protected void OnTemperatureReachesEmergencyLevel(TemperatureEventArgs e)
            {
                EventHandler<TemperatureEventArgs> handler = (EventHandler<TemperatureEventArgs>)_listEventDelegates[_temperatureReachesEmergencyLevelKey];

                if (handler != null)
                {
                    handler(this, e);
                }
            }

              protected void OnTemperatureFallsBelowWarningLevel(TemperatureEventArgs e)
            {
                EventHandler<TemperatureEventArgs> handler = (EventHandler<TemperatureEventArgs>)_listEventDelegates[_temperatureFallsBelowWarningLevelKey];

                if (handler != null)
                {
                    handler(this, e);
                }
            }

            event EventHandler<TemperatureEventArgs> IHeatSensor.TemperatureReachesEmergencyLevelEventHandler
            {
                add
                {
                    _listEventDelegates.AddHandler(_temperatureReachesEmergencyLevelKey, value);
                }

                remove
                {
                    _listEventDelegates.RemoveHandler(_temperatureReachesEmergencyLevelKey, value);
                }
            }

            event EventHandler<TemperatureEventArgs> IHeatSensor.TemperatureReachesWarningLevelEventHandler
            {
                add
                {
                    _listEventDelegates.AddHandler(_temperatureReachesWarningLevelKey, value);
                }

                remove
                {
                    _listEventDelegates.RemoveHandler(_temperatureReachesWarningLevelKey, value);
                }
            }

            event EventHandler<TemperatureEventArgs> IHeatSensor.TemperatureFallsBelowWarningLevelEventHandler
            {
                add
                {
                    _listEventDelegates.AddHandler(_temperatureFallsBelowWarningLevelKey, value);
                }

                remove
                {
                    _listEventDelegates.RemoveHandler(_temperatureFallsBelowWarningLevelKey, value);
                }
            }

            public void RunHeatSensor()
            {
                Console.WriteLine("Heat sensor is running...");
                MonitorTemperature();
            }
        }
        public interface IHeatSensor
        {
            event EventHandler<TemperatureEventArgs> TemperatureReachesEmergencyLevelEventHandler;
            event EventHandler<TemperatureEventArgs> TemperatureReachesWarningLevelEventHandler;
            event EventHandler<TemperatureEventArgs> TemperatureFallsBelowWarningLevelEventHandler;

            void RunHeatSensor();
        }

        public class TemperatureEventArgs : EventArgs
        {
            public double Temperature { get; set; }
            public DateTime CurrentDateTime { get; set; }
        }
    }
}