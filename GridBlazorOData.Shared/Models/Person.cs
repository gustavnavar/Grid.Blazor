using System;
using System.ComponentModel.DataAnnotations;

namespace GridBlazorOData.Shared.Models
{
    [Serializable]
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } set { } }
    }
}
