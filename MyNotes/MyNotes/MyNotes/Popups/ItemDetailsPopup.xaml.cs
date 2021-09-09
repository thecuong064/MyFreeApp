using MyNotes.Models;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyNotes.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailsPopup : PopupPage
    {
        public ItemDetailsPopup()
        {
            InitializeComponent();
        }

        private static ItemDetailsPopup _instance;
        private bool _preventTappingOutside = false;
        private static bool IsAppearing = false;
        private Item currentItem;

        private TaskCompletionSource<(Item, bool)> Proccess;

        public static ItemDetailsPopup Instance => _instance ??= new ItemDetailsPopup();

        #region Show

        public async Task<(Item item, bool isSaved)> Show(Item item = null,
            bool preventTappingOutside = true,
            bool isEditLocked = true)
        {
            if (IsAppearing)
                return (item, false);

            IsAppearing = true;

            _preventTappingOutside = preventTappingOutside;

            itemContent.Text = "";
            descriptionContent.Text = "";
            currentItem = item ?? new Item();

            if (item != null)
            {
                itemContent.Text = item.Title;
                descriptionContent.Text = item.Description;
            }

            this.isLocked = isEditLocked;
            GoToState(isLocked);

            Proccess = new TaskCompletionSource<(Item, bool)>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await PopupNavigation.Instance.PushAsync(this);
            });

            var result = await GetResult();

            await Hide();
            IsAppearing = false;

            return result;
        }

        #endregion

        private Task Hide()
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (PopupNavigation.Instance.PopupStack.Contains(this))
                {
                    await PopupNavigation.Instance.PopAsync(true);
                }
            });
        }

        #region GetResult

        public Task<(Item, bool)> GetResult()
        {
            if (Proccess != null)
            {
                return Proccess.Task;
            }

            return null;
        }

        #endregion

        #region OnBackButtonPressed

        protected override bool OnBackButtonPressed()
        {
            if (Proccess != null)
            {
                Proccess.SetResult((currentItem, false));  //remove this line to lock the hardware back button
            }
            return true;
        }

        #endregion

        #region OnBackgroundTapped

        private void OnBackgroundTapped(object sender, EventArgs e)
        {
            if (_preventTappingOutside)
                return;
            Proccess.SetResult((currentItem, false));
            _preventTappingOutside = true;
        }

        #endregion

        bool isLocked;
        private void editLockButton_Clicked(object sender, EventArgs e)
        {
            isLocked = !isLocked;
            GoToState(isLocked);
        }

        void GoToState(bool isLocked)
        {
            string visualState = isLocked ? "Locked" : "Unlocked";
            VisualStateManager.GoToState(editLockButton, visualState);
            itemContent.IsEnabled = !isLocked;
            descriptionContent.IsEnabled = !isLocked;
        }

        private void cancelButton_Clicked(object sender, EventArgs e)
        {
            Proccess.SetResult((currentItem, false));
        }

        private void saveButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(itemContent.Text))
            {
                itemContent.HasError = true;
                return;
            }

            currentItem.Title = itemContent.Text?.Trim();
            currentItem.Description = descriptionContent.Text?.Trim();
            Proccess.SetResult((currentItem, true));
        }

        private void itemContent_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(itemContent.Text))
            {
                itemContent.HasError = false;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}