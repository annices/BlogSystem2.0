using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogSystem.Models
{
    /// <summary>
    /// This model class reflects the DB table "BS_Entries". Changes made in this class
    /// must reflect the corresponding DB table and BlogSystemContext class!
    /// </summary>
    public partial class BsEntry
    {
        public BsEntry()
        {
            BsComments = new HashSet<BsComment>();
            BsEntryCategories = new HashSet<BsEntryCategory>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        [StringLength(50, ErrorMessage = "The title max-length is 50 characters.")]
        [RegularExpression(@"[^<>]*$", ErrorMessage = "HTML is not allowed in the title.")]
        public string Title { get; set; }

        public DateTime Date { get; set; }

        public bool IsPublished { get; set; }

        [Required(ErrorMessage = "Please write an entry.")]
        [StringLength(8000, ErrorMessage = "The entry max-length is 8000 characters.")]
        public string Entry { get; set; }

        public int UserId { get; set; }
        public virtual BsUser User { get; set; }

        public virtual ICollection<BsComment> BsComments { get; set; }
        public virtual ICollection<BsEntryCategory> BsEntryCategories { get; set; }
    }
}
