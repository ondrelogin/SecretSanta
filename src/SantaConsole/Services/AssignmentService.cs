using System;
using System.Collections.Generic;
using SantaConsole.Models;

namespace SantaConsole.Services
{
  /// <summary>
  /// Simple but flexible logic for assigning gift exchange.
  /// 
  /// This class differs from most as this is designed to accomodate a
  /// gift exchange for large extended families. The primary intent was
  /// so the children of the families could participate in a gift exchange
  /// but not have to give gifts to their direct siblings.
  /// </summary>
  public class AssignmentService : IAssignmentService
  {
    private readonly Random _rnd;
    
    /// <summary>Constructor</summary>
    public AssignmentService()
    {
      _rnd = new Random();
    }

    /// <summary>
    /// Assigns the source of list of people returning the list of assignements,
    /// or a value of null (if was unable to assign). The simplicity of the algorithm
    /// recommends that in most scenarios, if you simply retry, you will eventually
    /// come up with a valid list of assignments.
    /// </summary>
    public List<Assignment> AssignPeople(List<Person> source)
    {
      var assignedList = new List<Assignment>();

      var available = new List<Person>(source);
      foreach (var giver in source)
      {
        if (!this.AssignPerson(giver, available, assignedList)) return null;
      }

      return assignedList;
    }

    /// <summary>
    /// This takes in a person who needs a gift to be
    /// </summary>
    /// <remarks>
    /// This may not be the best algorithm for this, but it is simple enough and
    /// can easily be modified.
    /// </remarks>
    private bool AssignPerson(Person giver, List<Person> available, List<Assignment> assignedList)
    {
      if (giver == null) throw new ArgumentNullException("p");
      if (available == null) throw new ArgumentNullException("available");
      if (available.Count == 0) throw new ArgumentException("available");

      // filter available to those people who are available and eligable
      var eligable = this.FilterAvailableToEligable(giver, available);
      if (eligable == null || eligable.Count == 0) return false;

      // rng
      int index = _rnd.Next(eligable.Count);
      var receiver = eligable[index];
      assignedList.Add(new Assignment(giver, receiver.Name)); // assigned giver to name
      available.Remove(receiver); // remove from the available list.
      return true;
    }

    /// <summary>
    /// Reduces the list of available (i.e. people who don't have a giver) to an
    /// even smaller list, containing only people eligable to be the givers target/receiver.
    /// (i.e. can't be the same person and can't be the same FamilyGroupIdentifier).
    /// </summary>
    private List<Person> FilterAvailableToEligable(Person giver, List<Person> available)
    {
      var list = new List<Person>();

      foreach (var target in available)
      {
        if (this.CanAssignToPerson(giver, target)) { list.Add(target); }
      }
      return list;
    }

    /// <summary>
    /// returns true if the giver can give a gift to the receiver.
    /// </summary>
    private bool CanAssignToPerson(Person giver, Person receiver)
    {
      if (giver.FamilyGroupIdentifier == null) return true;
      if (receiver.FamilyGroupIdentifier == null) return true;
      // can't have self
      if (giver.Name.Equals(receiver.Name, StringComparison.InvariantCultureIgnoreCase)) return false;

      // if giver and receiver are in the same FamilyGroupIdentifier then they can't have each other
      if (giver.FamilyGroupIdentifier.Equals(receiver.FamilyGroupIdentifier, StringComparison.InvariantCultureIgnoreCase)) return false;

      return true;
    }
  }
}
