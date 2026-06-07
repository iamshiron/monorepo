using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;
using Attribute = Shiron.ResonanceSystem.DB.Schema.Attribute;

namespace Shiron.ResonanceSystem.API.Seeders;

public static class CharacterSeeder {
    public static void SeedCharacters(this IServiceProvider services) {
        using var context = services.GetRequiredService<ResSystemDbContext>();

        var charactersToSeed = new List<Character> {
            // Fusion
            new("Aemeath") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Brant") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Changli") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Chixia") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Pistol, Rarity = Rarity.S4 },
            new("Denia") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Encore") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Galbrena") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Pistol, Rarity = Rarity.S5 },
            new("Lupa") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S5 },
            new("Mornye") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S5 },
            new("Mortefi") { Attribute = Attribute.Fusion, WeaponType = WeaponType.Pistol, Rarity = Rarity.S4 },

            // Glacio
            new("Baizhi") { Attribute = Attribute.Glacio, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S4 },
            new("Carlotta") { Attribute = Attribute.Glacio, WeaponType = WeaponType.Pistol, Rarity = Rarity.S5 },
            new("Hiyuki") { Attribute = Attribute.Glacio, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Lingyang") { Attribute = Attribute.Glacio, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S5 },
            new("Lucilla") { Attribute = Attribute.Glacio, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Sanhua") { Attribute = Attribute.Glacio, WeaponType = WeaponType.Sword, Rarity = Rarity.S4 },
            new("Youhu") { Attribute = Attribute.Glacio, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S4 },
            new("Zhezhi") { Attribute = Attribute.Glacio, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },

            // Aero
            new("Aalto") { Attribute = Attribute.Aero, WeaponType = WeaponType.Pistol, Rarity = Rarity.S4 },
            new("Cartethyia") { Attribute = Attribute.Aero, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Ciaccona") { Attribute = Attribute.Aero, WeaponType = WeaponType.Pistol, Rarity = Rarity.S5 },
            new("Iuno") { Attribute = Attribute.Aero, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S5 },
            new("Jianxin") { Attribute = Attribute.Aero, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S5 },
            new("Jiyan") { Attribute = Attribute.Aero, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S5 },
            new("Qiuyuan") { Attribute = Attribute.Aero, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Rover-Aero") { Attribute = Attribute.Aero, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Sigrika") { Attribute = Attribute.Aero, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S5 },
            new("Yangyang") { Attribute = Attribute.Aero, WeaponType = WeaponType.Sword, Rarity = Rarity.S4 },

            // Electro
            new("Augusta") { Attribute = Attribute.Electro, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S5 },
            new("Buling") { Attribute = Attribute.Electro, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S4 },
            new("Calcharo") { Attribute = Attribute.Electro, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S5 },
            new("Lumi") { Attribute = Attribute.Electro, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S4 },
            new("Rebecca") { Attribute = Attribute.Electro, WeaponType = WeaponType.Pistol, Rarity = Rarity.S5 },
            new("Xiangli Yao") { Attribute = Attribute.Electro, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S5 },
            new("Yinlin") { Attribute = Attribute.Electro, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Yuanwu") { Attribute = Attribute.Electro, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S4 },

            // Spectro
            new("Jinhsi") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S5 },
            new("Lucy") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Pistol, Rarity = Rarity.S5 },
            new("Luuk Herssen") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S5 },
            new("Lynae") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Pistol, Rarity = Rarity.S5 },
            new("Phoebe") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Rover-Spectro") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Shorekeeper") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Verina") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Zani") { Attribute = Attribute.Spectro, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S5 },

            // Havoc
            new("Camellya") { Attribute = Attribute.Havoc, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Cantarella") { Attribute = Attribute.Havoc, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Chisa") { Attribute = Attribute.Havoc, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S5 },
            new("Danjin") { Attribute = Attribute.Havoc, WeaponType = WeaponType.Sword, Rarity = Rarity.S4 },
            new("Phrolova") { Attribute = Attribute.Havoc, WeaponType = WeaponType.Rectifier, Rarity = Rarity.S5 },
            new("Roccia") { Attribute = Attribute.Havoc, WeaponType = WeaponType.Gauntlet, Rarity = Rarity.S5 },
            new("Rover-Havoc") { Attribute = Attribute.Havoc, WeaponType = WeaponType.Sword, Rarity = Rarity.S5 },
            new("Taoqi") { Attribute = Attribute.Havoc, WeaponType = WeaponType.Broadblade, Rarity = Rarity.S4 }
        };

        var existingCharacterNames = context.Characters
            .Select(c => c.Name)
            .ToHashSet();

        var charactersToAdd = charactersToSeed
            .Where(c => !existingCharacterNames.Contains(c.Name))
            .ToList();

        if (charactersToAdd.Count > 0) {
            context.Characters.AddRange(charactersToAdd);
            context.SaveChanges();
        }

        context.SaveChanges();
    }
}
