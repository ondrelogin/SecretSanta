using System;

namespace SantaConsole.Models
{
  /// <summary>
  /// Represents a single person who will participate in the gift exchange.
  /// </summary>
  public class Person
  {
    /// <summary>Get/sets the name of the person. Must be Unique.</summary>
    public string Name { get; set; }
    /// <summary>Get/sets the address that should receive the assignments.</summary>
    public string NotificationAddress { get; set; }
    /// <summary>
    /// Gets/sets the group that Person belongs to. Using the AssignmentService
    /// this helps group a group of people together, preventing them from having each other.
    /// All siblings or direct family members typically belong to a single group.
    /// </summary>
    public string FamilyGroupIdentifier { get; set; }
  }
}
