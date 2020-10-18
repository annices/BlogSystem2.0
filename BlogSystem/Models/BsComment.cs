using System;
using System.ComponentModel.DataAnnotations;

namespace BlogSystem.Models
{
    /// <summary>
    /// This model class reflects the DB table "BS_Comments". Changes made in this class
    /// must reflect the corresponding DB table and BlogSystemContext class!
    /// </summary>
    public partial class BsComment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter an author name.")]
        [StringLength(30, ErrorMessage = "The name max-length is 30 characters.")]
        [RegularExpression(@"[^<>]*$", ErrorMessage = "HTML is not allowed in the name.")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "The email max-length is 50 characters.")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        public DateTime? Date { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(100, ErrorMessage = "The URL max-length is 100 characters.")]
        public string Website { get; set; }

        [Required(ErrorMessage = "Please write a comment.")]
        [StringLength(300, ErrorMessage = "The comment max-length is 300 characters.")]
        [RegularExpression(@"[^<>]*$", ErrorMessage = "HTML is not allowed in the comment.")]
        public string Comment { get; set; }

        [StringLength(300, ErrorMessage = "The reply max-length is 300 characters.")]
        public string Reply { get; set; }

        public int EntryId { get; set; }
        public virtual BsEntry Entry { get; set; }
    }
}
