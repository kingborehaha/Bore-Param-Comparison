using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsFormats;

namespace BoreParamCompare
{
    public static class Util
    {
        /// <summary>
        /// Apply param defs for list of BinderFiles
        /// </summary>
        public static void ApplyParamDefs(List<PARAMDEF> paramdefs, List<PARAMDEF> paramdefs_alt, List<BinderFile> fileList, ConcurrentDictionary<string, PARAM> paramList, List<string> changeList, List<string> presentParamList, string oldNew)
        {
            Parallel.ForEach(Partitioner.Create(fileList), file =>
            {
                if (file.Name.Contains(".param") == false)
                    return; //not a param.
                string name = Path.GetFileNameWithoutExtension(file.Name);
                var param = PARAM.Read(file.Bytes);
                try
                {
                    presentParamList.Add(param.ParamType);
                    param = Util.ApplyDefWithWarnings(param, paramdefs, paramdefs_alt, changeList, oldNew);
                    if (param != null)
                        paramList.TryAdd(name, param);
                }
                catch (InvalidDataException)
                {
                    string labelText;
                    if (param != null)
                        labelText = param.ParamType;
                    else
                        labelText = file.Name;
                    changeList.Add($"InvalidDataException: Could not apply ParamDef for {labelText} in {oldNew} file. If correct game was selected, Param is incompatible with current ParamDef");
                }
            });
        }

        /// <summary>
        /// Apply param def for a single file path.
        /// </summary>
        public static void ApplyParamDefs(List<PARAMDEF> paramdefs, List<PARAMDEF> paramdefs_alt, string filePath, ConcurrentDictionary<string, PARAM> paramList, List<string> changeList, List<string> presentParamList, string oldNew)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            try
            {
                var param = PARAM.Read(filePath);
                presentParamList.Add(param.ParamType);
                param = Util.ApplyDefWithWarnings(param, paramdefs, paramdefs_alt, changeList, oldNew);
                if (param != null)
                    paramList.TryAdd(param.ParamType, param);
            }
            catch (InvalidDataException)
            {
                string labelText = fileName;
                changeList.Add($"InvalidDataException: Could not apply ParamDef for {labelText} in {oldNew} file. If correct game was selected, Param is incompatible with current ParamDef");
            }
        }

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
