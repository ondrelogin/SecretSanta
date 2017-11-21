using System;

namespace SantaConsole.Services
{
  public interface IStatusService
  {
    void Info(string message);
    void Debug(string message);
    int Fail(int errorCode, string errorMessage);
  }
}
