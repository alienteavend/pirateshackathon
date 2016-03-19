using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading;
namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "pirateshun.azure-devices.net";
        static string deviceKey = "i9Wubtsb1ac3TN9AQV9HSN0kT0OSB8BEDn/tO/9ruw8=";
        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("mockDevice", deviceKey));

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
        private static async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            double avgTemp = 25;
            double avgPressure = 101325;
            double avgHumidity = 50;
            double bird = 0.1;
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;
                double currentTemp = avgTemp + rand.NextDouble() * 4 - 2;
                double currentPressure = avgPressure + rand.NextDouble() * 4 - 2;
                double currentHumidity = avgHumidity + rand.NextDouble() * 4 - 2;
                double currentBird = rand.NextDouble() * 100 < 10?1:0;

                var telemetryDataPoint = new
                {
                    deviceId = "mockDevice",
                    windSpeed = currentWindSpeed,
                    temp = currentTemp,
                    pressure = currentPressure,
                    humidity = currentHumidity,
                    bird = currentBird
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties["messageType"] = "interactive";
                message.MessageId = Guid.NewGuid().ToString();

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Thread.Sleep(10000);

            }
        }
    }
}
