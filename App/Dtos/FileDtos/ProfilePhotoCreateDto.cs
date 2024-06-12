using System.ComponentModel.DataAnnotations;

namespace App.Dtos.FileDtos;

public class ProfilePhotoCreateDto
{
    [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Only picture files are allowed (JPEG, PNG, GIF).")]
    public string FileName { get; set; }
    public string FilePath { get; set; }
    
    [Required]
    public Guid PersonInformationId { get; set; }
}