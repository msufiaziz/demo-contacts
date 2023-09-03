using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.UI.Shared
{
    public class UpdateContactRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [Phone]
        public string Phone { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string SkillSets { get; set; } = null!;
        [Required]
        public string Hobby { get; set; } = null!;
    }
}
