using System.Collections.Generic;
using SantaConsole.Models;

namespace SantaConsole.Services
{
  /// <summary>
  /// Interface for loading the list of People that will
  /// participate in the gift exchange. This can be hardcoded
  /// or loaded from a file.
  /// </summary>
  public interface ILoadingService
  {
    int MaxIterations { get;  }

    /// <summary>
    /// Returns a list of the people participating in the Christmas Gift Exchange.
    /// </summary>
    List<Person> GetPeople();
  }
}
