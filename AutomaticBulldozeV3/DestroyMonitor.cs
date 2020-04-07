using AutomaticBulldozeV3.Extensions;
using AutomaticBulldozeV3.UI;
using ColossalFramework;
using ICities;

namespace AutomaticBulldozeV3
{
    public class DestroyMonitor : ThreadingExtensionBase
    {
        private readonly BuildingManager buildingManager;
        private readonly SimulationManager simulationManager;
        private readonly EconomyManager economyManager;
        private readonly CoverageManager coverageManager;
        private readonly AudioGroup nullAudioGroup;

        public DestroyMonitor()
        {
            buildingManager = Singleton<BuildingManager>.instance;
            simulationManager = Singleton<SimulationManager>.instance;
            economyManager = Singleton<EconomyManager>.instance;
            coverageManager = Singleton<CoverageManager>.instance;
            nullAudioGroup = new AudioGroup(0, new SavedFloat("NOTEXISTINGELEMENT", Settings.gameSettingsFile, 0, false));
        }

        private void DeleteBuildingImpl(ref ushort buildingId, ref Building building)
        {
            if (building.Info.m_buildingAI.CheckBulldozing(buildingId, ref building) != ToolBase.ToolErrors.None)
                return;
            var buildingRefundAmount = building.GetRefundAmount(ref buildingId);
            if (buildingRefundAmount != 0)
                economyManager.AddResource(EconomyManager.Resource.RefundAmount, buildingRefundAmount, building.Info.m_class);
            building.DispatchAutobulldozeEffect(ref buildingId, nullAudioGroup);
            buildingManager.ReleaseBuilding(buildingId);
            if (ItemClass.GetPublicServiceIndex(building.Info.m_class.m_service) != -1)
                coverageManager.CoverageUpdated(building.Info.m_class.m_service, building.Info.m_class.m_subService, building.Info.m_class.m_level);
        }
        
        public override void OnAfterSimulationTick()
        {
            for (ushort i = (ushort)(simulationManager.m_currentTickIndex % 1000); i < buildingManager.m_buildings.m_buffer.Length; i+=1000)
            {
                if (buildingManager.m_buildings.m_buffer[i].m_flags == Building.Flags.None)
                    continue;

                if ((UIAutoBulldozerPanel.DemolishAbandoned && (buildingManager.m_buildings.m_buffer[i].m_flags & Building.Flags.Abandoned) != Building.Flags.None)
                    || (UIAutoBulldozerPanel.DemolishBurned && !buildingManager.DisasterResponseBuildingExist() && (buildingManager.m_buildings.m_buffer[i].m_flags & Building.Flags.BurnedDown) != Building.Flags.None))
                {
                    DeleteBuildingImpl(ref i, ref buildingManager.m_buildings.m_buffer[i]);
                }
            }
        }
    }
}
