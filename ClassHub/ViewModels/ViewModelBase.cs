// ViewModelBase.cs
// 所有ViewModel的基类，实现INotifyPropertyChanged
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClassHub.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
