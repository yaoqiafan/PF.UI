using System.Windows.Media;

namespace PF.UI.Shared.Data.MaterialColors;

/// <summary>
/// 单个颜色色调，包含名称、RGB 色值和对应的推荐前景色。
/// </summary>
public class Hue
{
    public Hue(string name, Color color, Color foreground)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Color = color;
        Foreground = foreground;
    }

    public string Name { get; }
    public Color Color { get; }
    public Color Foreground { get; }

    public override string ToString() => Name;
}
