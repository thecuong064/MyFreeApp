using MvvmHelpers;
using SQLite;
using System;

namespace MyNotes.Models
{
    public enum SheetStatus
    {
        Default = 0,
        Pinned = 1
    }

    public class Sheet : ObservableObject
    {

        [PrimaryKey]
        public string Id { get; set; }
        public int Ordering { get; set; }
        public string Name { get; set; }
        public int ItemCount { get; set; }
        public int CompletedItemCount { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        private SheetStatus status;
        public SheetStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
    }
}
