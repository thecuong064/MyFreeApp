using MyNotes.Enums;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyNotes.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmationPopup : PopupPage
    {
        public ConfirmationPopup()
        {
            InitializeComponent();
        }

        private static ConfirmationPopup _instance;
        private bool _preventTappingOutside = false;
        private static bool IsAppearing = false;
        private bool _preferAccept = true;
        private ConfirmationResult _leftButtonResult = ConfirmationResult.Decline;
        private ConfirmationResult _rightButtonResult = ConfirmationResult.Accept;

        private TaskCompletionSource<ConfirmationResult> Proccess;

        public static ConfirmationPopup Instance => _instance ?? (_instance = new ConfirmationPopup());

        #region Show

        public async Task<ConfirmationResult> Show(
            string title = null,
            string content = null,
            string acceptButtonText = "Yes",
            string declineButtonText = "No",
            bool preferAccept = true,
            bool preventTappingOutside = false)
        {
            if (IsAppearing)
                return ConfirmationResult.Cancel;

            IsAppearing = true;

            _preventTappingOutside = preventTappingOutside;

            titleLabel.Text = title;
            titleLabel.IsVisible = !string.IsNullOrWhiteSpace(title);

            contentLabel.Text = content;
            contentLabel.IsVisible = !string.IsNullOrWhiteSpace(content);

            HandleButtonPreference(preferAccept, acceptButtonText, declineButtonText);

            Proccess = new TaskCompletionSource<ConfirmationResult>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                await PopupNavigation.Instance.PushAsync(this);
            });

            var result = await GetResult();

            await Hide();
            IsAppearing = false;

            return result;
        }

        private void HandleButtonPreference(bool preferAccept,
            string acceptButtonText = "Yes",
            string declineButtonText = "No")
        {
            _preferAccept = preferAccept;
            leftButton.Text = _preferAccept ? declineButtonText : acceptButtonText;
            rightButton.Text = _preferAccept ? acceptButtonText : declineButtonText;

            _leftButtonResult = _preferAccept ? ConfirmationResult.Decline : ConfirmationResult.Accept;
            _rightButtonResult = _preferAccept ? ConfirmationResult.Accept : ConfirmationResult.Decline;
        }

        #endregion

        private async Task Hide()
        {
            if (PopupNavigation.Instance.PopupStack.Contains(this))
            {
                await PopupNavigation.Instance.PopAsync(true);
            }
        }

        #region GetResult

        public Task<ConfirmationResult> GetResult()
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
                Proccess.SetResult(ConfirmationResult.Cancel);  //remove this line to lock the hardware back button
            }
            return true;
        }

        #endregion

        #region OnBackgroundTapped

        private void OnBackgroundTapped(object sender, EventArgs e)
        {
            if (_preventTappingOutside)
                return;
            Proccess.SetResult(ConfirmationResult.Cancel);
            _preventTappingOutside = true;
        }

        #endregion

        private void leftButton_Clicked(object sender, EventArgs e)
        {
            if (Proccess != null)
            {
                Proccess.SetResult(_leftButtonResult);
            }
        }

        private void rightButton_Clicked(object sender, EventArgs e)
        {
            if (Proccess != null)
            {
                Proccess.SetResult(_rightButtonResult);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}