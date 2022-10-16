using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsFormats;

namespace BoreParamCompare
{
    public static class Util
    {
        public static PARAM? ApplyDefWithWarnings(PARAM param, List<PARAMDEF> paramdefs, List<PARAMDEF> paramdefs_alt, List<string> changeList, string oldnew)
        {
            bool matchType = false;
            bool matchDefVersion = false;
            int bestDefVersion = -420;
            long bestRowsize = -69;
            long bestDefRowSize = -999;

            foreach (PARAMDEF paramdef in paramdefs)
            {
                if (param.ParamType == paramdef.ParamType)
                {
                    matchType = true;
                    bestDefVersion = paramdef.DataVersion;
                    if (param.ParamdefDataVersion == paramdef.DataVersion)
                    {
                        matchDefVersion = true;
                        bestRowsize = param.DetectedSize;
                        bestDefRowSize = paramdef.GetRowSize();
                        if (param.DetectedSize == -1 || param.DetectedSize == bestDefRowSize)
                        {
                            param.ApplyParamdef(paramdef);
                            return param;
                        }
                    }
                }
            }

            foreach (PARAMDEF paramdef in paramdefs_alt)
            {
                if (param.ParamType == paramdef.ParamType)
                {
                    matchType = true;
                    bestDefVersion = paramdef.DataVersion;
                    if (param.ParamdefDataVersion == paramdef.DataVersion)
                    {
                        matchDefVersion = true;
                        bestRowsize = param.DetectedSize;
                        bestDefRowSize = paramdef.GetRowSize();
                        if (param.DetectedSize == -1 || param.DetectedSize == bestDefRowSize)
                        {
                            param.ApplyParamdef(paramdef);
                            return param;
                        }
                    }
                }
            }

            // Def could not be applied.

            if (!matchType && !matchDefVersion)
                changeList.Add($"Could not apply ParamDef for ({oldnew}) {param.ParamType}. Valid ParamDef could not be found.");
            else if (matchType && !matchDefVersion)
                changeList.Add($"Could not apply ParamDef for ({oldnew}) {param.ParamType}. Cannot find ParamDef version {param.ParamdefDataVersion}.");
            else if (matchType && matchDefVersion)
                changeList.Add($"Could not apply ParamDef for ({oldnew}) {param.ParamType}. Row sizes do not match. Param: {bestRowsize}, Def: {bestDefRowSize}.");
            else
                throw new Exception("Unhandled Apply ParamDef error.");

            return null;
        }

    }

}
