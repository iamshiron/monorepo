namespace Shiron.HonamiGit.DB.Schema;

public interface ITaggable {
    List<string> Tags { get; set; }
}

public interface IOwned {
    User? CreatedBy { get; set; }
    Guid? CreatedByID { get; set; }
}
