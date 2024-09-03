using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Player;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.TimeDate;

namespace MageAFK.Management
{

    [System.Serializable]
    public class WaveSaveData
    {
        public GearData gearData;
        public TempInventoryData inventoryData;
        public SiegeStatistic statisticData;
        public TempSpellData[] spellData;
        public OrderDataCollection orderData;
        public StatData playerStatData;
        public StatData enemyStatData;
        public MindsetData mindsetData;
        public WaveData waveData;
        public LevelData levelData;
        public float maxHealth;
        public float ultimateCD = 0;
        public int silver;
        public WaveSaveData(bool saveTempData = false)
        {
            if (!saveTempData) return;
            gearData = ServiceLocator.Get<GearHandler>().SaveData();
            inventoryData = ServiceLocator.Get<InventoryHandler>().SaveTempData();
            silver = ServiceLocator.Get<CurrencyHandler>().GetCurrencyAmount(CurrencyType.SilverCoins);
            statisticData = ServiceLocator.Get<SiegeStatisticTracker>().SaveData();
            orderData = ServiceLocator.Get<CraftingHandler>().SaveTempData();
            playerStatData = ServiceLocator.Get<PlayerStatHandler>().SaveTempData();
            enemyStatData = ServiceLocator.Get<EnemyStatHandler>().SaveTempData();
            levelData = ServiceLocator.Get<LevelHandler>().SaveData();
            // Assuming SpellHandler's SaveData method should save its result to a variable like the others
            spellData = ServiceLocator.Get<SpellHandler>().SaveTempData();
            waveData = ServiceLocator.Get<WaveHandler>().SaveData();
            mindsetData = ServiceLocator.Get<MindsetHandler>().SaveData();
            ultimateCD = ServiceLocator.Get<TimeTaskHandler>().ReturnTimeLeft(Ultimate.ultTimeKey);
            maxHealth = ServiceLocator.Get<PlayerHealth>().MaxHealth;

        }
    }

    public class WaveSaveHandler : IData<WaveSaveData>
    {
        public WaveSaveData waveSave;

        public void InitializeData(WaveSaveData data)
        {
            waveSave = data;
            ServiceLocator.RegisterService(this);
            return;
        }

        public void LoadData()
        {
            ServiceLocator.Get<GearHandler>().InitializeData(waveSave.gearData);
            ServiceLocator.Get<InventoryHandler>().InitializeTempData(waveSave.inventoryData);
            ServiceLocator.Get<CurrencyHandler>().SetCurrencyValue(CurrencyType.SilverCoins, waveSave.silver);
            ServiceLocator.Get<SiegeStatisticTracker>().InitializeData(waveSave.statisticData);
            ServiceLocator.Get<ITempData<TempSpellData[]>>().InitializeTempData(waveSave.spellData);
            ServiceLocator.Get<CraftingHandler>().InitializeTempData(waveSave.orderData);
            ServiceLocator.Get<PlayerStatHandler>().LoadTempValues(waveSave.playerStatData.stats);
            ServiceLocator.Get<EnemyStatHandler>().LoadTempValues(waveSave.enemyStatData.stats);
            ServiceLocator.Get<LevelHandler>().InitializeData(waveSave.levelData);
            ServiceLocator.Get<MindsetHandler>().InitializeData(waveSave.mindsetData);
            ServiceLocator.Get<WaveHandler>().InitializeData(waveSave.waveData);

            //Ult timer
            if (waveSave.ultimateCD > 0) ServiceLocator.Get<SpellCastHandler>().HandleUltTimer(waveSave.ultimateCD);
        }


        public WaveSaveData SaveData()
        {
            return new WaveSaveData(true);
        }
    }


}
