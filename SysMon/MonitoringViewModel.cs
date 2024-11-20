using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LibreHardwareMonitor.Hardware;

namespace SysMon
{
    public class MonitoringViewModel : INotifyPropertyChanged
    {
        private readonly Computer _computer;
        private float _cpuUsage;
        private float _memoryUsage;
        private float _diskUsage;
        private string _networkStatus;

        public event PropertyChangedEventHandler PropertyChanged;

        public MonitoringViewModel()
        {
            _computer = new Computer { IsCpuEnabled = true, IsMemoryEnabled = true, IsStorageEnabled = true };
            _computer.Open();

            RefreshDataCommand = new Command(RefreshData);
            RefreshData();
        }

        public float CpuUsage
        {
            get => _cpuUsage;
            set => SetProperty(ref _cpuUsage, value);
        }

        public float MemoryUsage
        {
            get => _memoryUsage;
            set => SetProperty(ref _memoryUsage, value);
        }

        public float DiskUsage
        {
            get => _diskUsage;
            set => SetProperty(ref _diskUsage, value);
        }

        public string NetworkStatus
        {
            get => _networkStatus;
            set => SetProperty(ref _networkStatus, value);
        }

        public ICommand RefreshDataCommand { get; }

        private void RefreshData()
        {
            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();
                foreach (var sensor in hardware.Sensors)
                {
                    switch (sensor.SensorType)
                    {
                        case SensorType.Load when hardware.HardwareType == HardwareType.Cpu:
                            CpuUsage = sensor.Value.GetValueOrDefault();
                            break;
                        case SensorType.Data when hardware.HardwareType == HardwareType.Memory:
                            MemoryUsage = sensor.Value.GetValueOrDefault();
                            break;
                        case SensorType.Load when hardware.HardwareType == HardwareType.Storage:
                            DiskUsage = sensor.Value.GetValueOrDefault();
                            break;
                    }
                }
            }

            NetworkStatus = CheckNetworkStatus("https://www.google.com") ? "Available" : "Unavailable";
        }

        private bool CheckNetworkStatus(string url)
        {
            try
            {
                using var client = new HttpClient();
                var response = client.GetAsync(url).Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
