using MyNotes.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyNotes.Extensions
{
    public static class ShellExtensions
    {
        private static string GetRouteParams(ShellNavigationParameters parameters)
        {
            var routeParams = "?";
            if (parameters != null && parameters.Count() > 0)
            {
                foreach (var key in parameters.GetKeys())
                {
                    var paramStr = $"{key}={parameters.GetValue<string>(key)}&";
                    routeParams += paramStr;
                }
            }

            return routeParams.Length > 1 ? routeParams : "";
        }

        public static Task NavigateToAsync(this Shell shell, string destination, ShellNavigationParameters parameters = null, bool animate = true)
        {
            var route = $"{destination}";
            route += GetRouteParams(parameters);
            return Shell.Current.GoToAsync(route, animate: animate);
        }

        public static Task NavigateBackAsync(this Shell shell, ShellNavigationParameters parameters = null, bool animate = true)
        {
            var route = "..";
            route += GetRouteParams(parameters);
            return Shell.Current.GoToAsync(route, animate: animate);
        }
    }
}
