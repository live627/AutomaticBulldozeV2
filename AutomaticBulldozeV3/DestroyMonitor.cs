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
        private bool disasterResponseBuildingExist;

        public DestroyMonitor()
        {
            buildingManager = Singleton<BuildingManager>.instance;
            simulationManager = Singleton<SimulationManager>.instance;
            economyManager = Singleton<EconomyManager>.instance;
            coverageManager = Singleton<CoverageManager>.instance;
            nullAudioGroup = new AudioGroup(0, new SavedFloat("NOTEXISTINGELEMENT", Settings.gameSettingsFile, 0, false));
        }

        public override void OnCreated(IThreading threading)
        {
            disasterResponseBuildingExist = HasDisasterResponseBuilding();
            buildingManager.EventBuildingCreated += OnBuildingCreated;
            buildingManager.EventBuildingReleased += OnBuildingReleased;
            base.OnCreated(threading);
        }

        private bool HasDisasterResponseBuilding()
        {
            var serviceBuildings = buildingManager.GetServiceBuildings(ItemClass.Service.Disaster);
            for (var index = 0; index < serviceBuildings.m_size; ++index)
            {
                Building building = buildingManager.m_buildings.m_buffer[serviceBuildings.m_buffer[index]];
                if (IsDisasterResponseBuilding(ref building))
                    return true;
            }
            return false;
        }

        public override void OnReleased()
        {
            buildingManager.EventBuildingCreated -= OnBuildingCreated;
            buildingManager.EventBuildingReleased -= OnBuildingReleased;
            base.OnReleased();
        }

        private static bool IsDisasterResponseBuilding(ref Building building) =>
            building.Info.m_buildingAI is DisasterResponseBuildingAI && (building.m_flags & Building.Flags.Completed) != Building.Flags.None;

        private void OnBuildingCreated(ushort id) => disasterResponseBuildingExist |= IsDisasterResponseBuilding(ref buildingManager.m_buildings.m_buffer[id]);

        private void OnBuildingReleased(ushort id) => disasterResponseBuildingExist = HasDisasterResponseBuilding();

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

        private bool CanDemolish(ref Building building) =>
            (UIAutoBulldozerPanel.DemolishAbandoned && (building.m_flags & Building.Flags.Abandoned) != Building.Flags.None)
               || (UIAutoBulldozerPanel.DemolishBurned && (building.m_flags & Building.Flags.BurnedDown) != Building.Flags.None);

        public override void OnAfterSimulationTick()
        {
            if (!simulationManager.SimulationPaused)
                for (ushort i = (ushort)(simulationManager.m_currentTickIndex % 1000); i < buildingManager.m_buildings.m_buffer.Length; i += 1000)
                    if (CanDemolish(ref buildingManager.m_buildings.m_buffer[i]))
                        DeleteBuildingImpl(ref i, ref buildingManager.m_buildings.m_buffer[i]);
        }
    }
}