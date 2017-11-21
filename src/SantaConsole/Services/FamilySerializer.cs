using System;
using System.Collections.Generic;
using System.Xml;
using SantaConsole.Models;

namespace SantaConsole.Services
{
  public class FamilySerializer : ILoadingService, IStorageService
  {
    public int MaxIterations { get; set; }
    public string GiftExchangeName { get; set; }

    public FamilySerializer()
    {
      this.MaxIterations = 25;
      this.GiftExchangeName = $"Secret Santa {DateTime.Today.Year}";
    }

    /// <summary>
    /// Returns the list of people that will take part in the gift exchange.
    /// </summary>
    public List<Person> GetPeople()
    {
      var list = new List<Person>();
      // TODO load the people
      // this could be a hardcoded list, or loaded from some text file.
      return list;
    }

    /// <summary>
    /// Saves the assignments to an base64 encoded xml document.
    /// </summary>
    /// <remarks>
    /// The goal of not just saving the xml document as a raw document
    /// is that it becomes too easy to open up the document by someone
    /// technical and see who everyone is assigned to. By adding a simple
    /// base64 encoding, it becomes really easy to decrypt if needed
    /// to see who might be assigned to who.
    /// </remarks>
    public void SaveResults(List<Assignment> assignments)
    {
      string fileName = this.GenerateFileName();

      var doc = new XmlDocument();
      var ele = doc.AppendChild(doc.CreateElement("root")) as XmlElement;

      foreach (var assign in assignments)
      {
        var personEle = ele.AppendChild(doc.CreateElement("person")) as XmlElement;
        personEle.SetAttribute("name", assign.Giver.Name);
        personEle.SetAttribute("address", assign.Giver.NotificationAddress);
        if (!string.IsNullOrWhiteSpace(assign.Giver.FamilyGroupIdentifier))
        {
          personEle.SetAttribute("group", assign.Giver.FamilyGroupIdentifier);
        }
        personEle.SetAttribute("assignedTo", assign.Receiver);
      }

      string xml = doc.OuterXml;
      byte[] data = System.Text.Encoding.Default.GetBytes(xml);
      string hexData = System.Convert.ToBase64String(data);

      System.IO.File.WriteAllText(fileName, hexData);
    }

    /// <summary>
    /// Generates the full filename and path of the assignments
    /// </summary>
    private string GenerateFileName()
    {
      var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      var name = this.GiftExchangeName + ".txt";
      return System.IO.Path.Combine(path, name);
    }
  }
}
