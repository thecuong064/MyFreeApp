using MyNotes.ViewModels;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace MyNotes.Views
{
    public class BasePage : ContentPage
    {
        public BasePage()
        {
        }

        #region Properties

        protected bool IsAppeared { get; private set; }
        public ViewModelBase ViewModel{ get; set; }

        #endregion

        protected override void OnBindingContextChanged()
        {
            if (BindingContext != null)
            {
                ViewModel = (ViewModelBase)BindingContext;
                ViewModel.Page = this;
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                if (ViewModel == null && BindingContext != null)
                {
                    ViewModel = (ViewModelBase)BindingContext;
                    ViewModel.Page = this;
                }

                if (!IsAppeared)

                IsAppeared = true;

                //for iOS only
                On<iOS>().SetUseSafeArea(true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }
    }
}
