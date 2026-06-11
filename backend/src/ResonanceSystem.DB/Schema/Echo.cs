using System.IO.Hashing;
using System.Text;

namespace Shiron.ResonanceSystem.DB.Schema;

public class Echo(string name) {
    public ulong Id { get; set; } = XxHash3.HashToUInt64(Encoding.UTF8.GetBytes(name));
    public string Name { get; set; } = name;
    public required EchoCost Cost { get; set; }

    public IList<EchoSonata> Sonatas { get; set; } = [];
}
