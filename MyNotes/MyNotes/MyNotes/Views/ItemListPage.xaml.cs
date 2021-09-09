using MyNotes.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemListPage : BasePage
    {
        public ItemListPage()
        {
            InitializeComponent();
        }

        bool isAnimated;

        public async Task MoveItem(int oldIndex, int newIndex)
        {
            if (isAnimated)
                return;

            if (BindingContext == null || ViewModel == null)
                return;

            if (oldIndex == newIndex)
            {
                await ((ItemListViewModel)ViewModel).ExecuteRefreshCommand();
                return;
            }

            isAnimated = true;

            Device.BeginInvokeOnMainThread(() =>
                collectionView.MoveItem(oldIndex, newIndex, async () =>
                {
                    isAnimated = false;
                    // need to get rid of this after the bug was fixed
                    await ((ItemListViewModel)ViewModel).ExecuteRefreshCommand();
                })
            );
        }
    }
}