using System;
using SantaConsole.Services;

namespace SantaConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      // ioc-like stuff
      // TODO setup familySerializer so that it can maybe read command line args, with arg[0] maybe being some sort of file that contains the family data.
      var familySerializer = new FamilySerializer();
      var assignSvc = new AssignmentService();
      var consoleSvc = new ConsoleStatusService();
      IStorageService storageSvc = familySerializer;
      IMessagingService messageSvc = consoleSvc;

      // try and use twilio
      try
      {
        var tw = new TwilioService(consoleSvc);
        messageSvc = tw;
      }
      catch (TwilioService.BadEnvironmentForTwilioException ex)
      {
        consoleSvc.Info("Unable to configure Twilio, using Debug Console Service");
        consoleSvc.Debug(ex.Message);
      }
      catch (Exception)
      {
        throw;
      }
      
      //  ho ho ho, santa assigns the gift exchange
      var santa = new SantaController(familySerializer, assignSvc, storageSvc, messageSvc, consoleSvc);
      int exitCode = santa.Assign();
      // assumption if exit code is 0, it should already be outputted to the screen.

      // wait for any key to exit
      Console.Write("Press any key to continue...");
      var anyKey = Console.ReadKey();
      Console.WriteLine();
      
      // exit
      Environment.Exit(exitCode);
    }
  }
}
