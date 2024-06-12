using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class PlaceOfResidence
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int HouseNumber { get; set; }
    public int? ApartmentNumber { get; set; }
        
    [ForeignKey("PersonInformation")]
    public Guid PersonInformationId { get; set; }
    public PersonInformation? PersonInformation { get; set; }
}