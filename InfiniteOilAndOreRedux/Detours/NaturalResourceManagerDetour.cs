using System.Reflection;
using ColossalFramework;
using InfiniteOilAndOreRedux.Redirection;
using InfiniteOilAndOreRedux.Redirection.Attributes;
using UnityEngine;

namespace InfiniteOilAndOreRedux.Detours
{
    [TargetType(typeof(NaturalResourceManager))]
    public class NaturalResourceManagerDetour : NaturalResourceManager
    {
        [RedirectMethod]
        private int CountResource(NaturalResourceManager.Resource resource, Vector3 position, float radius,
            int cellDelta, out int numCells, out int totalCells, out int resultDelta, bool refresh)
        {
            float num1 = 33.75f;
            int num2 = 512;
            radius += num1*0.51f;
            int minX = Mathf.Max((int) (((double) position.x - (double) radius)/(double) num1 + (double) num2*0.5), 0);
            int minZ = Mathf.Max((int) (((double) position.z - (double) radius)/(double) num1 + (double) num2*0.5), 0);
            int maxX = Mathf.Min((int) (((double) position.x + (double) radius)/(double) num1 + (double) num2*0.5),
                num2 - 1);
            int maxZ = Mathf.Min((int) (((double) position.z + (double) radius)/(double) num1 + (double) num2*0.5),
                num2 - 1);
            int num3 = 0;
            numCells = 0;
            totalCells = 0;
            resultDelta = 0;
            int deltaBuffer = Singleton<SimulationManager>.instance.m_randomizer.Bits32(20);
            if (cellDelta < 0)
                deltaBuffer = -deltaBuffer;
            byte num4 = 0;
            if (cellDelta != 0)
                num4 = resource < NaturalResourceManager.Resource.Pollution ? (byte) 1 : (byte) 2;
            for (int index1 = minZ; index1 <= maxZ; ++index1)
            {
                float num5 = (float) ((double) index1 - (double) num2*0.5 + 0.5)*num1 - position.z;
                for (int index2 = minX; index2 <= maxX; ++index2)
                {
                    float num6 = (float) ((double) index2 - (double) num2*0.5 + 0.5)*num1 - position.x;
                    if ((double) num5*(double) num5 + (double) num6*(double) num6 < (double) radius*(double) radius ||
                        (double) radius == 0.0)
                    {
                        NaturalResourceManager.ResourceCell resourceCell = this.m_naturalResources[index1*num2 + index2];
                        totalCells = totalCells + 1;
                        int num7 = 0;
                        switch (resource)
                        {
                            case NaturalResourceManager.Resource.Ore:
                                //begin mod
                                num7 = CountAndKeep(ref resourceCell.m_ore, ref deltaBuffer,
                                    ref resultDelta, cellDelta);
                                //end mod
                                break;
                            case NaturalResourceManager.Resource.Sand:
                                num7 = CountAndModify(ref resourceCell.m_sand, ref deltaBuffer,
                                    ref resultDelta, cellDelta);
                                break;
                            case NaturalResourceManager.Resource.Oil:
                                //begin mod
                                num7 = CountAndKeep(ref resourceCell.m_oil, ref deltaBuffer,
                                    ref resultDelta, cellDelta);
                                //end mod
                                break;
                            case NaturalResourceManager.Resource.Fertility:
                                num7 = CountAndKeep(ref resourceCell.m_fertility, ref deltaBuffer,
                                    ref resultDelta, cellDelta);
                                break;
                            case NaturalResourceManager.Resource.Forest:
                                num7 = CountAndKeep(ref resourceCell.m_forest, ref deltaBuffer,
                                    ref resultDelta, cellDelta);
                                break;
                            case NaturalResourceManager.Resource.Pollution:
                                num7 = CountAndModify(ref resourceCell.m_pollution,
                                    ref deltaBuffer, ref resultDelta, cellDelta);
                                break;
                            case NaturalResourceManager.Resource.Burned:
                                num7 = CountAndModify(ref resourceCell.m_burned, ref deltaBuffer,
                                    ref resultDelta, cellDelta);
                                break;
                            case NaturalResourceManager.Resource.Destroyed:
                                num7 = CountAndModify(ref resourceCell.m_destroyed,
                                    ref deltaBuffer, ref resultDelta, cellDelta);
                                break;
                        }
                        if (num7 != 0)
                        {
                            numCells = numCells + 1;
                            num3 += num7;
                        }
                        if (cellDelta != 0)
                        {
                            if (!refresh)
                                resourceCell.m_modified |= num4;
                            this.m_naturalResources[index1*num2 + index2] = resourceCell;
                        }
                    }
                }
            }
            if (refresh && cellDelta != 0)
            {
                if (resource >= NaturalResourceManager.Resource.Pollution)
                    this.AreaModifiedB(minX, minZ, maxX, maxZ);
                else
                    this.AreaModified(minX, minZ, maxX, maxZ);
            }
            return num3;
        }

        [RedirectReverse]
        private static int CountAndKeep(ref byte data, ref int deltaBuffer, ref int resultDelta, int cellDelta)
        {
            UnityEngine.Debug.Log("InifiniteOilAndOreRedux - failed to detour CountAndKeep()");
            return 0;
        }

        [RedirectReverse]
        private static int CountAndModify(ref byte data, ref int deltaBuffer, ref int resultDelta, int cellDelta)
        {
            UnityEngine.Debug.Log("InifiniteOilAndOreRedux - failed to detour CountAndModify()");
            return 0;
        }
    }
}