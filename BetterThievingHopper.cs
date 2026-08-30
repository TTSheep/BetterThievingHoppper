
using BetterThievingHoppper.Patch;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace BetterThievingHoppper
{
    [ModInitializer(nameof(Initialize))]
    public static class BetterThievingHopper
    {
        public static void Initialize()
        {
            ThievingHopperPatch.Apply();

            Log.Info("BetterThievingHopper - 加载成功!");
        }
    }
}
