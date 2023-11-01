using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static void ApplyTentativeParamType(string paramFileName, PARAM param, Dictionary<string, string> tentativeParamTypes)
        {
            if (tentativeParamTypes.TryGetValue(paramFileName, out string? tentativeType))
            {
                param.ParamType = tentativeType;
            }
        }

        /// <summary>
        /// Used to determine param types for params with missing or garbaled paramTypes.
        /// </summary>
        public static Dictionary<string, string> GetTentativeParamTypes(string gameType)
        {
            Dictionary<string, string> dict = new();

            var filePath = $@"{Directory.GetCurrentDirectory()}\Paramdex\{gameType}\Defs\TentativeParamType.csv";
            if (File.Exists(filePath))
            {
                // Code copied from thefifthmatt
                foreach (string line in File.ReadAllLines(filePath).Skip(1))
                {
                    string[] split = line.Split(',');
                    if (split.Length != 2 || string.IsNullOrWhiteSpace(split[0]) || string.IsNullOrWhiteSpace(split[1]))
                    {
                        throw new FormatException($"Malformed line in {filePath}: {line}");
                    }
                    dict[split[0]] = split[1];
                }
            }

            return dict;
        }

        /// <summary>
        /// Apply param defs for list of BinderFiles
        /// </summary>
        public static void ApplyParamDefs(ConcurrentBag<PARAMDEF> paramdefs, ConcurrentBag<PARAMDEF> paramdefs_alt, List<BinderFile> fileList, ConcurrentDictionary<string, PARAM> paramList, List<string> changeList, ConcurrentBag<string> presentParamList, string oldNew, Dictionary<string, string> tentativeParamTypes)
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

                    ApplyTentativeParamType(fileName, param, tentativeParamTypes);

                    param = Util.ApplyDefWithWarnings(param, paramdefs, paramdefs_alt, warningList, oldNew, fileName);
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
                param = Util.ApplyDefWithWarnings(param, paramdefs, paramdefs_alt, warningList, oldNew, fileName);
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

        public static PARAM? ApplyDef(PARAM param, PARAMDEF paramdef)
        {
            param.ApplyParamdef(paramdef);
            return param;
        }

        public static PARAM? ApplyDefWithWarnings(PARAM param, ConcurrentBag<PARAMDEF> paramdefs, ConcurrentBag<PARAMDEF> paramdefs_alt, ConcurrentBag<string> warningList, string oldNew, string paramName)
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
                            try
                            {
                                return ApplyDef(param, paramdef);
                            }
                            catch (Exception e)
                            {
                                // ApplyDef failed. Check if Paramdex ALT contains this param before giving up.
                                if (paramdefs_alt.FirstOrDefault(f => f.ParamType == param.ParamType) == null)
                                {
                                    warningList.Add($"Could not apply ParamDef for {param.ParamType} in {oldNew} file. {e.Message}");
                                    return null;
                                }
                            }

                            // ApplyDef failed and Paramdex ALT contains this paramdef.
                            break;
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
                            try
                            {
                                return ApplyDef(param, paramdef);
                            }
                            catch(Exception e)
                            {
                                warningList.Add($"Could not apply ParamDef for {param.ParamType} in {oldNew} file. {e.Message}");
                                return null;
                            }

                        }
                    }
                }
            }

            // Def could not be applied.

            if (!matchType && !matchDefVersion)
                warningList.Add($"Could not apply ParamDef for {paramName}.param with paramType {param.ParamType} in {oldNew} file. Valid ParamDef could not be found.");
            else if (matchType && !matchDefVersion)
                warningList.Add($"Could not apply ParamDef for {param.ParamType} in {oldNew} file. Cannot find ParamDef version {param.ParamdefDataVersion}.");
            else if (matchType && matchDefVersion)
                warningList.Add($"Could not apply ParamDef for {param.ParamType} in {oldNew} file. Row sizes do not match. Param: {bestRowsize}, Def: {bestDefRowSize}.");
            else
                throw new Exception("Unhandled Apply ParamDef error.");

            return null;
        }

        public static string ParseRegulationVersion(ulong version) => ParseRegulationVersion(version.ToString());
        public static string ParseRegulationVersion(string versionStr)
        {
            if (versionStr.Length != 8)
                return "Invalid regulation verison";

            versionStr = versionStr.Insert(1, ".");
            versionStr = versionStr.Insert(4, ".");
            versionStr = versionStr[..6];

            return versionStr;
        }

    }

}
