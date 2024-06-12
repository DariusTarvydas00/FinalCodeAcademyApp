using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class ProfilePhoto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string FilePath { get; set; }
    
    [ForeignKey("PersonInformation")]
    public Guid? PersonInformationId { get; set; }
    public PersonInformation? PersonInformation { get; set; }
    
    [ForeignKey("User")]
    public Guid? UserId { get; set; }
    public User? User { get; set; }
}