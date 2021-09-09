using MyNotes.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MyNotes.Services
{
    public class DatabaseService
    {
        static SQLiteAsyncConnection db;
        public static async Task Init()
        {
            if (db != null)
                return;

            // Get an absolute path to the database file
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "FreeApp.db");

            db = new SQLiteAsyncConnection(databasePath);

            await CreateTableAsync();
        }

        private static Task CreateTableAsync()
        {
            var tasks = new List<Task>
            {
                db.CreateTableAsync<Sheet>(),
                db.CreateTableAsync<Item>()
            };
            return Task.WhenAll(tasks);
        }

        private static string NewId()
        {
            return Guid.NewGuid().ToString();
        }

        #region Item methods

        public static async Task AddItems(IEnumerable<Item> items)
        {
            await Init();
        }

        public static async Task<bool> AddItem(Item item)
        {
            await Init();

            item.Id ??= NewId();
            return await db.InsertAsync(item) > 0;
        }

        public static async Task<bool> RemoveItem(string id)
        {
            await Init();

            return await db.DeleteAsync<Item>(id) > 0;
        }

        public static async Task<List<Item>> GetItems()
        {
            await Init();

            return await db.Table<Item>().ToListAsync();
        }

        public static async Task<List<Item>> GetItems(string sheetId)
        {
            await Init();

            return await db.Table<Item>().Where(item => item.SheetId.Equals(sheetId)).ToListAsync();
        }

        public static async Task<Item> GetItem(string id)
        {
            await Init();

            return await db.GetAsync<Item>(id);
        }

        public static async Task<bool> UpdateItem(Item item)
        {
            await Init();

            return await db.UpdateAsync(item) > 0;
        }

        public static async Task<bool[]> UpdateItems(ObservableCollection<Item> items)
        {
            await Init();

            var taskCount = items.Count;
            Task<bool>[] tasks = new Task<bool>[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = UpdateItem(items[i]);
            }

            return await Task.WhenAll(tasks);
        }

        #endregion

        #region Sheet methods

        public static async Task AddSheets(IEnumerable<Sheet> sheets)
        {
            await Init();
        }

        public static async Task<bool> AddSheet(Sheet sheet)
        {
            await Init();

            sheet.Id = NewId();

            return await db.InsertAsync(sheet) > 0;
        }

        public static async Task<bool> RemoveSheet(string id)
        {
            await Init();

            return await db.DeleteAsync<Sheet>(id) > 0;
        }

        public static async Task<List<Sheet>> GetSheets()
        {
            await Init();

            return await db.Table<Sheet>().ToListAsync();
        }

        public static async Task<Sheet> GetSheet(string id)
        {
            await Init();

            return await db.GetAsync<Sheet>(id);
        }

        public static async Task<bool> UpdateSheet(Sheet sheet, bool isLastDateUpdated = true)
        {
            await Init();

            if (isLastDateUpdated)
            {
                sheet.LastUpdated = DateTimeOffset.Now;
            }

            return await db.UpdateAsync(sheet) > 0;
        }

        public static async Task<bool[]> UpdateSheets(ObservableCollection<Sheet> sheets, bool isLastDateUpdated = true)
        {
            await Init();

            var taskCount = sheets.Count;
            Task<bool>[] tasks = new Task<bool>[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = UpdateSheet(sheets[i], isLastDateUpdated);
            }

            return await Task.WhenAll(tasks);
        }

        #endregion
    }
}
