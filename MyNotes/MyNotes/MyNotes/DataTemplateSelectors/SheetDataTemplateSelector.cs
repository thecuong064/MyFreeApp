using MyNotes.Models;
using Xamarin.Forms;

namespace MyNotes
{
    public class SheetDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is Sheet currentItem))
                return null;

            return currentItem.Status switch
            {
                SheetStatus.Pinned => PinnedDataTemplate,
                _ => DefaultDataTemplate,
            };
        }

        public DataTemplate PinnedDataTemplate { get; set; }
        public DataTemplate DefaultDataTemplate { get; set; }
    }
}
