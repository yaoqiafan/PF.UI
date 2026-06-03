using System.Windows.Media;

namespace PF.UI.Shared.Data.MaterialColors;

/// <summary>
/// 提供对全部 19 种 Material Design 推荐调色板的静态访问。
///
/// 用法示例：
///   Color red500 = SwatchHelper.Lookup[MaterialDesignColor.Red];
///   ISwatch blue = SwatchHelper.Swatches.First(s => s.Name == "Blue");
///   Color tealA700 = new TealSwatch().Lookup[MaterialDesignColor.TealA700];
/// </summary>
public static class SwatchHelper
{
    /// <summary>
    /// 全部 19 种 Material Design 推荐调色板。
    /// </summary>
    public static IEnumerable<ISwatch> Swatches { get; } = new ISwatch[]
    {
        new RedSwatch(),
        new PinkSwatch(),
        new PurpleSwatch(),
        new DeepPurpleSwatch(),
        new IndigoSwatch(),
        new BlueSwatch(),
        new LightBlueSwatch(),
        new CyanSwatch(),
        new TealSwatch(),
        new GreenSwatch(),
        new LightGreenSwatch(),
        new LimeSwatch(),
        new YellowSwatch(),
        new AmberSwatch(),
        new OrangeSwatch(),
        new DeepOrangeSwatch(),
        new BrownSwatch(),
        new GreySwatch(),
        new BlueGreySwatch(),
    };

    /// <summary>
    /// 所有 19 种调色板的汇总查找表（MaterialDesignColor → Color）。
    /// </summary>
    public static IDictionary<MaterialDesignColor, Color> Lookup { get; } =
        Swatches.SelectMany(s => s.Lookup).ToDictionary(kv => kv.Key, kv => kv.Value);
}
