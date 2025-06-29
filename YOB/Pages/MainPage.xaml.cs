using System.Globalization;

namespace YOB.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.BindingContext is MainPageModel pageModel)
            {
                pageModel.PropertyChanged += this.PageModel_PropertyChanged;
            }
        }

        protected override void OnDisappearing()
        {
            if (this.BindingContext is MainPageModel pageModel)
            {
                pageModel.PropertyChanged -= this.PageModel_PropertyChanged;
            }

            base.OnDisappearing();
        }

        private async void PageModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.GetDayItem() is Element item)
            {
                await Task.Delay(100);
                await this.DayScroller.ScrollToAsync(item, ScrollToPosition.Start, false);
            }
        }

        private async void CalendarButton_Clicked(object sender, EventArgs e)
        {
            if (this.GetDayItem() is Element item)
            {
                await this.DayScroller.ScrollToAsync(item, ScrollToPosition.Start, true);
            }
        }

        private Element? GetDayItem()
        {
            Element? retVal = null;

            var now = DateTime.Now.Date;
            if (this.BindingContext is MainPageModel pageModel &&
                pageModel.Projects.FirstOrDefault(
                    d => DateTime.Parse($"{d.Name}/{now.Year}", CultureInfo.InvariantCulture) >= now)
                    is Models.Project day &&
                pageModel.Projects.IndexOf(day) is int index &&
                index >= 0 &&
                this.Days[index] is Element item)
            {
                retVal = item;
            }

            return retVal;
        }
    }
}