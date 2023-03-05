using TenisApp.Shared.ViewModel;
using TenisApp.Core.Model;
using System.ComponentModel;

namespace TenisApp.Shared.Tests;

public class ObservableViewModelTest
{
    [Fact]
    public void ObservableViewModel_PropertySetter_ShouldRaiseNotifyPropertyChangedEvent()
    {
        // Arrange
        var topic = new CourtViewModel();
        var raises = 0;
        topic.PropertyChanged += (o,p) => raises = p.PropertyName == nameof(topic.Court) ? raises + 1 : raises;

        // Act
        topic.Court = new Court();

        // Assert
        Assert.Equal(1, raises);
    }
}   