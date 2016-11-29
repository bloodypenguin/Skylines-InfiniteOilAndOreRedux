using System.Reflection;
using ColossalFramework;
using InfiniteOilAndOreRedux.Redirection;
using UnityEngine;

namespace InfiniteOilAndOreRedux
{
    [TargetType(typeof(NaturalResourceManager))]
    public class NaturalResourceManagerDetour : NaturalResourceManager
    {
        private static RedirectCallsState _state;
        private static MethodInfo _originalInfo;
        private static MethodInfo _detourInfo = typeof(NaturalResourceManagerDetour).GetMethod("CountResource", BindingFlags.NonPublic | BindingFlags.Instance);
        private static bool _deployed;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            var tuple = RedirectionUtil.RedirectMethod(typeof(NaturalResourceManager), _detourInfo);
            _originalInfo = tuple.First;
            _state = tuple.Second;
            _deployed = true;
        }

        public static void Revert()
        {
            if (!_deployed) return;
            if (_originalInfo != null && _detourInfo != null)
            {
                RedirectionHelper.RevertRedirect(_originalInfo, _state);
            }
            _deployed = false;
        }



        [RedirectMethod]
        private int CountResource(NaturalResourceManager.Resource resource, Vector3 position, float radius, int cellDelta, out int numCells, out int totalCells, out int resultDelta)
        {
            float num1 = 33.75f;
            int num2 = 512;
            radius += num1 * 0.51f;
            int num3 = Mathf.Max((int)(((double)position.x - (double)radius) / (double)num1 + (double)num2 * 0.5), 0);
            int num4 = Mathf.Max((int)(((double)position.z - (double)radius) / (double)num1 + (double)num2 * 0.5), 0);
            int num5 = Mathf.Min((int)(((double)position.x + (double)radius) / (double)num1 + (double)num2 * 0.5), num2 - 1);
            int num6 = Mathf.Min((int)(((double)position.z + (double)radius) / (double)num1 + (double)num2 * 0.5), num2 - 1);
            int num7 = 0;
            numCells = 0;
            totalCells = 0;
            resultDelta = 0;
            int deltaBuffer = Singleton<SimulationManager>.instance.m_randomizer.Bits32(20);
            if (cellDelta < 0)
                deltaBuffer = -deltaBuffer;
            for (int index1 = num4; index1 <= num6; ++index1)
            {
                float num8 = (float)((double)index1 - (double)num2 * 0.5 + 0.5) * num1 - position.z;
                for (int index2 = num3; index2 <= num5; ++index2)
                {
                    float num9 = (float)((double)index2 - (double)num2 * 0.5 + 0.5) * num1 - position.x;
                    if ((double)num8 * (double)num8 + (double)num9 * (double)num9 < (double)radius * (double)radius || (double)radius == 0.0)
                    {
                        NaturalResourceManager.ResourceCell resourceCell = NaturalResourceManager.instance.m_naturalResources[index1 * num2 + index2];
                        totalCells = totalCells + 1;
                        int num10 = 0;
                        switch (resource)
                        {
                            case NaturalResourceManager.Resource.Ore:
                                //begin mod
                                num10 = CountAndKeep(ref resourceCell.m_ore, ref deltaBuffer, ref resultDelta, cellDelta);
                                //end mod
                                break;
                            case NaturalResourceManager.Resource.Sand:
                                num10 = CountAndModify(ref resourceCell.m_sand, ref deltaBuffer, ref resultDelta, cellDelta);
                                break;
                            case NaturalResourceManager.Resource.Oil:
                                //begin mod
                                num10 = CountAndKeep(ref resourceCell.m_oil, ref deltaBuffer, ref resultDelta, cellDelta);
                                //end mod
                                break;
                            case NaturalResourceManager.Resource.Fertility:
                                num10 = CountAndKeep(ref resourceCell.m_fertility, ref deltaBuffer, ref resultDelta, cellDelta);
                                break;
                            case NaturalResourceManager.Resource.Forest:
                                num10 = CountAndKeep(ref resourceCell.m_forest, ref deltaBuffer, ref resultDelta, cellDelta);
                                break;
                            case NaturalResourceManager.Resource.Pollution:
                                num10 = CountAndModify(ref resourceCell.m_pollution, ref deltaBuffer, ref resultDelta, cellDelta);
                                break;
                        }
                        if (num10 != 0)
                        {
                            numCells = numCells + 1;
                            num7 += num10;
                        }
                        if (cellDelta != 0)
                        {
                            resourceCell.m_modified = true;
                            this.m_naturalResources[index1 * num2 + index2] = resourceCell;
                        }
                    }
                }
            }
            return num7;
        }

        //no changes
        private static int CountAndModify(ref byte data, ref int deltaBuffer, ref int resultDelta, int cellDelta)
        {
            if ((int)data != 0 || cellDelta > 0)
                deltaBuffer = deltaBuffer + cellDelta;
            if (deltaBuffer >= 1048576)
            {
                int num = Mathf.Min((int)byte.MaxValue - (int)data, deltaBuffer >> 20);
                data = (byte)((uint)data + (uint)(byte)num);
                deltaBuffer = deltaBuffer - (num << 20);
                resultDelta = resultDelta + num;
            }
            else if (deltaBuffer <= -1048576)
            {
                int num = Mathf.Min((int)data, -deltaBuffer >> 20);
                data = (byte)((uint)data - (uint)(byte)num);
                deltaBuffer = deltaBuffer + (num << 20);
                resultDelta = resultDelta - num;
            }
            return (int)data;
        }

        //no changes
        private static int CountAndKeep(ref byte data, ref int deltaBuffer, ref int resultDelta, int cellDelta)
        {
            if ((int)data != 0 || cellDelta > 0)
                deltaBuffer = deltaBuffer + cellDelta;
            if (deltaBuffer >= 1048576)
            {
                int num = Mathf.Min((int)byte.MaxValue - (int)data, deltaBuffer >> 20);
                deltaBuffer = deltaBuffer - (num << 20);
                resultDelta = resultDelta + num;
            }
            else if (deltaBuffer <= -1048576)
            {
                int num = Mathf.Min((int)data, -deltaBuffer >> 20);
                deltaBuffer = deltaBuffer + (num << 20);
                resultDelta = resultDelta - num;
            }
            return (int)data;
        }
    }
}