using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shiron.Lib.Types;

namespace Shiron.Lib.Types.EFCore;

public static class PropertyBuilderExtensions {
    public static PropertyBuilder<Color32> IsColor32(this PropertyBuilder<Color32> builder) {
        builder.HasConversion(
            new Color32ValueConverter()
        );
        return builder;
    }

    public static PropertyBuilder<LabColor> IsLabColor(this PropertyBuilder<LabColor> builder) {
        builder.HasConversion(
            new LabColorValueConverter()
        );
        return builder;
    }
}
