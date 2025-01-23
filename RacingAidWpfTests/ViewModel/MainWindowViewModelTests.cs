using Moq;
using RacingAidWpf.OverlayManagement;
using RacingAidWpf.View;
using RacingAidWpf.ViewModel;

namespace RacingAidWpfTests.ViewModel
{
    [Apartment(ApartmentState.STA)]
    [TestFixture]
    public class MainWindowViewModelTests
    {
        private Mock<OverlayController> overlayControllerMock;
        private MainWindowViewModel cut;

        [SetUp]
        public void Setup()
        {
            // Mock the OverlayController
            overlayControllerMock = new Mock<OverlayController>();
            overlayControllerMock
                .Setup(c => c.AddOverlay(It.IsAny<TelemetryOverlay>()))
                .Verifiable();
            
            // Configure mock behavior for IsRepositioningEnabled
            overlayControllerMock.SetupProperty(o => o.IsRepositioningEnabled, false);

            // Create the ViewModel instance, injecting the mocked OverlayController
            cut = new MainWindowViewModel(overlayControllerMock.Object);
        }

        [Test]
        public void ToggleOverlayRepositioning_TogglesIsRepositionEnabled()
        {
            // Arrange
            cut.Start(); // Ensure IsStarted is true

            // Act
            cut.ToggleOverlayRepositioning();

            // Assert
            Assert.That(cut.IsRepositionEnabled, Is.True, "IsRepositionEnabled should be true after toggling.");

            // Act again to toggle back
            cut.ToggleOverlayRepositioning();

            // Assert
            Assert.That(cut.IsRepositionEnabled, Is.False, "IsRepositionEnabled should be false after toggling again.");
        }

        [Test]
        public void ToggleOverlayRepositioning_DoesNothingIfNotStarted()
        {
            // Arrange
            cut.Stop(); // Ensure IsStarted is false

            // Act
            cut.ToggleOverlayRepositioning();

            // Assert
            Assert.That(cut.IsRepositionEnabled, Is.False, "IsRepositionEnabled should remain false if IsStarted is false.");
            Assert.That(overlayControllerMock.Object.IsRepositioningEnabled, Is.False, "OverlayController.IsRepositioningEnabled should remain false.");
        }

        [Test]
        public void IsRepositionEnabled_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedTriggered = false;
            cut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(cut.IsRepositionEnabled))
                {
                    propertyChangedTriggered = true;
                }
            };

            // Act
            cut.Start();
            cut.ToggleOverlayRepositioning();

            // Assert
            Assert.That(propertyChangedTriggered, Is.True, "PropertyChanged should be triggered for IsRepositionEnabled.");
        }
    }
}
