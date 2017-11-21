using System;

namespace SantaConsole.Models
{
  /// <summary>
  /// Simple POCO for storing the assignment of the gift giver to the gift receiver.
  /// </summary>
  public class Assignment
  {
    /// <summary>Gets the Person who is going to give the gift.</summary>
    public Person Giver { get; private set; }
    /// <summary>Gets the Person's name who will receive the gift.</summary>
    public string Receiver { get; private set; }

    public Assignment(Person giver, string receiver)
    {
      this.Giver = giver;
      this.Receiver = receiver;
    }
  }
}
