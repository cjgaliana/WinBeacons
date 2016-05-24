using System;
using System.Linq;
using Xamarin.Forms;
using XamlBeacons.Views;

namespace XamlBeacons.Services
{
    public class NavigationService : INavigationService
    {
        private NavigationPage _frame;

        public bool CanGoBack => _frame.Navigation.NavigationStack.Any();

        public void GoBack()
        {
            if (CanGoBack)
            {
                _frame.Navigation.PopAsync(true);
            }
        }

        public void NavigateTo(PageKey page)
        {
            NavigateTo(page, null);
        }

        public void NavigateTo(PageKey page, object parameters)
        {
            NavigationParameter = parameters;

            ContentPage target = null;
            switch (page)
            {
                case PageKey.BeaconsList:
                    target = new BeaconsListView();
                    break;

                case PageKey.BeaconDetails:
                    target = new BeaconDetailsView();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }

            _frame.Navigation.PushAsync(target);
        }

        public object NavigationParameter { get; set; }

        public void ClearNavigationStack()
        {
        }

        public void InitializeFrame(NavigationPage frame)
        {
            _frame = frame;
        }
    }
}