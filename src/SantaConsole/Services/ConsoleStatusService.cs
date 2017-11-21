using System;
using System.Collections.Generic;
using SantaConsole.Models;

namespace SantaConsole.Services
{
  /// <summary>
  /// This is used for displaying status to the Console app. During Development or troubelshooting,
  /// this can be used as replacements for storage and messaging, i.e. sending the information to
  /// the console, rather than a real output source.
  /// </summary>
  public class ConsoleStatusService : IStatusService, IStorageService, IMessagingService
  {
    /// <summary>Displays an error message to the screen</summary>
    /// <remarks>The color changing is not thread safe, but this program isn't leveraging multiple threads.</remarks>
    public int Fail(int errorCode, string errorMessage)
    {
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine(" ERR: {0}", errorMessage);
      Console.ResetColor();
      return errorCode;
    }

    /// <summary>Displays a debug message to the screen.</summary>
    /// <remarks>
    /// The color changing is not thread safe, but this program isn't leveraging multiple threads.
    /// Maybe have some switch so can not output debug messages.
    /// </remarks>
    public void Debug(string message)
    {
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine("DBUG: {0}", message);
      Console.ResetColor();
    }

    /// <summary>Displays information to the console.</summary>
    public void Info(string message)
    {
      Console.WriteLine("INFO: {0}", message);
    }

    /// <summary>Here for testing/debugging</summary>
    void IStorageService.SaveResults(List<Assignment> assignments)
    {
      // do nothing
    }

    /// <summary>Here for testing/debugging</summary>
    void IMessagingService.SendMessages(List<Assignment> assignments)
    {
      foreach (var assign in assignments)
      {
        this.Debug($"{assign.Giver.Name} is assigned to {assign.Receiver}.");
      }
    }
  }
}
