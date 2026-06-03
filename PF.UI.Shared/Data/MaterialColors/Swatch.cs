namespace PF.UI.Shared.Data.MaterialColors;

/// <summary>
/// 定义一个完整的 Material Design 调色板，包含主要色调（PrimaryHues）和次要色调（SecondaryHues）。
/// ExemplarHue 默认为 PrimaryHues[5]（即 500 层级），SecondaryExemplarHue 默认为 SecondaryHues[2]（即 A200 层级）。
/// </summary>
public class Swatch
{
    public Swatch(string name, IEnumerable<Hue> primaryHues, IEnumerable<Hue> secondaryHues)
    {
        if (name is null) throw new ArgumentNullException(nameof(name));
        if (primaryHues is null) throw new ArgumentNullException(nameof(primaryHues));
        if (secondaryHues is null) throw new ArgumentNullException(nameof(secondaryHues));

        var primaryList = primaryHues.ToList();
        if (primaryList.Count == 0) throw new ArgumentException("Primary hues must not be empty.", nameof(primaryHues));

        Name = name;
        PrimaryHues = primaryList;
        SecondaryHues = secondaryHues.ToList();
        ExemplarHue = primaryList[Math.Min(5, primaryList.Count - 1)];
    }

    public string Name { get; }
    public Hue ExemplarHue { get; }
    public IList<Hue> PrimaryHues { get; }
    public IList<Hue> SecondaryHues { get; }

    public override string ToString() => Name;
}
