using ICities;
using InfiniteOilAndOreRedux.Detours;
using InfiniteOilAndOreRedux.Redirection;

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
            AssemblyRedirector.Deploy();
        }

        public override void OnLevelUnloading()
        {
            AssemblyRedirector.Revert();
        }
    }
}