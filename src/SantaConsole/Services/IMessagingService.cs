using System;
using System.Collections.Generic;
using SantaConsole.Models;

namespace SantaConsole.Services
{
  public interface IMessagingService
  {
    void SendMessages(List<Assignment> assignments);
  }
}
