using MyNotes.Models;
using Xamarin.Forms;

namespace MyNotes
{
    public class ItemDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is Item currentItem))
                return null;

            return currentItem.Status switch
            {
                ItemStatus.Pinned => UrgentDataTemplate,
                ItemStatus.Completed => CompletedDataTemplate,
                _ => UncompletedDataTemplate,
            };
        }

        public DataTemplate UrgentDataTemplate { get; set; }
        public DataTemplate CompletedDataTemplate { get; set; }
        public DataTemplate UncompletedDataTemplate { get; set; }
    }
}
