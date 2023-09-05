using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.UI.Shared
{
    public class CreateContactRequest
    {
        [Required]
        public string UserName { get; set; } = "username";
        [Required]
        [Phone]
        public string Phone { get; set; } = "01234567890";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "user@example.com";
        [Required]
        public string SkillSets { get; set; } = "skill1, skill2, skill3";
        [Required]
        public string Hobby { get; set; } = "Hobby";
    }
}
