using ECommons.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TreasureMaps.UI;

namespace TreasureMaps;

public class Config : IEzConfig
{
    // Map Config
    public string mapSelected { get; set; } = "Default";
    public bool runInfinite { get; set; } = true;
    public int totalMaps { get; set; } = 1;
    public bool specificMap { get; set; } = false;
    public bool goToTreasure { get; set; } = false;
    public bool digMap { get; set; } = false;
    public bool openCoffer { get; set; } = false;
    public bool enterPortal { get; set; } = false;
    public bool doDungeon { get; set; } = false;

    // Rotation and AI Config
    public bool autoRotaion { get; set; } = false;
    public bool bossModRebornPlugin { get; set; } = false;

    // Repair Config
    public float repairSlider { get; set; } = 30f;
    public bool selfRepair { get; set; } = false;

    // General Config
    public bool acceptedDisclaimer { get; set; } = false;
    public bool isFirstLoad { get; set; } = true;
    public bool disableWarning { get; set; } = false;
    public bool DEBUG { get; set; } = false;

    public class BattleTalkPattern
    {
        public Regex pattern { get; set; }
        public bool showMessage { get; set; }

        public BattleTalkPattern(Regex pattern, bool showMessage)
        {
            this.pattern = pattern;
            this.showMessage = showMessage;
        }
    }

    public List<Regex> patterns { get; set; } = new();
    public List<BattleTalkPattern> battleTalkPatterns { get; set; } = new();

    public void Save()
    {
        EzConfig.Save();
    }
}
