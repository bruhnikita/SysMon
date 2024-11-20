using LibreHardwareMonitor.Hardware;
using System.Linq;

namespace SysMon
{
    public class SystemMonitor
    {
        private readonly Computer _computer;

        public SystemMonitor()
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsMemoryEnabled = true,
                IsStorageEnabled = true
            };
            _computer.Open();
        }

        public float GetCpuUsage()
        {
            var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            cpu?.Update();

            var sensor = cpu?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Total"));
            return sensor?.Value ?? 0;
        }

        public float GetMemoryUsage()
        {
            var memory = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);
            memory?.Update();

            var sensor = memory?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
            return sensor?.Value ?? 0;
        }

        public float GetDiskUsage()
        {
            var storage = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Storage);
            storage?.Update();

            var sensor = storage?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
            return sensor?.Value ?? 0;
        }

        public bool IsServerAvailable(string server)
        {
            try
            {
                using var ping = new System.Net.NetworkInformation.Ping();
                var reply = ping.Send(server, 1000);
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _computer.Close();
        }
    }

}
