using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace MyNotes.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SheetNameInputPopup : PopupPage
    {
        public SheetNameInputPopup()
        {
            InitializeComponent();
        }

        private static SheetNameInputPopup _instance;
        private bool _preventTappingOutside = false;
        private static bool IsAppearing = false;

        private TaskCompletionSource<string> Proccess;

        public static SheetNameInputPopup Instance => _instance ??= new SheetNameInputPopup();

        #region Show

        public async Task<string> Show(string sheetName = null, bool preventTappingOutside = false)
        {
            if (IsAppearing)
                return null;

            IsAppearing = true;

            _preventTappingOutside = preventTappingOutside;

            nameContent.Text = sheetName ?? "";

            Proccess = new TaskCompletionSource<string>();

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

        public Task<string> GetResult()
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
                Proccess.SetResult(null);  //remove this line to lock the hardware back button
            }
            return true;
        }

        #endregion

        #region OnBackgroundTapped

        private void OnBackgroundTapped(object sender, EventArgs e)
        {
            if (_preventTappingOutside)
                return;
            Proccess.SetResult(null);
            _preventTappingOutside = true;
        }

        #endregion

        private void cancelButton_Clicked(object sender, EventArgs e)
        {
            Proccess.SetResult(null);
        }

        private void saveButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameContent.Text))
            {
                nameContent.HasError = true;
                return;
            }

            Proccess.SetResult(nameContent.Text?.Trim());
        }

        private void nameContent_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(nameContent.Text))
            {
                nameContent.HasError = false;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}