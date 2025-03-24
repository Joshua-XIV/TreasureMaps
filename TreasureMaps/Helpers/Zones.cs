using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace TreasureMaps.Helpers;

public unsafe static class Zones
{
    public static bool IsInZone(uint zoneID) => Svc.ClientState.TerritoryType == zoneID;

    public static string GetZoneName(uint zoneID) => GetRow<TerritoryType>(zoneID)!.Value.PlaceName.Value.Name.ToString();

    public static uint CurrentZoneId() => Svc.ClientState.TerritoryType;

    public static bool IsInNormalTreasureDungeon() => NormalTreasureDungeonIds.ContainsKey(CurrentZoneId());

    public static bool IsInShiftingTreasureDungeon() => ShiftingTreasureDungeonIds.ContainsKey(CurrentZoneId());

    public static uint FlagZoneID() => AgentMap.Instance()->FlagMapMarker.TerritoryId;

    public static float FlagXCoords() => AgentMap.Instance()->FlagMapMarker.XFloat;

    public static float FlagYCoords() => AgentMap.Instance()->FlagMapMarker.YFloat;

    public static float FlagScale() => AgentMap.Instance()->SelectedMapSizeFactorFloat;

    /// <summary>
    /// Finds the closest aetheryte to the currently set map flag.
    /// </summary>
    /// <returns>The ID of the closest aetheryte.</returns>
    public static uint GetClosestAetheryte()
    {
        var aetherytes = Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Aetheryte>();
        uint closestId = 0;
        double distance = 0;
        foreach (var data in aetherytes)
        {
            if (!data.IsAetheryte) continue;
            if (data.Territory.ValueNullable == null) continue;
            if (data.PlaceName.ValueNullable == null) continue;
            if (data.Territory.Value.RowId == FlagZoneID())
            {
                var scale = data.Map.Value.SizeFactor;
                var aetherX = ConvertMapMarkerToMapCoordinate(data.AetherstreamX, scale );
                var aetherY = ConvertMapMarkerToMapCoordinate(data.AetherstreamY, scale );
                var tempDistance = Math.Pow((FlagXCoords() - aetherX), 2) + Math.Pow((FlagYCoords() - aetherY), 2);
                Generic.PluginLogInfo($"Distance {tempDistance}, ID {data.RowId}. AetherX: {data.AetherstreamX}. AetherY: {data.AetherstreamY}, NewX: {aetherX}, NewY: {aetherY}");
                if (distance == 0 || tempDistance < distance)
                {
                    distance = tempDistance;
                    closestId = data.RowId;
                }
            }
        }
        
        return closestId;
    }

    /// <summary>
    /// Converts a map marker position to a map coordinate.
    /// </summary>
    /// <param name="pos">The position of the map marker.</param>
    /// <param name="scale">The scale factor of the map.</param>
    /// <returns>The converted map coordinate.</returns>
    private static float ConvertMapMarkerToMapCoordinate(int pos, float scale)
    {
        var num = scale / 100f;
        var rawPosition = (int)((float)(pos - 1024.0) / num * 1000f);
        return ConvertRawPositionToMapCoordinate(rawPosition, scale);
    }

    /// <summary>
    /// Converts a raw position to a map coordinate.
    /// </summary>
    /// <param name="pos">The raw position.</param>
    /// <param name="scale">The scale factor of the map.</param>
    /// <returns>The converted map coordinate.</returns>
    private static float ConvertRawPositionToMapCoordinate(int pos, float scale)
    {
        var num = scale / 100f;
        return (float)((((pos / 1000f * num) + 1024.0) / 2048.0 * 41.0 / num) + 1.0);
    }
}