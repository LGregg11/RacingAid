using NUnit.Framework;
using Moq;
using RacingAidWpf.OverlayManagement;
using RacingAidWpf.ViewModel;
using RacingAidWpf.View;


namespace RacingAidWpf.Tests.ViewModel
{
    [Apartment(ApartmentState.STA)]
    [TestFixture]
    public class MainWindowViewModelTests
    {
        private Mock<OverlayController> _overlayControllerMock;
        private MainWindowViewModel _viewModel;

        [SetUp]
        public void Setup()
        {


            // Mock the OverlayController
            _overlayControllerMock = new Mock<OverlayController>();

            var telemetryOverlayMock = new Mock<TelemetryOverlay>();
            _overlayControllerMock
                .Setup(c => c.AddOverlay(It.IsAny<TelemetryOverlay>()))
                .Verifiable();
            
            // Configure mock behavior for IsRepositioningEnabled
            _overlayControllerMock.SetupProperty(o => o.IsRepositioningEnabled, false);

            // Create the ViewModel instance, injecting the mocked OverlayController
            _viewModel = new MainWindowViewModel(_overlayControllerMock.Object);
        }

        [Test]
        public void ToggleOverlayRepositioning_TogglesIsRepositionEnabled()
        {
            // Arrange
            _viewModel.Start(); // Ensure IsStarted is true

            // Act
            _viewModel.ToggleOverlayRepositioning();

            // Assert
            Assert.That(_viewModel.IsRepositionEnabled, Is.True, "IsRepositionEnabled should be true after toggling.");

            // Act again to toggle back
            _viewModel.ToggleOverlayRepositioning();

            // Assert
            Assert.That(_viewModel.IsRepositionEnabled, Is.False, "IsRepositionEnabled should be false after toggling again.");
        }

        [Test]
        public void ToggleOverlayRepositioning_DoesNothingIfNotStarted()
        {
            // Arrange
            _viewModel.Stop(); // Ensure IsStarted is false

            // Act
            _viewModel.ToggleOverlayRepositioning();

            // Assert
            Assert.That(_viewModel.IsRepositionEnabled, Is.False, "IsRepositionEnabled should remain false if IsStarted is false.");
            Assert.That(_overlayControllerMock.Object.IsRepositioningEnabled, Is.False, "OverlayController.IsRepositioningEnabled should remain false.");
        }

        [Test]
        public void IsRepositionEnabled_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedTriggered = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.IsRepositionEnabled))
                {
                    propertyChangedTriggered = true;
                }
            };

            // Act
            _viewModel.Start();
            _viewModel.ToggleOverlayRepositioning();

            // Assert
            Assert.That(propertyChangedTriggered, Is.True, "PropertyChanged should be triggered for IsRepositionEnabled.");
        }
    }
}
