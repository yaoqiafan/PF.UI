using System.Windows.Media;
using PF.UI.Shared.Data.ColorManipulation;

namespace PF.UI.Shared.Data.MaterialColors;

/// <summary>
/// 颜色对：Color + 自动计算的前景色。
/// 使用对比度算法（ColorAssist.ContrastingForegroundColor）自动选择黑/白。
/// </summary>
public readonly struct ColorPair
{
    public ColorPair(Color color) : this(color, null) { }

    public ColorPair(Color color, Color? foregroundColor)
    {
        Color = color;
        ForegroundColor = foregroundColor;
    }

    public Color Color { get; }
    public Color? ForegroundColor { get; }

    /// <summary>
    /// 获取推荐前景色：若未指定则自动计算对比色（浅色背景→黑，深色背景→白）。
    /// </summary>
    public Color GetForegroundColor() => ForegroundColor ?? Color.ContrastingForegroundColor();

    public static implicit operator ColorPair(Color color) => new(color);
}
