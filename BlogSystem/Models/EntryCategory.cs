
namespace BlogSystem.Models
{
    /// <summary>
    /// This entity class is called in the HomeController to populate a dropdown category list in the view layer.
    /// </summary>
    public class EntryCategory
    {
        public int EntryId { get; set; }
        public string CategoryName { get; set; }
    }
}
