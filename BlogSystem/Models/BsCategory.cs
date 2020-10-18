using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogSystem.Models
{
    /// <summary>
    /// This model class reflects the DB table "BS_Categories". Changes made in this class
    /// must reflect the corresponding DB table and BlogSystemContext class!
    /// </summary>
    public partial class BsCategory
    {
        public BsCategory()
        {
            BsEntryCategories = new HashSet<BsEntryCategory>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a category name.")]
        [StringLength(50, ErrorMessage = "The category max-length is 50 characters.")]
        [RegularExpression(@"[^<>]*$", ErrorMessage = "HTML is not allowed in the category name.")]
        public string Category { get; set; }

        public virtual ICollection<BsEntryCategory> BsEntryCategories { get; set; }
    }
}
