using System.ComponentModel.DataAnnotations;

namespace Shiron.TheArchive.DB.Schema.Car;

public enum Condition {
    New,
    Used
}

public enum BodyType {
    Convertible,
    SUV,
    SmallCar,
    EstateCar,
    Saloon,
    SportsCar,
    Van,
    Other
}

public enum FuelType {
    Gasoline,
    Diesel,
    Electric,
    Ethanol,
    HybridDiesel,
    HybridGasoline,
    Hydrogen,
    LPG,
    NaturalGas,
    Other
}

public enum Transmission {
    Automatic,
    Manual
}

public enum ExteriorColor {
    Black,
    Beige,
    Grey,
    Brown,
    White,
    Orange,
    Blue,
    Yellow,
    Red,
    Green,
    Silver,
    Gold,
    Purple,
    Matte,
    Metallic
}

public enum InteriorColor {
    Beige,
    Black,
    Blue,
    Brown,
    Grey,
    Red,
    Other
}

public enum InteriorType {
    Alcantara,
    FullLeather,
    PartLeather,
    SyntheticLeather,
    Velour,
    Cloth,
    Other
}

public class Brand : BaseEntity {
    [MaxLength(127)] public required string Name { get; set; }

    public IList<Model> Models { get; set; } = [];
    public IList<Car> Cars { get; set; } = [];
}

public class Model : BaseEntity {
    [MaxLength(127)] public required string Name { get; set; }
    public required Brand Brand { get; set; }
    public required Guid BrandID { get; set; }

    public IList<Car> Cars { get; set; } = [];
}

public class Car : BaseEntity {
    public required Brand Brand { get; set; }
    public required Guid BrandID { get; set; }
    public required Model Model { get; set; }
    public required Guid ModelID { get; set; }

    [MaxLength(255)] public required string Variant { get; set; }
    [MaxLength(4095)] public string Description { get; set; } = string.Empty;
    public IList<Image> Images { get; set; } = [];
    public required int Seats { get; set; }
    public required int Doors { get; set; }
    public required int PriceEur { get; set; }
    public required DateOnly RegistrationDate { get; set; }
    public int? MileageKm { get; set; }
    public int? PowerKw { get; set; }
    public bool Damaged { get; set; }

    public required Condition Condition { get; set; }
    public BodyType? BodyType { get; set; }
    public FuelType? FuelType { get; set; }
    public Transmission? Transmission { get; set; }
    public ExteriorColor? Color { get; set; }
    public InteriorColor? InteriorColor { get; set; }
    public InteriorType? InteriorType { get; set; }
}
