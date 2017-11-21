using System;
using System.Collections.Generic;
using SantaConsole.Models;

namespace SantaConsole.Services
{
  public interface IAssignmentService
  {
    List<Assignment> AssignPeople(List<Person> source);
  }
}
