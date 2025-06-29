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

        private async void CalendarButton_Clicked(object sender, EventArgs e)
        {
            var now = DateTime.Now.Date;
            if (this.BindingContext is MainPageModel pageModel &&
                pageModel.Projects.FirstOrDefault(
                    d => DateTime.Parse($"{d.Name}/{now.Year}", CultureInfo.InvariantCulture) >= now)
                    is Models.Project day &&
                pageModel.Projects.IndexOf(day) is int index &&
                index >= 0 &&
                this.Days[index] is Element item)
            {
                await this.DayScroller.ScrollToAsync(item, ScrollToPosition.Start, true);
            }
        }
    }
}