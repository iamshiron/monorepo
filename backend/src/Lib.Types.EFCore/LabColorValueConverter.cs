using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shiron.Lib.Types;

namespace Shiron.Lib.Types.EFCore;

public class LabColorValueConverter : ValueConverter<LabColor, string> {
    public LabColorValueConverter() : base(
        lab => JsonSerializer.Serialize(new LabColorData(lab), (JsonSerializerOptions?)null),
        json => ToLabColor(JsonSerializer.Deserialize<LabColorData>(json, (JsonSerializerOptions?)null)!)
    ) { }

    static LabColor ToLabColor(LabColorData data) {
        return new LabColor(data.L, data.A, data.B);
    }

    sealed class LabColorData {
        public double L { get; set; }
        public double A { get; set; }
        public double B { get; set; }

        public LabColorData(LabColor lab) {
            L = lab.L;
            A = lab.A;
            B = lab.B;
        }
    }
}
