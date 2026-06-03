using System.Windows.Media;

namespace PF.UI.Shared.Data.MaterialColors;

/// <summary>
/// 调色板接口：提供名称和 MaterialDesignColor → Color 的查找字典。
/// </summary>
public interface ISwatch
{
    string Name { get; }
    IDictionary<MaterialDesignColor, Color> Lookup { get; }
}
