using MvvmHelpers;
using SQLite;

namespace MyNotes.Models
{
    public enum ItemStatus
    {
        Todo = 0,
        Completed = 1,
        Pinned = 2,
    }

    public class Item : ObservableObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public int Ordering { get; set; }
        public string SheetId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public bool IsPinned { get; set; }

        private ItemStatus status;
        public ItemStatus Status 
        { 
            get => status; 
            set => SetProperty(ref status, value); 
        }

        public Item()
        {
        }
    }
}