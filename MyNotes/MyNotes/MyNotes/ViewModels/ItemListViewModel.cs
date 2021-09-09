using System;
using System.Collections.Generic;
using MvvmHelpers;
using MyNotes.Models;
using System.Windows.Input;
using MvvmHelpers.Commands;
using Xamarin.Forms;
using System.Threading.Tasks;
using MyNotes.Views;
using MyNotes.Services;
using System.Web;
using MyNotes.Extensions;
using MyNotes.Enums;
using System.Linq;
using MyNotes.Popups;
using System.ComponentModel;
using Command = Xamarin.Forms.Command;
using Xamarin.CommunityToolkit.Extensions;
using MyNotes.Constants;

namespace MyNotes.ViewModels
{
    [QueryProperty(nameof(SheetId), ParamKey.SheetId)]
    [QueryProperty(nameof(Title), ParamKey.Title)]
    public class ItemListViewModel : ViewModelBase, IQueryAttributable, INotifyPropertyChanged
    {
        public ItemListViewModel()
        {
            AddItemCommand = new AsyncCommand(ExecuteAddItemCommand);
            RefreshCommand = new AsyncCommand(ExecuteRefreshCommand);
            BackCommand = new AsyncCommand(ExecuteBackCommand);
            EditSheetCommand = new AsyncCommand(ExecuteEditSheetCommand);

            SelectItemCommand = new Command(ExecuteSelectItemCommand);
            DeleteItemCommand = new AsyncCommand<Item>(ExecuteDeleteItemCommand);
            UncompleteItemCommand = new AsyncCommand<Item>(ExecuteUncompleteItemCommand);
            CompleteItemCommand = new AsyncCommand<Item>(ExecuteCompleteItemCommand);
            PinItemCommand = new AsyncCommand<Item>(ExecutePinItemCommand);
            UnpinItemCommand = new AsyncCommand<Item>(ExecuteUnpinItemCommand);

            Items = new ObservableRangeCollection<Item>();
        }

        #region Properties

        private string _sheetId;
        public string SheetId
        {
            get => _sheetId;
            set
            {
                if (value != _sheetId && !string.IsNullOrWhiteSpace(value))
                {
                    _sheetId = value;
                }
            }
        }

        private Sheet _sheet;
        
        private ObservableRangeCollection<Item> items;
        public ObservableRangeCollection<Item> Items    
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        private Item selectedItem;
        public Item SelectedItem 
        { 
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
                ExecuteSelectItemCommand();
            }
        }

        #endregion

        #region ApplyQueryAttributes

        public async void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            if (query == null || query.Count == 0)
                return;

            query.TryGetValue(nameof(CanLoadMore), out string str);
            var needRefreshStr = HttpUtility.UrlDecode(str);
            bool.TryParse(needRefreshStr, out bool needRefresh);

            query.TryGetValue(nameof(SheetId), out var sheetId);
            SheetId = sheetId;

            if (needRefresh || !string.IsNullOrWhiteSpace(SheetId))
            {
                await ExecuteRefreshCommand();
            }

            if (!string.IsNullOrWhiteSpace(SheetId))
            {
                _sheet = await DatabaseService.GetSheet(SheetId).ConfigureAwait(false);
            }
        }

        #endregion

        #region LoadItems

        private async Task LoadItems(string sheetId)
        {
            var items = await DatabaseService.GetItems(sheetId);
            Items = items != null ? new ObservableRangeCollection<Item>(items.OrderBy(x => x.Ordering)) : new ObservableRangeCollection<Item>();
        }

        #endregion

        #region RefreshCommand

        public ICommand RefreshCommand { get; set; }
        public async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await LoadItems(SheetId);

            IsBusy = false;

            IsRefreshing = false;
        }

        #endregion

        #region SelectItemCommand

        public ICommand SelectItemCommand { get; set; }
        public async void ExecuteSelectItemCommand()
        {
            if (SelectedItem == null || string.IsNullOrWhiteSpace(SelectedItem.Id))
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            var result = await ItemDetailsPopup.Instance.Show(item: SelectedItem, preventTappingOutside: false);

            IsBusy = false;

            SelectedItem = null;

            if (result.isSaved)
            {
                var success = await DatabaseService.UpdateItem(result.item);
                if (success)
                {
                    Application.Current.MainPage.DisplayToastAsync($"Update \"{result.item.Title.GetFirstSubstringWithEllipsis(5)}\" successfully", DefinedConst.TOAST_LENGTH_UPDATE);
                    DatabaseService.UpdateSheet(_sheet);
                    await ExecuteRefreshCommand();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(null, "Something went wrong!\nPlease try again", "OK");
                }
            }
        }

        #endregion

        #region AddItemCommand

        public AsyncCommand AddItemCommand { get; }
        private async Task ExecuteAddItemCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var result = await ItemDetailsPopup.Instance.Show(item: null, preventTappingOutside: false, isEditLocked: false);

            IsBusy = false;

            if (result.isSaved)
            {
                result.item.SheetId = SheetId;
                result.item.Ordering = Items.Where(t => t.Status == ItemStatus.Pinned).Count() + Items.Where(t => t.Status == ItemStatus.Todo).Count();
                var success = await DatabaseService.AddItem(result.item);

                if (success)
                {
                    Application.Current.MainPage.DisplayToastAsync($"Add \"{result.item.Title.GetFirstSubstringWithEllipsis(5)}\" successfully", DefinedConst.TOAST_LENGTH_ADD);
                    DatabaseService.UpdateSheet(_sheet);
                    await ExecuteRefreshCommand();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(null, "Something went wrong!\nPlease try again", "OK");
                }
            }
        }

        #endregion

        #region DeleteItemCommand

        public AsyncCommand<Item> DeleteItemCommand { get; }
        private async Task ExecuteDeleteItemCommand(Item item)
        {
            if (item == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            var confirmationResult = await ConfirmationPopup.Instance.Show(
                title: "Warning",
                content: $"Are you sure want to delete \"{item.Title.GetFirstSubstringWithEllipsis(5)}\"?",
                preferAccept: false);

            if (confirmationResult != ConfirmationResult.Accept)
            {
                IsBusy = false;
                return;
            }

            IsBusy = false;

            var oldIndex = Items.IndexOf(item);
            Items.Remove(item);

            var shouldUndo = await Application.Current.MainPage.DisplaySnackBarAsync(
                $"Delete \"{item.Title.GetFirstSubstringWithEllipsis(5)}\" successfully",
                "Undo",
                null,
                TimeSpan.FromMilliseconds(DefinedConst.TOAST_LENGTH_DELETE));

            if (shouldUndo)
            {
                Items.Insert(oldIndex, item);
                await Application.Current.MainPage.DisplayToastAsync($"Undo delete \"{item.Title.GetFirstSubstringWithEllipsis(5)}\" successfully", DefinedConst.TOAST_LENGTH_UNDO);
            }
            else
            {
                var success = await DatabaseService.RemoveItem(item.Id);
                if (success)
                {
                    DatabaseService.UpdateSheet(_sheet);
                    await ExecuteRefreshCommand();
                }
                else
                {
                    Items.Insert(oldIndex, item);
                    await Application.Current.MainPage.DisplayAlert(null, "Something went wrong!\nPlease try again", "OK");
                }
            }
        }

        #endregion
        
        #region BackCommand

        public AsyncCommand BackCommand { get; }
        private async Task ExecuteBackCommand()
        {
            ShellNavigationParameters navParams = new ShellNavigationParameters
            {
                //{ nameof(CanLoadMore), bool.TrueString },
            };

            await Shell.Current.NavigateBackAsync(navParams);
        }

        #endregion

        #region CompleteItemCommand

        public AsyncCommand<Item> CompleteItemCommand { get; }
        private async Task ExecuteCompleteItemCommand(Item item)
        {
            if (item == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            item.Status = ItemStatus.Completed;
            await UpdateStatus(item);

            IsBusy = false;
        }

        #endregion

        #region UncompleteItemCommand

        public AsyncCommand<Item> UncompleteItemCommand { get; }
        private async Task ExecuteUncompleteItemCommand(Item item)
        {
            if (item == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            item.Status = ItemStatus.Todo;
            await UpdateStatus(item);

            IsBusy = false;
        }

        #endregion

        #region UnpinItemCommand

        public AsyncCommand<Item> UnpinItemCommand { get; }
        private async Task ExecuteUnpinItemCommand(Item item)
        {
            if (item == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            item.Status = ItemStatus.Todo;
            await UpdateStatus(item);

            IsBusy = false;
        }

        #endregion

        #region PinItemCommand

        public AsyncCommand<Item> PinItemCommand { get; }
        private async Task ExecutePinItemCommand(Item item)
        {
            if (item == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            item.Status = ItemStatus.Pinned;
            await UpdateStatus(item);

            IsBusy = false;
        }

        #endregion

        #region UpdateStatus

        private async Task UpdateStatus(Item item)
        {
            int oldIndex = Items.IndexOf(item);
            int newIndex = oldIndex;

            switch (item.Status)
            {
                case ItemStatus.Pinned:
                    newIndex = Items.Where(t => t.Status == ItemStatus.Pinned).Count() - 1;
                    break;
                case ItemStatus.Todo:
                    newIndex = Items.Where(t => t.Status == ItemStatus.Pinned).Count() + Items.Where(t => t.Status == ItemStatus.Todo).Count() - 1;
                    break;
                case ItemStatus.Completed:
                    newIndex = Items.Count() - 1;
                    break;
            }

            if (newIndex != oldIndex)
            {
                Items.Move(oldIndex, newIndex);
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Ordering = i;
                }
                await DatabaseService.UpdateItems(Items);
            }
            else
            {
                await DatabaseService.UpdateItem(item);
            }

            // need to get rid of this after the bug was fixed
            IsBusy = false;

            DatabaseService.UpdateSheet(_sheet);
            if (Page != null)
            {
                var page = (ItemListPage)Page;
                await page.MoveItem(oldIndex, newIndex);
            }
        }

        #endregion

        #region EditSheetCommand

        public AsyncCommand EditSheetCommand { get; }
        private async Task ExecuteEditSheetCommand()
        {
            if (SheetId == null || _sheet == null)
                return;

            if (IsBusy)
                return;

            IsBusy = true;

            var newName = await SheetNameInputPopup.Instance.Show(sheetName: Title);

            if (string.IsNullOrWhiteSpace(newName) || newName.Equals(Title))
            {
                IsBusy = false;
                return;
            }

            _sheet.Name = newName;

            var success = await DatabaseService.UpdateSheet(_sheet);
            if (success)
            {
                Application.Current.MainPage.DisplayToastAsync($"Update \"{_sheet.Name.GetFirstSubstringWithEllipsis(5)}\" successfully", DefinedConst.TOAST_LENGTH_UPDATE);
                Title = newName;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(null, "Something went wrong!\nPlease try again", "OK");
            }

            IsBusy = false;
        }

        #endregion
    }
}
