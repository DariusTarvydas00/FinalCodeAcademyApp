using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace DataAccess.Entities;

public class PersonInformation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public string Birthday { get; set; }
    public string PersonalCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    
    [ForeignKey("UserId")]
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public PlaceOfResidence PlaceOfResidence { get; set; }
    public ProfilePhoto? ProfilePhoto { get; set; }
}