using CommunityToolkit.Mvvm.Input;
using YOB.Models;

namespace YOB.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}