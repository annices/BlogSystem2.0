using System;
using System.Collections.Generic;

namespace BlogSystem.Models
{
    /// <summary>
    /// This model class reflects the DB table "BS_EntryCategories". Changes made in this class
    /// must reflect the corresponding DB table and BlogSystemContext class!
    /// </summary>
    public partial class BsEntryCategory
    {
        public int Id { get; set; }
        public int? EntryId { get; set; }
        public int? CategoryId { get; set; }

        public virtual BsCategory Category { get; set; }

        public virtual BsEntry Entry { get; set; }
    }
}
