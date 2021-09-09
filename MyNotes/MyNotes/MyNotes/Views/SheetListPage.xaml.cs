using DevExpress.XamarinForms.CollectionView;
using MyNotes.Models;
using MyNotes.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SheetListPage : BasePage
    {
        public SheetListPage()
        {
            InitializeComponent();
        }

        bool isAnimated;
        async void OnStatusChanged(object sender, SwipeItemTapEventArgs e)
        {
            if (isAnimated)
                return;

            if (BindingContext == null || ViewModel == null)
                return;

            IList<Sheet> source = ((SheetListViewModel)ViewModel).Sheets;

            Sheet sheet = e.Item as Sheet;
            int newItemHandle = 0;

            switch (sheet.Status)
            {
                case SheetStatus.Pinned:
                    newItemHandle = source.Where(t => t.Status == SheetStatus.Pinned).Count() - 1;
                    break;
                case SheetStatus.Default:
                    newItemHandle = source.Count() - 1;
                    break;
            }

            int itemHandle = e.ItemHandle;
            if (itemHandle == newItemHandle)
            {
                await ((SheetListViewModel)ViewModel).ExecuteRefreshCommand();
                return;
            }

            isAnimated = true;
            Device.BeginInvokeOnMainThread(() =>
                sheetCollectionView.MoveItem(itemHandle, newItemHandle, async () =>
                {
                    isAnimated = false;
                    // need to get rid of this after the bug was fixed
                    await ((SheetListViewModel)ViewModel).ExecuteRefreshCommand();
                })
            );
        }

        public async Task MoveItem(int oldIndex, int newIndex)
        {
            if (isAnimated)
                return;

            if (BindingContext == null || ViewModel == null)
                return;

            if (oldIndex == newIndex)
            {
                await ((SheetListViewModel)ViewModel).ExecuteRefreshCommand();
                return;
            }

            isAnimated = true;

            Device.BeginInvokeOnMainThread(() =>
                sheetCollectionView.MoveItem(oldIndex, newIndex, async () =>
                {
                    isAnimated = false;
                    // need to get rid of this after the bug was fixed
                    await ((SheetListViewModel)ViewModel).ExecuteRefreshCommand();
                })
            );
        }
    }
}