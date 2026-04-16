namespace Shiron.HonamiSystem.DB.Schema;

public interface IAttachable {
    Guid ID { get; set; }
    Guid MessageID { get; set; }
    ChatMessage Message { get; set; }
}
