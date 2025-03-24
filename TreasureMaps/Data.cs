using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.ObjectModel;
using System.Numerics;
namespace TreasureMaps;

public static class Data
{
    public static string rotationSolverRepo = "https://raw.githubusercontent.com/FFXIV-CombatReborn/CombatRebornRepo/main/pluginmaster.json";
    public static string portalTargetName = "Teleportation Portal";
    public static string completedTreasureHuntToast = "You defeat all the enemies drawn by the trap!";
    public static string portalSpawnToast = "A portal has appeared.";
    public static string arcaneSphere = "Arcane Sphere";

    public static readonly List<string> TreasureHuntExamineCofferText = new List<string>
    {
        "Examine the treasure coffer!",
        "Untersuch die Schatztruhe!"
    };

    public static readonly List<string> CompleteTreasureLocationToast = new List<string>
    {
        "You defeat all the enemies drawn by the trap!",
        "Du hast alle Gegner besiegt, die an der Falle gelauert hatten!"
    };

    public static readonly Dictionary<uint, string> NormalTreasureDungeonIds = new Dictionary<uint, string>
    {
        { 558, "Aquapolis" },
        { 712, "Lost Canals Uznair" },
        { 725, "Lost Canals Thief Uznair" },
        { 879, "Dungeons Lyhe Ghiah" },
        { 1000, "Excitatron" },
        { 1209, "Cenote" }
    };

    public static readonly Dictionary<uint, string> ShiftingTreasureDungeonIds = new Dictionary<uint, string>
    {
        { 794, "Shifting Altars Uznair" },
        { 924, "Shifting Lyhe Ghiah" },
        { 1123, "Shifting Gymnasion" },
    };

    public static readonly Dictionary<uint, Vector3> InitialTreasureDungeonPosition = new Dictionary<uint, Vector3>
    {
        { 558,  new Vector3(0,0,0)},    // Need to find these locations or just always move forward after entering instance?
        { 712,  new Vector3(-.07466215f, 149.99988f, 391.83997f)},
        { 725,  new Vector3(-.07466215f, 149.99988f, 391.83997f)},
        { 879,  new Vector3(0,0,0)},
        { 1000, new Vector3(0,0,0)},
        { 1209, new Vector3(0,0,0)}
    };

    // Need to find more spots later
    public static readonly List<Vector3> ThiefPortalLocationsRubySea = new List<Vector3>
    {
        new Vector3(-125.95223f, -193.75761f, -155.86938f),
        new Vector3(634.3f, -103.104f, 440.39197f)
    };

    public static readonly List<Vector3> ThiefPortalLocationsLochs = new List<Vector3>
    {
        new Vector3(103.459f, -343.743f, 207.728f),
    };

    public static readonly Dictionary<uint, string> DecipheredTreasureMapIds = new Dictionary<uint, string>
    {
        { 2001087, "Leather Treasure Map" },
        { 2001088, "Goatskin Treasure Map" },
        { 2001089, "Toadskin Treasure Map" },
        { 2001090, "Boarskin Treasure Map" },
        { 2001091, "Peisteskin Treasure Map" },
        { 2001352, "Leather Buried Treasure Map" },
        { 2001762, "Archaeoskin Treasure Map" },
        { 2001763, "Wyvernskin Treasure Map" },
        { 2001764, "Dragonskin Treasure Map" },
        { 2001977, "Master Thief's Map" },
        { 2002209, "Gaganaskin Treasure Map" },
        { 2002210, "Gazelleskin Treasure Map" },
        { 2002236, "Veteran Thief's Map" },
        { 2002260, "Fabled Thief's Map" },
        { 2002386, "Fabled Thief's Map" },
        { 2002503, "Seemingly Special Treasure Map" },
        { 2002504, "Seemingly Special Thief's Map" },
        { 2002663, "Gliderskin Treasure Map" },
        { 2002664, "Zonureskin Treasure Map" },
        { 2002665, "Legendary Thief's Map" },
        { 2003075, "Ostensibly Special Treasure Map" },
        { 2003076, "Ostensibly Special Thief's Map" },
        { 2003245, "Saigaskin Treasure Map" },
        { 2003246, "Kumbhiraskin Treasure Map" },
        { 2003247, "Thrill-seeking Thief's Map" },
        { 2003455, "Potentially Special Treasure Map" },
        { 2003456, "Potentially Special Thief's Map" },
        { 2003457, "Ophiotauroskin Treasure Map" },
        { 2003458, "Ancient Thief's Map" },
        { 2003463, "Conceivably Special Treasure Map" },
        { 2003464, "Conceivably Special Thief's Map" },
        { 2003562, "Loboskin Treasure Map" },
        { 2003563, "Br'aaxskin Treasure Map" },
        { 2003564, "Enigmatic Thief's Map" }
    };

    public static readonly Dictionary<uint, string> TreasureMapIds = new Dictionary<uint, string>
    {
        { 6688, "Timeworn Leather Map" },
        { 6689, "Timeworn Goatskin Map" },
        { 6690, "Timeworn Toadskin Map" },
        { 6691, "Timeworn Boarskin Map" },
        { 6692, "Timeworn Peisteskin Map" },
        { 12241, "Timeworn Archaeoskin Map" },
        { 12242, "Timeworn Wyvernskin Map" },
        { 12243, "Timeworn Dragonskin Map" },
        { 17835, "Timeworn Gaganaskin Map" },
        { 17836, "Timeworn Gazelleskin Map" },
        { 19770, "Timeworn Thief's Map" },
        { 24794, "Seemingly Special Timeworn Map" },
        { 26744, "Timeworn Gliderskin Map" },
        { 26745, "Timeworn Zonureskin Map" },
        { 33328, "Ostensibly Special Timeworn Map" },
        { 36611, "Timeworn Saigaskin Map" },
        { 36612, "Timeworn Kumbhiraskin Map" },
        { 39591, "Timeworn Ophiotauroskin Map" },
        { 39593, "Potentially Special Timeworn Map" },
        { 39918, "Conceivably Special Timeworn Map" },
        { 43556, "Timeworn Loboskin Map" },
        { 43557, "Timeworn Br'aaxskin Map" }
    };
}