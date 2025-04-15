using CommunityToolkit.Mvvm.Input;
using DotnetPublishBug.Models;

namespace DotnetPublishBug.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}