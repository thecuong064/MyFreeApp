using MvvmHelpers;
using MvvmHelpers.Commands;
using MyNotes.Constants;
using MyNotes.Enums;
using MyNotes.Extensions;
using MyNotes.Models;
using MyNotes.Popups;
using MyNotes.Services;
using MyNotes.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Command = Xamarin.Forms.Command;

namespace MyNotes.ViewModels
{
    public class SheetListViewModel : ViewModelBase, IQueryAttributable
    {
        public SheetListViewModel()
        {
            SelectSheetCommand = new Command(ExecuteSelectSheetCommand);
            AddCommand = new AsyncCommand(ExecuteAddCommand);
            RefreshCommand = new AsyncCommand(ExecuteRefreshCommand);

            DeleteSheetCommand = new AsyncCommand<Sheet>(ExecuteDeleteSheetCommand);
            PinSheetCommand = new AsyncCommand<Sheet>(ExecutePinSheetCommand);
            UnpinSheetCommand = new AsyncCommand<Sheet>(ExecuteUnpinSheetCommand);

            Sheets = new ObservableRangeCollection<Sheet>();
        }

        #region Properties

        private ObservableRangeCollection<Sheet> _sheets;
        public ObservableRangeCollection<Sheet> Sheets
        {
            get => _sheets;
            set => SetProperty(ref _sheets, value);
        }

        private Sheet _selectedSheet;
        public Sheet SelectedSheet
        {
            get => _selectedSheet;
            set
            {
                SetProperty(ref _selectedSheet, value);
                ExecuteSelectSheetCommand();
            }

        }

        #endregion

        #region ApplyQueryAttributes

        public async void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            if (IsFirstTimeAppeared)
            {
                await ExecuteRefreshCommand();

                IsFirstTimeAppeared = false;

                return;
            }

            if (query == null || query.Count == 0)
                return;

            var str = HttpUtility.UrlDecode(query[nameof(CanLoadMore)]);
            bool.TryParse(str, out bool needRefresh);
            if (needRefresh)
            {
                await ExecuteRefreshCommand();
            }
        }

        #endregion

        #region RefreshCommand

        public AsyncCommand RefreshCommand { get; set; }
        public async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await LoadSheets();

            IsBusy = false;

            IsRefreshing = false;
        }

        #endregion

        #region LoadSheets

        private async Task LoadSheets()
        {
            var sheets = await DatabaseService.GetSheets();

            if (sheets != null)
            {
                foreach (var sheet in sheets)
                {
                    var items = await DatabaseService.GetItems(sheet.Id);
                    sheet.ItemCount = items.Count;
                    sheet.CompletedItemCount = items.Where(item => item.Status.Equals(ItemStatus.Completed)).Count();
                }
            }

            Sheets = sheets != null ? new ObservableRangeCollection<Sheet>(sheets.OrderBy(x => x.Ordering)) : new ObservableRangeCollection<Sheet>();

            DatabaseService.UpdateSheets(Sheets, isLastDateUpdated: false);
        }

        #endregion

        #region AddCommand

        public AsyncCommand AddCommand { get; }
        private async Task ExecuteAddCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var newName = await SheetNameInputPopup.Instance.Show();

            if (string.IsNullOrWhiteSpace(newName))
            {
                IsBusy = false;
                return;
            }

            var addSuccess = await DatabaseService.AddSheet(
                new Sheet
                {
                    Name = newName,
                    Ordering = Sheets.Count(),
                    LastUpdated = DateTimeOffset.Now
                });

            IsBusy = false;

            if (addSuccess)
            {
                Application.Current.MainPage.DisplayToastAsync($"Add \"{newName.GetFirstSubstringWithEllipsis(5)}\" successfully", DefinedConst.TOAST_LENGTH_ADD);
                await ExecuteRefreshCommand();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(null, "Something went wrong!\nPlease try again", "OK");
            }
        }

        #endregion

        #region DeleteSheetCommand

        public AsyncCommand<Sheet> DeleteSheetCommand { get; }
        private async Task ExecuteDeleteSheetCommand(Sheet sheet)
        {
            if (sheet == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            var confirmationResult = await ConfirmationPopup.Instance.Show(
                title: "Warning",
                content: $"Are you sure want to delete \"{sheet.Name}\"?",
                preferAccept: false);

            if (confirmationResult != ConfirmationResult.Accept)
            {
                IsBusy = false;
                return;
            }

            IsBusy = false;

            var oldIndex = Sheets.IndexOf(sheet);
            Sheets.Remove(sheet);

            var shouldUndo = await Application.Current.MainPage.DisplaySnackBarAsync(
                $"Delete \"{sheet.Name.GetFirstSubstringWithEllipsis(5)}\" successfully",
                "Undo",
                null,
                TimeSpan.FromMilliseconds(DefinedConst.TOAST_LENGTH_DELETE));
                
            if (shouldUndo)
            {
                Sheets.Insert(oldIndex, sheet);
                await Application.Current.MainPage.DisplayToastAsync($"Undo delete \"{sheet.Name.GetFirstSubstringWithEllipsis(5)}\" successfully", DefinedConst.TOAST_LENGTH_UNDO);
            }
            else
            {
                var success = await DatabaseService.RemoveSheet(sheet.Id);
                if (success)
                {
                    await ExecuteRefreshCommand();
                }
                else
                {
                    Sheets.Insert(oldIndex, sheet);
                    await Application.Current.MainPage.DisplayAlert(null, "Something went wrong!\nPlease try again", "OK");
                }
            }
        }

        #endregion

        #region SelectSheetCommand

        public ICommand SelectSheetCommand { get; }
        private async void ExecuteSelectSheetCommand()
        {
            if (SelectedSheet == null || string.IsNullOrWhiteSpace(SelectedSheet.Id))
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            var navParams = new ShellNavigationParameters
            {
                { ParamKey.SheetId, SelectedSheet.Id },
                { ParamKey.Title, SelectedSheet.Name }
            };

            await Shell.Current.NavigateToAsync(nameof(ItemListPage), navParams);

            SelectedSheet = null;

            IsBusy = false;
        }

        #endregion

        #region UnpinSheetCommand

        public AsyncCommand<Sheet> UnpinSheetCommand { get; }
        private async Task ExecuteUnpinSheetCommand(Sheet sheet)
        {
            if (sheet == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            sheet.Status = SheetStatus.Default;
            await UpdateStatus(sheet);
            
            IsBusy = false;
        }

        #endregion

        #region PinSheetCommand

        public AsyncCommand<Sheet> PinSheetCommand { get; }
        private async Task ExecutePinSheetCommand(Sheet sheet)
        {
            if (sheet == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            sheet.Status = SheetStatus.Pinned;
            await UpdateStatus(sheet);

            IsBusy = false;
        }

        #endregion

        #region UpdateStatus

        private async Task UpdateStatus(Sheet sheet)
        {
            int oldIndex = Sheets.IndexOf(sheet);
            int newIndex = oldIndex;

            switch (sheet.Status)
            {
                case SheetStatus.Pinned:
                    newIndex = Sheets.Where(t => t.Status == SheetStatus.Pinned).Count() - 1;
                    break;
                case SheetStatus.Default:
                    newIndex = Sheets.Count() - 1;
                    break;
            }

            if (newIndex != oldIndex)
            {
                Sheets.Move(oldIndex, newIndex);
                for (int i = 0; i < Sheets.Count; i++)
                {
                    Sheets[i].Ordering = i;
                }
                await DatabaseService.UpdateSheets(Sheets);
            }
            else
            {
                await DatabaseService.UpdateSheet(sheet);
            }

            // need to get rid of this after the bug was fixed
            IsBusy = false;

            if (Page != null)
            {
                var page = (SheetListPage)Page;
                await page.MoveItem(oldIndex, newIndex);
            }
        }

        #endregion
    }
}
