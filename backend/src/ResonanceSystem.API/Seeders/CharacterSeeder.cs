using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;
using Attribute = Shiron.ResonanceSystem.DB.Schema.Attribute;

namespace Shiron.ResonanceSystem.API.Seeders;

public static class CharacterSeeder {
    public static void SeedCharacters(this IServiceProvider services) {
        using var context = services.GetRequiredService<ResSystemDbContext>();

        // Fusion
        context.Characters.Add(new Character("Aemeath") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Brant") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Changli") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Chixia") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S4
        });
        context.Characters.Add(new Character("Denia") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Encore") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Galbrena") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Lupa") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Mornye") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Mortefi") {
            Attribute = Attribute.Fusion,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S4
        });

        // Glacio
        context.Characters.Add(new Character("Baizhi") {
            Attribute = Attribute.Glacio,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S4
        });
        context.Characters.Add(new Character("Carlotta") {
            Attribute = Attribute.Glacio,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Hiyuki") {
            Attribute = Attribute.Glacio,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Lingyang") {
            Attribute = Attribute.Glacio,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Lucilla") {
            Attribute = Attribute.Glacio,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Sanhua") {
            Attribute = Attribute.Glacio,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S4
        });
        context.Characters.Add(new Character("Youhu") {
            Attribute = Attribute.Glacio,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S4
        });
        context.Characters.Add(new Character("Zhezhi") {
            Attribute = Attribute.Glacio,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });

        // Aero
        context.Characters.Add(new Character("Aalto") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S4
        });
        context.Characters.Add(new Character("Cartethyia") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Ciaccona") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Iuno") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Jianxin") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Jiyan") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Qiuyuan") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Rover-Aero") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Sigrika") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Yangyang") {
            Attribute = Attribute.Aero,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S4
        });

        // Electro
        context.Characters.Add(new Character("Augusta") {
            Attribute = Attribute.Electro,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Buling") {
            Attribute = Attribute.Electro,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S4
        });
        context.Characters.Add(new Character("Calcharo") {
            Attribute = Attribute.Electro,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Lumi") {
            Attribute = Attribute.Electro,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S4
        });
        context.Characters.Add(new Character("Rebecca") {
            Attribute = Attribute.Electro,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Xiangli Yao") {
            Attribute = Attribute.Electro,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Yinlin") {
            Attribute = Attribute.Electro,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Yuanwu") {
            Attribute = Attribute.Electro,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S4
        });

        // Spectro
        context.Characters.Add(new Character("Jinhsi") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Lucy") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Luuk Herssen") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Lynae") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Pistol,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Phoebe") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Rover-Spectro") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Shorekeeper") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Verina") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Zani") {
            Attribute = Attribute.Spectro,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S5
        });

        // Havoc
        context.Characters.Add(new Character("Camellya") {
            Attribute = Attribute.Havoc,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Cantarella") {
            Attribute = Attribute.Havoc,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Chisa") {
            Attribute = Attribute.Havoc,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Danjin") {
            Attribute = Attribute.Havoc,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S4
        });
        context.Characters.Add(new Character("Phrolova") {
            Attribute = Attribute.Havoc,
            WeaponType = WeaponType.Rectifier,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Roccia") {
            Attribute = Attribute.Havoc,
            WeaponType = WeaponType.Gauntlet,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Rover-Havoc") {
            Attribute = Attribute.Havoc,
            WeaponType = WeaponType.Sword,
            Rarity = Rarity.S5
        });
        context.Characters.Add(new Character("Taoqi") {
            Attribute = Attribute.Havoc,
            WeaponType = WeaponType.Broadblade,
            Rarity = Rarity.S4
        });

        context.SaveChanges();
    }
}
