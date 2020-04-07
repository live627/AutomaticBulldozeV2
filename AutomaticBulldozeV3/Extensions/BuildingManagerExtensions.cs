using ColossalFramework;

namespace AutomaticBulldozeV3.Extensions
{
    internal static class BuildingManagerExtensions
    {
        private static uint lastTickIndex;
        private static bool disasterResponseBuildingExist;

        internal static bool DisasterResponseBuildingExist(this BuildingManager buildingManager)
        {
            if (lastTickIndex == Singleton<SimulationManager>.instance.mcurrentTickIndex)
                return disasterResponseBuildingExist;
            
            lastTickIndex = Singleton<SimulationManager>.instance.mcurrentTickIndex;
            disasterResponseBuildingExist = false;
            var serviceBuildings = buildingManager.GetServiceBuildings(ItemClass.Service.Disaster);
            if (serviceBuildings.m_buffer != null && serviceBuildings.m_size <= serviceBuildings.m_buffer.Length)
            {
                for (var index = 0; index < serviceBuildings.m_size; ++index)
                {
                    var serviceBuildingId = serviceBuildings.m_buffer[index];
                    if (serviceBuildingId == 0) continue;
                    if (buildingManager.m_buildings.m_buffer[serviceBuildingId].m_flags == Building.Flags.None) continue;
                    if (!(buildingManager.m_buildings.m_buffer[serviceBuildingId].Info.m_buildingAI is DisasterResponseBuildingAI)) continue;
                    _disasterResponseBuildingExist = true;
                    break;
                }
            }
            return disasterResponseBuildingExist;
        }
    }
}
