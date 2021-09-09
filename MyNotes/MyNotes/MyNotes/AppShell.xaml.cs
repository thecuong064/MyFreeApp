using MyNotes.Views;
using Xamarin.Forms;

namespace MyNotes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ItemListPage), typeof(ItemListPage));
        }
    }
}
