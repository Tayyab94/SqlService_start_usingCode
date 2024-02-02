// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using StartSqlServiceFromCode;
using System.Diagnostics;
using System.ServiceProcess;


try
{
    ConnectToSqlServer();
}
catch (Exception ex) when (IsNetworkError(ex))
{
    Console.WriteLine("Network-related error. Attempting to start SQL Server service.");

    // Attempt to start the SQL Server service
    ServiceController service = new ServiceController("MSSQLSERVER");
    if (service.Status != ServiceControllerStatus.Running)
    {
        service.Start();
        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
        Console.WriteLine("SQL Server service started successfully.");
    }
    else
    {
        Console.WriteLine("SQL Server service is already running.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
    // Attempt to start SQL Server service
    StartSqlServerService();
}


static void StartSqlServerService()
{
    RunAsAdministrator();
    ServiceController service = new ServiceController("MSSQLSERVER");
    if (service.Status != ServiceControllerStatus.Running)
    {
        service.Start();
        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
        Console.WriteLine("SQL Server service started successfully.");
    }
    else
    {
        Console.WriteLine("SQL Server service is already running.");
    }

    // Attempt to connect to SQL Server again after starting the service
    ConnectToSqlServer();
}

static void ConnectToSqlServer()
{
    string connectionString = "Putt Connection String here";

    using (var context = new YourDbContext(connectionString))
    {
        //context.Database.Migrate();
        context.Database.EnsureCreated();
        // Attempt to check database connectivity
        if (context.Database.CanConnect())
        {
            Console.WriteLine("SQL Server is running.");
        }
        else
        {
            Console.WriteLine("Error: Unable to connect to the database.");
        }
    }

}


static void RunAsAdministrator()
{
    // Start a new process with elevated privileges using 'runas'
    ProcessStartInfo psi = new ProcessStartInfo();
    psi.FileName = Process.GetCurrentProcess().MainModule.FileName;
    psi.Verb = "runas"; // Run as administrator
    psi.UseShellExecute = true;

    try
    {
        Process.Start(psi);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
static bool IsNetworkError(Exception ex)
{
    // Check if the exception is a network-related error
    return ex.InnerException is System.Net.Sockets.SocketException ||
           ex.InnerException is System.IO.IOException;
}