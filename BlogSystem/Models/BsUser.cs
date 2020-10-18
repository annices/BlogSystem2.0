using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogSystem.Models
{
    /// <summary>
    /// This model class reflects the DB table "BS_Users". Changes made in this class
    /// must reflect the corresponding DB table and BlogSystemContext class!
    /// </summary>
    public partial class BsUser
    {
        public BsUser()
        {
            BsEntries = new HashSet<BsEntry>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a username.")]
        [StringLength(30, ErrorMessage = "The username max-length is 30 characters.")]
        [RegularExpression(@"[^<>]*$", ErrorMessage = "HTML is not allowed in the username.")]
        public string Username { get; set; }

        [StringLength(30, ErrorMessage = "The first name max-length is 30 characters.")]
        [RegularExpression(@"[^<>]*$", ErrorMessage = "HTML is not allowed in the first name.")]
        public string Firstname { get; set; }

        [StringLength(30, ErrorMessage = "The last name max-length is 30 characters.")]
        [RegularExpression(@"[^<>]*$", ErrorMessage = "HTML is not allowed in the last name.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Please enter an email.")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            ErrorMessage = "Please enter a valid email address.")]
        [StringLength(50, ErrorMessage = "The email max-length is 50 characters.")]
        public string Email { get; set; }

        public string Password { get; set; }

        public virtual ICollection<BsEntry> BsEntries { get; set; }
    }
}
