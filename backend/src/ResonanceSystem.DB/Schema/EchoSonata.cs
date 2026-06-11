using System.IO.Hashing;
using System.Text;

namespace Shiron.ResonanceSystem.DB.Schema;

public class EchoSonata(string name) {
    public ulong Id { get; set; } = XxHash3.HashToUInt64(Encoding.UTF8.GetBytes(name));
    public string Name { get; set; } = name;
    public IList<Echo> Echoes { get; set; } = [];
}
