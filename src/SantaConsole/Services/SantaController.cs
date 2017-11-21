using SantaConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantaConsole.Services
{
  /// <summary>
  /// Essentially the classes that takes all the services
  /// and manages the state between them all.
  /// </summary>
  public class SantaController
  {
    private readonly ILoadingService _loadSvc;
    private readonly IAssignmentService _assignSvc;
    private readonly IStorageService _storageSvc;
    private readonly IMessagingService _msgSvc;
    private readonly IStatusService _status;

    /// <summary>Constructor</summary>
    public SantaController(
      ILoadingService loadSvc,
      IAssignmentService assignSvc,
      IStorageService storageSvc,
      IMessagingService msgSvc,
      IStatusService status)
    {
      _loadSvc = loadSvc;
      _assignSvc = assignSvc;
      _storageSvc = storageSvc;
      _msgSvc = msgSvc;
      _status = status;
    }

    /// <summary>Loads the people, assigns them, and notifies them.</summary>
    public int Assign()
    {
      try
      {
        int exit = 0;

        // loading the participants
        _status.Info("Loading people..");
        var people = _loadSvc.GetPeople();
        exit = this.ValidatePeople(people);
        if (exit != 0) return exit;

        // assign the people
        _status.Info($"Assigning {people.Count:N0} people..");
        List<Assignment> result = null;
        for (int i = 0; i < _loadSvc.MaxIterations; i++)
        {
          result = _assignSvc.AssignPeople(people);
          if (result != null) break;
          _status.Debug("retrying assignment");
        }
        if (result == null) return _status.Fail(508, "Unable to assign people.");

        // save the results..
        _status.Info("Saving results to file..");
        _storageSvc.SaveResults(result);

        // send messages
        _status.Info("Sending Messages to everyone..");
        _msgSvc.SendMessages(result);

        return 0;
      }
      catch (Exception ex)
      {
        return _status.Fail(500, ex.Message);
      }
    }

    /// <summary>Validates the list of people loaded.</summary>
    private int ValidatePeople(List<Person> people)
    {
      if (people.Count <= 3) return _status.Fail(400, $"Invalid number of people specified {people.Count}. Atleast 4 people are required.");
      
      return 0;
    }
  }
}
