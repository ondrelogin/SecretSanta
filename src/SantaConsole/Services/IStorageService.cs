using System;
using System.Collections.Generic;
using SantaConsole.Models;

namespace SantaConsole.Services
{
  public interface IStorageService
  {
    void SaveResults(List<Assignment> assignments);
  }
}
