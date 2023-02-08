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
        public static string GetByteArrayString(byte[] field)
        {
            string bytestr = "";
            for (var i = 0; i < field.Length; i++)
            {
                bytestr += field[i];
            }
            bytestr = bytestr[..^1];
            return bytestr + "]";
        }

        /// <summary>
        /// Apply param defs for list of BinderFiles
        /// </summary>
        public static void ApplyParamDefs(ConcurrentBag<PARAMDEF> paramdefs, ConcurrentBag<PARAMDEF> paramdefs_alt, List<BinderFile> fileList, ConcurrentDictionary<string, PARAM> paramList, List<string> changeList, ConcurrentBag<string> presentParamList, string oldNew)
        {
            ConcurrentBag<string> warningList = new();
            Parallel.ForEach(Partitioner.Create(fileList), file =>
            {
                PARAM? param = null;
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                string labelText = fileName;
                presentParamList.Add(labelText);
                try
                {
                    if (!file.Name.Contains(".param"))
                    {
                        // Not a param
                        return;
                    }

                    param = PARAM.Read(file.Bytes);
                    param = Util.ApplyDefWithWarnings(param, paramdefs, paramdefs_alt, warningList, oldNew);
                    if (param != null)
                        paramList.TryAdd(fileName, param);
                }
                catch (Exception e)
                {
                    warningList.Add($"Could not read param data from {labelText} in {oldNew} file. {e.GetType()}: {e.Message}");
                }
            });
            changeList.AddRange(warningList.OrderBy(e => e));
        }

        /// <summary>
        /// Apply param def for a single file path.
        /// </summary>
        public static void ApplyParamDefs(ConcurrentBag<PARAMDEF> paramdefs, ConcurrentBag<PARAMDEF> paramdefs_alt, string filePath, ConcurrentDictionary<string, PARAM> paramList, List<string> changeList, ConcurrentBag<string> presentParamList, string oldNew)
        {
            ConcurrentBag<string> warningList = new();
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string labelText = fileName;
            presentParamList.Add(labelText);
            try
            {
                var param = PARAM.Read(filePath);
                param = Util.ApplyDefWithWarnings(param, paramdefs, paramdefs_alt, warningList, oldNew);
                if (param != null)
                    paramList.TryAdd(param.ParamType, param);
            }
            catch (Exception e)
            {
                warningList.Add($"Could not read param data from {labelText} in {oldNew} file. {e.GetType()}: {e.Message}");
            }
            changeList.AddRange(warningList.OrderBy(e => e));
        }

  
        public static void ApplyRowNames(ConcurrentDictionary<string, string[]> rowNames, ConcurrentDictionary<string, PARAM> paramList)
        {
            Parallel.ForEach(Partitioner.Create(rowNames), file =>
            {
                if (paramList.TryGetValue(file.Key.Replace(".txt", ""), out PARAM param))
                {
                    foreach (string line in file.Value)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] split = line.Split(' ');
                        int id = int.Parse(split[0]);
                        string name = string.Join(' ', split[1..]);

                        PARAM.Row? row = param[id];
                        if (row != null)
                        {
                            row.Name = name;
                        }
                    }
                }
            });
        }

        public static PARAM? ApplyDef(PARAM param, PARAMDEF paramdef, ConcurrentBag<string> warningList, string oldNew)
        {
            try
            {
                param.ApplyParamdef(paramdef);
                return param;
            }
            catch(InvalidDataException e)
            {
                warningList.Add($"Could not apply ParamDef for {param.ParamType} in {oldNew} file. {e.Message}");
                return null;
            }
        }

        public static PARAM? ApplyDefWithWarnings(PARAM param, ConcurrentBag<PARAMDEF> paramdefs, ConcurrentBag<PARAMDEF> paramdefs_alt, ConcurrentBag<string> warningList, string oldNew)
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
                            return ApplyDef(param, paramdef, warningList, oldNew);
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
                            return ApplyDef(param, paramdef, warningList, oldNew);
                        }
                    }
                }
            }

            // Def could not be applied.

            if (!matchType && !matchDefVersion)
                warningList.Add($"Could not apply ParamDef for {param.ParamType} in {oldNew} file. Valid ParamDef could not be found.");
            else if (matchType && !matchDefVersion)
                warningList.Add($"Could not apply ParamDef for {param.ParamType} in {oldNew} file. Cannot find ParamDef version {param.ParamdefDataVersion}.");
            else if (matchType && matchDefVersion)
                warningList.Add($"Could not apply ParamDef for {param.ParamType} in {oldNew} file. Row sizes do not match. Param: {bestRowsize}, Def: {bestDefRowSize}.");
            else
                throw new Exception("Unhandled Apply ParamDef error.");

            return null;
        }

    }

}
