using UnityEngine.Scripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading.Tasks;
[Preserve]// ǿ�ƴ��
public class SkipSplashImage
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]// ��splash��ʾǰִ��
    private static void Run()
    {
        Task.Run(() =>
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        });
    }
}