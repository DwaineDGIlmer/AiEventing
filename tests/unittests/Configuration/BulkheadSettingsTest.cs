namespace Core.Configuration.Tests
{
    public class BulkheadSettingsTest
    {
        [Fact]
        public void DefaultValues_ShouldBeSetCorrectly()
        {
            var settings = new BulkheadSettings();

            Assert.True(settings.Enabled);
            Assert.Equal(10, settings.MaxParallelization);
            Assert.Equal(20, settings.MaxQueuingActions);
        }

        [Fact]
        public void CanSet_EnabledProperty()
        {
            var settings = new BulkheadSettings { Enabled = false };
            Assert.False(settings.Enabled);

            settings.Enabled = true;
            Assert.True(settings.Enabled);
        }

        [Fact]
        public void CanSet_MaxParallelizationProperty()
        {
            var settings = new BulkheadSettings { MaxParallelization = 5 };
            Assert.Equal(5, settings.MaxParallelization);

            settings.MaxParallelization = 15;
            Assert.Equal(15, settings.MaxParallelization);
        }

        [Fact]
        public void CanSet_MaxQueuingActionsProperty()
        {
            var settings = new BulkheadSettings { MaxQueuingActions = 8 };
            Assert.Equal(8, settings.MaxQueuingActions);

            settings.MaxQueuingActions = 25;
            Assert.Equal(25, settings.MaxQueuingActions);
        }
    }
}