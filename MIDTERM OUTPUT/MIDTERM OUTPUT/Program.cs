using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SmartHomeControlPanel
{
    // Custom exception for invalid operations
    public class InvalidOperationException : Exception
    {
        public InvalidOperationException(string message) : base(message) { }
    }

    // Interface for device behavior
    public interface IDevice
    {
        string DeviceID { get; }
        string SerialNumber { get; }
        string Name { get; set; }
        string Description { get; set; }
        string Location { get; set; }
        bool IsOn { get; set; }
        void TurnOn();
        void TurnOff();
        void UpdateConfiguration(string setting, object value);
    }

    // Abstract base class for devices
    public abstract class Device : IDevice
    {
        public string DeviceID { get; }
        public string SerialNumber { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsOn { get; set; }
        public bool IsLocked { get; set; }

        protected Device(string deviceId, string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Device ID cannot be empty");
            if (string.IsNullOrWhiteSpace(serialNumber))
                throw new ArgumentException("Serial number cannot be empty");

            DeviceID = deviceId;
            SerialNumber = serialNumber;
        }

        public virtual void TurnOn()
        {
            if (IsOn)
                throw new InvalidOperationException($"{Name} is already ON.");
            IsOn = true;
            Console.WriteLine($"{Name} in {Location} is now ON. (Device ID: {DeviceID})");
        }

        public virtual void TurnOff()
        {
            if (!IsOn)
                throw new InvalidOperationException($"{Name} is already OFF.");
            IsOn = false;
            Console.WriteLine($"{Name} in {Location} is now OFF. (Device ID: {DeviceID})");
        }

        public abstract void UpdateConfiguration(string setting, object value);
    }

    // Concrete device classes
    public class SmartLight : Device
    {
        public int Brightness { get; set; }
        public string Color { get; set; }

        public SmartLight(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            Brightness = 50;
            Color = "White";
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "Brightness" when value is int brightness:
                    if (brightness < 0 || brightness > 100)
                        throw new InvalidOperationException("Brightness must be between 0 and 100.");
                    Brightness = brightness;
                    Console.WriteLine($"{Name} brightness set to {Brightness}.");
                    break;

                case "Color" when value is string color:
                    if (!new[] { "White", "Warm", "Red", "Green", "Blue", "Yellow" }.Contains(color))
                        throw new InvalidOperationException("Invalid color option.");
                    Color = color;
                    Console.WriteLine($"{Name} color set to {Color}.");
                    break;

                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;
            }
        }
    }

    public class SmartThermostat : Device
    {
        public int Temperature { get; set; }
        public string Mode { get; set; } // "Heat", "Cool", "Auto", "Off"
        public int FanSpeed { get; set; } // 1-3

        public SmartThermostat(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            Temperature = 22;
            Mode = "Auto";
            FanSpeed = 2;
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "Temperature" when value is int temperature:
                    if (temperature < 16 || temperature > 30)
                        throw new InvalidOperationException("Temperature must be between 16°C and 30°C.");
                    Temperature = temperature;
                    Console.WriteLine($"{Name} temperature set to {Temperature}°C.");
                    break;

                case "Mode" when value is string mode:
                    if (!new[] { "Heat", "Cool", "Auto", "Off" }.Contains(mode))
                        throw new InvalidOperationException("Mode must be Heat, Cool, Auto, or Off.");
                    Mode = mode;
                    Console.WriteLine($"{Name} mode set to {Mode}.");
                    break;

                case "FanSpeed" when value is int speed:
                    if (speed < 1 || speed > 3)
                        throw new InvalidOperationException("Fan speed must be between 1 and 3.");
                    FanSpeed = speed;
                    Console.WriteLine($"{Name} fan speed set to {FanSpeed}.");
                    break;

                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;
            }
        }
    }

    public class SmartDoor : Device
    {
        public bool IsLocked { get; set; }
        public bool IsOpen { get; set; }

        public SmartDoor(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            IsLocked = true;
            IsOpen = false;
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;

                case "IsOpen" when value is bool isOpen:
                    if (IsLocked && isOpen)
                        throw new InvalidOperationException("Cannot open a locked door.");
                    IsOpen = isOpen;
                    Console.WriteLine($"{Name} is {(IsOpen ? "open" : "closed")}.");
                    break;
            }
        }
    }

    public class SmartGate : Device
    {
        public bool IsLocked { get; set; }
        public bool IsOpen { get; set; }

        public SmartGate(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            IsLocked = true;
            IsOpen = false;
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;

                case "IsOpen" when value is bool isOpen:
                    if (IsLocked && isOpen)
                        throw new InvalidOperationException("Cannot open a locked gate.");
                    IsOpen = isOpen;
                    Console.WriteLine($"{Name} is {(IsOpen ? "open" : "closed")}.");
                    break;
            }
        }
    }

    public class SecurityCamera : Device
    {
        public bool IsRecording { get; set; }
        public bool MotionDetectionEnabled { get; set; }
        public int RecordingQuality { get; set; } // 1-3 where 1=Low, 2=Medium, 3=High
        public string CurrentView { get; set; }

        public SecurityCamera(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            IsRecording = false;
            MotionDetectionEnabled = true;
            RecordingQuality = 2; // Medium by default
            CurrentView = "Default";
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "IsRecording" when value is bool recording:
                    IsRecording = recording;
                    Console.WriteLine($"{Name} recording {(IsRecording ? "started" : "stopped")}.");
                    break;

                case "MotionDetectionEnabled" when value is bool enabled:
                    MotionDetectionEnabled = enabled;
                    Console.WriteLine($"{Name} motion detection {(MotionDetectionEnabled ? "enabled" : "disabled")}.");
                    break;

                case "RecordingQuality" when value is int quality:
                    if (quality < 1 || quality > 3)
                        throw new InvalidOperationException("Recording quality must be between 1 (Low) and 3 (High).");
                    RecordingQuality = quality;
                    Console.WriteLine($"{Name} recording quality set to {(RecordingQuality == 1 ? "Low" : RecordingQuality == 2 ? "Medium" : "High")}.");
                    break;

                case "CurrentView" when value is string view:
                    if (string.IsNullOrWhiteSpace(view))
                        throw new InvalidOperationException("View cannot be empty.");
                    CurrentView = view;
                    Console.WriteLine($"{Name} view set to {CurrentView}.");
                    break;

                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;
            }
        }

        public void PanTilt(int panAngle, int tiltAngle)
        {
            if (panAngle < -90 || panAngle > 90)
                throw new InvalidOperationException("Pan angle must be between -90 and 90 degrees.");
            if (tiltAngle < -45 || tiltAngle > 45)
                throw new InvalidOperationException("Tilt angle must be between -45 and 45 degrees.");

            Console.WriteLine($"{Name} camera panned to {panAngle}° and tilted to {tiltAngle}°.");
        }
    }

    public class AlarmSystem : Device
    {
        public bool AlarmTriggered { get; private set; }
        public string AlarmMode { get; set; } // "Off", "Home", "Away"
        public List<string> ConnectedSensors { get; }

        public AlarmSystem(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            AlarmTriggered = false;
            AlarmMode = "Off";
            ConnectedSensors = new List<string>();
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "AlarmMode" when value is string mode:
                    if (!new[] { "Off", "Home", "Away" }.Contains(mode))
                        throw new InvalidOperationException("Alarm mode must be Off, Home, or Away.");
                    AlarmMode = mode;
                    Console.WriteLine($"{Name} alarm mode set to {AlarmMode}.");
                    break;

                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;
            }
        }

        public void TriggerAlarm()
        {
            if (AlarmMode == "Off")
                throw new InvalidOperationException("Cannot trigger alarm when system is off.");

            AlarmTriggered = true;
            Console.WriteLine($"ALARM TRIGGERED on {Name}! Sounding siren and notifying authorities.");
        }

        public void SilenceAlarm()
        {
            AlarmTriggered = false;
            Console.WriteLine($"{Name} alarm silenced.");
        }

        public void AddSensor(string sensorId)
        {
            if (ConnectedSensors.Contains(sensorId))
                throw new InvalidOperationException($"Sensor {sensorId} is already connected.");

            ConnectedSensors.Add(sensorId);
            Console.WriteLine($"Sensor {sensorId} added to {Name}.");
        }

        public void RemoveSensor(string sensorId)
        {
            if (!ConnectedSensors.Contains(sensorId))
                throw new InvalidOperationException($"Sensor {sensorId} not found in connected sensors.");

            ConnectedSensors.Remove(sensorId);
            Console.WriteLine($"Sensor {sensorId} removed from {Name}.");
        }
    }

    public class RobotMop : Device
    {
        public int BatteryLevel { get; set; }
        public string CurrentMode { get; set; }
        public int WaterTankLevel { get; set; }

        public RobotMop(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            BatteryLevel = 100;
            CurrentMode = "Standby";
            WaterTankLevel = 100;
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "BatteryLevel" when value is int level:
                    if (level < 0 || level > 100)
                        throw new InvalidOperationException("Battery level must be between 0 and 100.");
                    BatteryLevel = level;
                    Console.WriteLine($"{Name} battery level set to {BatteryLevel}%.");
                    break;

                case "CurrentMode" when value is string mode:
                    if (!new[] { "Standby", "Cleaning", "Charging", "Returning" }.Contains(mode))
                        throw new InvalidOperationException("Invalid mode. Must be Standby, Cleaning, Charging, or Returning.");
                    CurrentMode = mode;
                    Console.WriteLine($"{Name} mode set to {CurrentMode}.");
                    break;

                case "WaterTankLevel" when value is int level:
                    if (level < 0 || level > 100)
                        throw new InvalidOperationException("Water tank level must be between 0 and 100.");
                    WaterTankLevel = level;
                    Console.WriteLine($"{Name} water tank level set to {WaterTankLevel}%.");
                    break;

                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;
            }
        }

        public void StartCleaning(string cleaningMode)
        {
            if (BatteryLevel < 20)
                throw new InvalidOperationException("Battery too low to start cleaning.");
            if (WaterTankLevel < 10)
                throw new InvalidOperationException("Water tank too low to start cleaning.");

            CurrentMode = "Cleaning";
            Console.WriteLine($"{Name} started cleaning in {cleaningMode} mode.");
        }
    }

    public class SmartTv : Device
    {
        public int Volume { get; set; }
        public int Channel { get; set; }
        public bool IsMuted { get; set; }
        public string CurrentInput { get; set; }

        public SmartTv(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            Volume = 50;
            Channel = 1;
            IsMuted = false;
            CurrentInput = "HDMI1";
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "Volume" when value is int volume:
                    if (volume < 0 || volume > 100)
                        throw new InvalidOperationException("Volume must be between 0 and 100.");
                    Volume = volume;
                    IsMuted = false;
                    Console.WriteLine($"{Name} volume set to {Volume}.");
                    break;

                case "Channel" when value is int channel:
                    if (channel < 1 || channel > 999)
                        throw new InvalidOperationException("Channel must be between 1 and 999.");
                    Channel = channel;
                    Console.WriteLine($"{Name} channel set to {Channel}.");
                    break;

                case "IsMuted" when value is bool isMuted:
                    IsMuted = isMuted;
                    Console.WriteLine($"{Name} is {(IsMuted ? "muted" : "unmuted")}.");
                    break;

                case "CurrentInput" when value is string input:
                    if (!new[] { "HDMI1", "HDMI2", "AV", "TV", "USB" }.Contains(input))
                        throw new InvalidOperationException("Invalid input source.");
                    CurrentInput = input;
                    Console.WriteLine($"{Name} input set to {CurrentInput}.");
                    break;

                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;
            }
        }

        public void PlayContent(string contentName)
        {
            Console.WriteLine($"{Name} is now playing {contentName} on {CurrentInput}.");
        }
    }

    public class SmartPetFeeder : Device
    {
        public int FoodLevel { get; set; }
        public DateTime NextFeedingTime { get; set; }
        public int PortionSize { get; set; }
        public int FeedingFrequency { get; set; } // Times per day

        public SmartPetFeeder(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            FoodLevel = 100;
            NextFeedingTime = DateTime.Now.AddHours(6);
            PortionSize = 1;
            FeedingFrequency = 2;
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "FoodLevel" when value is int level:
                    if (level < 0 || level > 100)
                        throw new InvalidOperationException("Food level must be between 0 and 100.");
                    FoodLevel = level;
                    Console.WriteLine($"{Name} food level set to {FoodLevel}%.");
                    break;

                case "NextFeedingTime" when value is DateTime time:
                    if (time < DateTime.Now)
                        throw new InvalidOperationException("Feeding time cannot be in the past.");
                    NextFeedingTime = time;
                    Console.WriteLine($"{Name} next feeding time set to {NextFeedingTime:t}.");
                    break;

                case "PortionSize" when value is int size:
                    if (size < 1 || size > 5)
                        throw new InvalidOperationException("Portion size must be between 1 and 5.");
                    PortionSize = size;
                    Console.WriteLine($"{Name} portion size set to {PortionSize}.");
                    break;

                case "FeedingFrequency" when value is int frequency:
                    if (frequency < 1 || frequency > 6)
                        throw new InvalidOperationException("Feeding frequency must be between 1 and 6 times per day.");
                    FeedingFrequency = frequency;
                    Console.WriteLine($"{Name} will now feed {FeedingFrequency} times per day.");
                    break;

                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;
            }
        }

        public void DispenseFood()
        {
            if (FoodLevel < PortionSize * 10)
                throw new InvalidOperationException("Not enough food in the container.");

            Console.WriteLine($"{Name} dispensed {PortionSize} portion(s) of food.");
            FoodLevel -= PortionSize * 10;
            NextFeedingTime = DateTime.Now.AddHours(24 / FeedingFrequency);
        }
    }

    public class GardenSprinkler : Device
    {
        public int WaterFlowRate { get; set; } // 1-10
        public string Schedule { get; set; } // "Morning", "Evening", "Custom"
        public DateTime? CustomScheduleTime { get; set; }
        public int DurationMinutes { get; set; } // 1-60 minutes

        public GardenSprinkler(string deviceId, string serialNumber) : base(deviceId, serialNumber)
        {
            WaterFlowRate = 5;
            Schedule = "Morning";
            DurationMinutes = 15;
        }

        public override void UpdateConfiguration(string setting, object value)
        {
            switch (setting)
            {
                case "WaterFlowRate" when value is int rate:
                    if (rate < 1 || rate > 10)
                        throw new InvalidOperationException("Water flow rate must be between 1 and 10.");
                    WaterFlowRate = rate;
                    Console.WriteLine($"{Name} water flow rate set to {WaterFlowRate}.");
                    break;

                case "Schedule" when value is string schedule:
                    if (!new[] { "Morning", "Evening", "Custom" }.Contains(schedule))
                        throw new InvalidOperationException("Schedule must be Morning, Evening, or Custom.");
                    Schedule = schedule;
                    Console.WriteLine($"{Name} schedule set to {Schedule}.");
                    break;

                case "CustomScheduleTime" when value is DateTime time:
                    CustomScheduleTime = time;
                    Console.WriteLine($"{Name} custom schedule time set to {time:t}.");
                    break;

                case "DurationMinutes" when value is int duration:
                    if (duration < 1 || duration > 60)
                        throw new InvalidOperationException("Duration must be between 1 and 60 minutes.");
                    DurationMinutes = duration;
                    Console.WriteLine($"{Name} duration set to {DurationMinutes} minutes.");
                    break;

                case "IsLocked" when value is bool isLocked:
                    IsLocked = isLocked;
                    Console.WriteLine($"{Name} is {(IsLocked ? "locked" : "unlocked")}.");
                    break;
            }
        }

        public void StartWatering()
        {
            if (!IsOn)
                throw new InvalidOperationException($"{Name} must be turned on before watering.");

            Console.WriteLine($"{Name} started watering with flow rate {WaterFlowRate} for {DurationMinutes} minutes.");
        }

        public void StopWatering()
        {
            Console.WriteLine($"{Name} stopped watering.");
        }
    }

    // Scheduler class for device automation
    public class Scheduler
    {
        public static async Task ScheduleTask(Device device, Action<Device> action, TimeSpan delay)
        {
            await Task.Delay(delay);
            action(device);
        }
    }

    // Control Panel class
    public class ControlPanel
    {
        private List<Device> _devices;

        public ControlPanel()
        {
            _devices = new List<Device>();
        }

        public void AddDevice(Device device)
        {
            if (_devices.Exists(d => d.DeviceID == device.DeviceID))
            {
                throw new InvalidOperationException($"Device with ID {device.DeviceID} already exists.");
            }
            _devices.Add(device);
            Console.WriteLine($"{device.Name} (ID: {device.DeviceID}, SN: {device.SerialNumber}) added to {device.Location}.");
        }

        public void RemoveDevice(string deviceId)
        {
            var device = _devices.Find(d => d.DeviceID == deviceId);
            if (device != null)
            {
                _devices.Remove(device);
                Console.WriteLine($"{device.Name} (ID: {device.DeviceID}) removed from {device.Location}.");
            }
            else
            {
                Console.WriteLine($"Device with ID {deviceId} not found.");
            }
        }

        public void ListDevicesByLocation(string location)
        {
            Console.WriteLine($"Devices in {location}:");
            foreach (var device in _devices)
            {
                if (device.Location == location)
                {
                    Console.WriteLine($"- {device.Name} (ID: {device.DeviceID}, SN: {device.SerialNumber}) ({(device.IsOn ? "ON" : "OFF")})");
                }
            }
        }

        public void GroupControlByType<T>(bool turnOn) where T : Device
        {
            foreach (var device in _devices)
            {
                if (device is T)
                {
                    try
                    {
                        if (turnOn) device.TurnOn();
                        else device.TurnOff();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }

        public void UpdateDeviceDetails(string deviceId, string newName, string newDescription, string newLocation)
        {
            var device = _devices.Find(d => d.DeviceID == deviceId);
            if (device != null)
            {
                device.Name = newName;
                device.Description = newDescription;
                device.Location = newLocation;
                Console.WriteLine($"{deviceId} details updated to: Name={newName}, Description={newDescription}, Location={newLocation}.");
            }
            else
            {
                Console.WriteLine($"Device with ID {deviceId} not found.");
            }
        }

        public void ListAllDevices()
        {
            Console.WriteLine("All Devices:");
            foreach (var device in _devices)
            {
                Console.WriteLine($"- {device.Name} ({device.Description}) in {device.Location} (ID: {device.DeviceID}, SN: {device.SerialNumber}) ({(device.IsOn ? "ON" : "OFF")})");
            }
        }

        public async Task ScheduleDeviceAction(string deviceId, Action<Device> action, TimeSpan delay)
        {
            var device = _devices.Find(d => d.DeviceID == deviceId);
            if (device != null)
            {
                Console.WriteLine($"Scheduling action for {device.Name} (ID: {device.DeviceID}) in {delay.TotalSeconds} seconds...");
                await Scheduler.ScheduleTask(device, action, delay);
            }
            else
            {
                Console.WriteLine($"Device with ID {deviceId} not found.");
            }
        }

        public Device GetDeviceById(string deviceId)
        {
            return _devices.Find(d => d.DeviceID == deviceId);
        }
    }

    // Main program
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Smart Home Control Panel Initializing...\n");

            // Create control panel
            var controlPanel = new ControlPanel();

            // Add devices with unique IDs and serial numbers
            var livingRoomLight = new SmartLight("LIGHT-001", "SN-12345")
            {
                Name = "Living Room Light",
                Description = "Smart LED Light",
                Location = "Living Room",
                Brightness = 50
            };

            var frontDoor = new SmartDoor("DOOR-001", "SN-DOOR-54321")
            {
                Name = "Front Door",
                Description = "Smart Lock Door",
                Location = "Porch",
                IsLocked = true
            };

            var livingRoomThermostat = new SmartThermostat("THERM-001", "SN-THERM-98765")
            {
                Name = "Living Room Thermostat",
                Description = "Split-type Air Conditioner",
                Location = "Living Room",
                Temperature = 22
            };

            var frontGate = new SmartGate("GATE-001", "SN-GATE-13579")
            {
                Name = "Garage Gate",
                Description = "Smart Gate",
                Location = "Garage",
                IsLocked = false
            };

            var securityCamera = new SecurityCamera("CAM-001", "SN-CAM-13331")
            {
                Name = "Front Security Camera",
                Description = "4K Outdoor Security Camera with Night Vision",
                Location = "Front Door"
            };

            var alarmSystem = new AlarmSystem("ALS-001", "SN-ALS-12221")
            {
                Name = "Home Alarm System",
                Description = "Whole-house security with motion sensors",
                Location = "Hallway"
            };

            var robotMop = new RobotMop("MOP-001", "SN-MOP-24680")
            {
                Name = "Cleaning Robot",
                Description = "Smart Mopping Robot",
                Location = "Living Room"
            };

            var smartTv = new SmartTv("TV-001", "SN-TV-11223")
            {
                Name = "Living Room TV",
                Description = "65\" 4K Smart TV",
                Location = "Living Room"
            };

            var petFeeder = new SmartPetFeeder("FEED-001", "SN-FEED-33445")
            {
                Name = "Pet Feeder",
                Description = "Automatic Pet Food Dispenser",
                Location = "Kitchen"
            };

            var gardenSprinkler = new GardenSprinkler("SPRINK-001", "SN-SPRINK-55667")
            {
                Name = "Garden Sprinkler",
                Description = "Smart Lawn Irrigation System",
                Location = "Backyard"
            };

            // Add all devices to control panel
            controlPanel.AddDevice(livingRoomLight);
            controlPanel.AddDevice(frontDoor);
            controlPanel.AddDevice(livingRoomThermostat);
            controlPanel.AddDevice(frontGate);
            controlPanel.AddDevice(securityCamera);
            controlPanel.AddDevice(alarmSystem);
            controlPanel.AddDevice(robotMop);
            controlPanel.AddDevice(smartTv);
            controlPanel.AddDevice(petFeeder);
            controlPanel.AddDevice(gardenSprinkler);

            // List all devices
            Console.WriteLine("\n=== Initial Device Status ===");
            controlPanel.ListAllDevices();

            // Control devices
            Console.WriteLine("\n=== Device Control ===");
            try
            {
                livingRoomLight.TurnOn();
                livingRoomThermostat.TurnOn();
                frontDoor.TurnOn();
                securityCamera.TurnOn();
                alarmSystem.TurnOn();
                robotMop.TurnOn();
                smartTv.TurnOn();
                petFeeder.TurnOn();
                gardenSprinkler.TurnOn();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Update device configurations
            Console.WriteLine("\n=== Device Configuration ===");
            try
            {
                livingRoomLight.UpdateConfiguration("Brightness", 75);
                livingRoomLight.UpdateConfiguration("Color", "Warm");

                livingRoomThermostat.UpdateConfiguration("Temperature", 20);
                livingRoomThermostat.UpdateConfiguration("Mode", "Auto");

                frontDoor.UpdateConfiguration("IsLocked", false);

                securityCamera.UpdateConfiguration("RecordingQuality", 3);
                securityCamera.PanTilt(30, 10);

                alarmSystem.AddSensor("MOTION-001");
                alarmSystem.AddSensor("DOOR-001");
                alarmSystem.UpdateConfiguration("AlarmMode", "Away");

                robotMop.UpdateConfiguration("CurrentMode", "Cleaning");
                robotMop.UpdateConfiguration("WaterTankLevel", 80);

                smartTv.UpdateConfiguration("Channel", 42);
                smartTv.UpdateConfiguration("Volume", 65);
                smartTv.PlayContent("Movie Time");

                petFeeder.UpdateConfiguration("PortionSize", 2);
                petFeeder.UpdateConfiguration("FeedingFrequency", 3);
                petFeeder.DispenseFood();

                gardenSprinkler.UpdateConfiguration("WaterFlowRate", 7);
                gardenSprinkler.UpdateConfiguration("DurationMinutes", 1);
                gardenSprinkler.StartWatering();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Group control by type
            Console.WriteLine("\n=== Group Control ===");
            controlPanel.GroupControlByType<SmartLight>(true); // Turn all lights on
            controlPanel.GroupControlByType<SmartThermostat>(false); // Turn all thermostats off

            // Update device details
            Console.WriteLine("\n=== Device Details Update ===");
            controlPanel.UpdateDeviceDetails("LIGHT-001", "Main Light", "Bright Smart Light", "Living Room");

            // Schedule device actions
            Console.WriteLine("\n=== Scheduled Actions ===");
            await controlPanel.ScheduleDeviceAction("LIGHT-001", d => d.TurnOff(), TimeSpan.FromSeconds(5));
            await controlPanel.ScheduleDeviceAction("ALS-001", d =>
            {
                var alarm = d as AlarmSystem;
                alarm?.TriggerAlarm();
                Task.Delay(2000).Wait();
                alarm?.SilenceAlarm();
            }, TimeSpan.FromSeconds(10));

            await controlPanel.ScheduleDeviceAction("SPRINK-001", d =>
            {
                var sprinkler = d as GardenSprinkler;
                sprinkler?.StopWatering();
                sprinkler?.TurnOff();
            }, TimeSpan.FromMinutes(1));

            // Remove a device
            Console.WriteLine("\n=== Device Removal ===");
            controlPanel.RemoveDevice("DOOR-001");

            // Final device list
            Console.WriteLine("\n=== Final Device Status ===");
            controlPanel.ListAllDevices();

            Console.WriteLine("\nSmart Home Control Panel Demonstration Complete.");
        }
    }
}