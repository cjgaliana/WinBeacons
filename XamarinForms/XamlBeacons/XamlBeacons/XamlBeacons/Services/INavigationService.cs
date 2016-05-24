namespace XamlBeacons.Services
{
    public enum PageKey
    {
        BeaconsList,
        BeaconDetails
    }

    public interface INavigationService
    {
        object NavigationParameter { get; set; }

        bool CanGoBack { get; }

        void GoBack();

        void NavigateTo(PageKey page);

        void NavigateTo(PageKey page, object parameters);

        void ClearNavigationStack();
    }
}