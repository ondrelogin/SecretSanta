using System;
using System.Collections.Generic;
using SantaConsole.Models;

namespace SantaConsole.Services
{
  /// <summary>
  /// This service is setup to use Twilio to send text messages. This requires an account
  /// to be set up, with the appropriate Account SID, Auth Token and From Number.
  /// </summary>
  public class TwilioService : IMessagingService
  {
    private readonly IStatusService _status;
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;

    public string GiftExchangeName { get; set; }

    /// <summary>Constructor</summary>
    public TwilioService(IStatusService status)
    {
      _status = status;
      _accountSid = Environment.GetEnvironmentVariable("TW_ASID", EnvironmentVariableTarget.User);
      _authToken = Environment.GetEnvironmentVariable("TW_AT", EnvironmentVariableTarget.User);
      _fromNumber = Environment.GetEnvironmentVariable("TW_FP", EnvironmentVariableTarget.User);

      if (string.IsNullOrWhiteSpace(_accountSid)) throw new BadEnvironmentForTwilioException();
      if (string.IsNullOrWhiteSpace(_authToken)) throw new BadEnvironmentForTwilioException();
      if (string.IsNullOrWhiteSpace(_fromNumber)) throw new BadEnvironmentForTwilioException();

      this.GiftExchangeName = $"Secret Santa {DateTime.Today.Year}";

      // New accounts and subaccounts are now required to use TLS 1.2 when accessing the REST API.
      //   If the error thrown is "Upgrade Required" what it really means, is you need your ServicePointManager to use TLS 1.2
      //   the line below does that.
      System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
    }

    /// <summary>
    /// This will use the twilio settings to send a test text message.
    /// </summary>
    public void Test(string toPhoneNumber, string message = "Hello World!")
    {
      _status.Debug("init");
      Twilio.TwilioClient.Init(_accountSid, _authToken);

      var to = new Twilio.Types.PhoneNumber(toPhoneNumber);
      var fromNum = new Twilio.Types.PhoneNumber(_fromNumber);
      _status.Debug("sending");
      Twilio.Rest.Api.V2010.Account.MessageResource.Create(to,
        from: fromNum,
        body: message
        );
      _status.Debug("done");
    }

    public void SendMessages(List<Assignment> assignments)
    {
      _status.Debug("Building text messages...");
      var dict = this.BuildMessagesToSend(assignments);

      _status.Debug("Initializing Twilio...");
      Twilio.TwilioClient.Init(_accountSid, _authToken);
      var fromNum = new Twilio.Types.PhoneNumber(_fromNumber);
      foreach (var kvp in dict)
      {
        var to = new Twilio.Types.PhoneNumber(kvp.Key);
        _status.Debug($"Sending message to {to}...");
        Twilio.Rest.Api.V2010.Account.MessageResource.Create(to,
          from: fromNum,
          body: kvp.Value
        );
      }
    }

    /// <summary>
    /// In case some people have the same NotificationAddress, the assumption is
    /// that, the actual address is most likely a parent. To reduce the number of messages
    /// sent, this combines all assignments grouping by NotificationAddress.
    /// </summary>
    private Dictionary<string, string> BuildMessagesToSend(List<Assignment> assignments)
    {
      var dict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

      foreach (var assign in assignments)
      {
        if (assign.Giver == null) throw new Exception($"Assignment.Giver is null!");
        if (string.IsNullOrWhiteSpace(assign.Giver.NotificationAddress)) throw new Exception($"Assignment.Giver.NotificationAddress is required for {assign.Giver.Name}.");
        if (string.IsNullOrWhiteSpace(assign.Giver.Name)) throw new Exception("Assignment.Giver.Name is required!");
        if (string.IsNullOrWhiteSpace(assign.Receiver)) throw new Exception("Assignment.Receiver is required!");

        string message = $"{assign.Giver.Name} is assigned to {assign.Receiver}.";
        var address = assign.Giver.NotificationAddress;
        if (!dict.ContainsKey(address))
        {
          string newMessage = $"{this.GiftExchangeName}\r\n{message}";
          dict.Add(address, newMessage);
        }
        else
        {
          dict[address] += "\r\n" + message;
        }
      }
      
      return dict;
    }

    public class BadEnvironmentForTwilioException : Exception
    {
      public BadEnvironmentForTwilioException() : base("To use this server, User Environmental Variables need to be set for TW_ASID, TW_AT and TW_FP.") { }
    }
  }
}
