namespace Shiron.TheArchive.DB.Schema;

public interface ITaggable {
    IList<string> Tags { get; set; }
}

public interface IOwned {
    User? CreatedBy { get; set; }
    Guid? CreatedByID { get; set; }
}
