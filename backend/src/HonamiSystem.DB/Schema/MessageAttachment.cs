namespace Shiron.HonamiSystem.DB.Schema;

public class MessageAttachment : BaseEntity {
    public required Guid MessageID { get; set; }
    public required ChatMessage Message { get; set; }
    public Guid ChatID => Message.ChatID;
    public Chat Chat => Message.Chat;

    public Guid? FileHandleID { get; set; }
    public FileHandle? FileHandle { get; set; }
    public Guid? ImageHandleID { get; set; }
    public ImageHandle? ImageHandle { get; set; }
    public Guid? WidgetHandleID { get; set; }
    public WidgetHandle? WidgetHandle { get; set; }

    public bool IsFile => FileHandleID.HasValue;
    public bool IsImage => ImageHandleID.HasValue;
    public bool IsWidget => WidgetHandleID.HasValue;
    public IAttachable Handle => IsFile ? FileHandle! : IsImage ? ImageHandle! : WidgetHandle!;
}
