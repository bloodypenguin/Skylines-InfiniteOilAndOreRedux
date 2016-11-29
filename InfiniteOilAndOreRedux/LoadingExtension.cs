using ICities;

namespace InfiniteOilAndOreRedux
{
    public class LoadingExtension : LoadingExtensionBase
    {

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
            {
                return;
            }
            NaturalResourceManagerDetour.Deploy();
        }

        public override void OnLevelUnloading()
        {
            NaturalResourceManagerDetour.Revert();
        }
    }
}