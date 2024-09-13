using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Management;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Terminating 'securden' process...");
        terminateProcess("Securden");
        stopService("Securden");
        UninstallProgram("Securden");
        CleadFileResidue();
        Console.WriteLine("Operation complete.");
        Console.ReadLine();
    }

    static void stopService(string serviceName)
    {

        try
        {
            ServiceController[] services = ServiceController.GetServices();
            var filteredServices = services.Where(s => s.ServiceName.StartsWith(serviceName, StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("Deleting the servicec...");

            if (filteredServices.Count() > 0)
            {
                foreach (var service in filteredServices)
                {
                    Console.WriteLine($"deleting service ----------> {service.ServiceName}");
                    Process process = new Process();
                    process.StartInfo.FileName = "sc.exe";
                    process.StartInfo.Arguments = $"delete {service.ServiceName}";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        Console.WriteLine($"Service '{service.ServiceName}' successfully deleted.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to delete service '{service.ServiceName}'. Exit code: {process.ExitCode}");
                    }
                }
            }
            else {
                Console.WriteLine("No Service where founds");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void terminateProcess(string processName)
    {
        try
        {
            Process[] processes = Process.GetProcesses();
            if (processes.Length > 0) { 
                foreach (Process process in processes)
                {
                    if (process.ProcessName.StartsWith(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Terminating process {process.ProcessName} with ID {process.Id}");

                        if (!process.CloseMainWindow())
                        {
                            process.Kill();
                        }
                        process.WaitForExit();
                        Console.WriteLine($"Process {process.ProcessName} with ID {process.Id} terminated.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"No processes found with the name: {processName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    static void UninstallProgram(string applicationName)
    {
        try
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            foreach (ManagementObject obj in searcher.Get())
            {
                string displayName = obj["Name"] as string;
                if (displayName != null && displayName.IndexOf(applicationName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine($"Found application: {displayName}");
                    var uninstallResult = obj.InvokeMethod("Uninstall", null);

                    if ((uint)uninstallResult == 0) // 0 means success
                    {
                        Console.WriteLine($"Application '{applicationName}' uninstalled successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to uninstall application '{applicationName}'. Result code: {uninstallResult}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    static void CleadFileResidue() { 
        string fileNameString = "C:\\Program Files\\Securden";
        if (File.Exists(fileNameString))
        {
            File.Delete(fileNameString);
        }
        else {
            Console.WriteLine("File not found!!!!!!!!!");
         }
    }
}
