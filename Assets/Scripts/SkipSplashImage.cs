using UnityEngine.Scripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading.Tasks;
[Preserve]// 强制打包
public class SkipSplashImage
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]// 在splash显示前执行
    private static void Run()
    {
        Task.Run(() =>
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        });
    }
}