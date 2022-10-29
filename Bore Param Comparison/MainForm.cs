using SoulsFormats;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace BoreParamCompare
{
    /* TODO
     * 
     */
    public partial class MainForm : Form
    {
        public static string Version = Application.ProductVersion;
        public static string ProgramTitle = $"Bore Param Comparison v{Version}";

        private string gameType = "";

        private readonly List<string> gameTypes = new()
        {
            "DES",
            "DS1",
            "DS1R",
            "DS2",
            "DS2S",
            "DS3",
            "BB",
            "SDT",
            "ER",
        };

        public MainForm()
        {
            InitializeComponent();
            this.Text = ProgramTitle;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            combo_logNameExclusive.SelectedIndex = 0;

            toggle_buttons_dupe();
            toggle_buttons_logNames();

            if (cb_log_field_specifics.Checked)
                cb_fields_share_row.Enabled = true;
            else
                cb_fields_share_row.Enabled = false;

            menu_GameType.Items.Clear();
            menu_GameType.Items.AddRange(gameTypes.ToArray());

            // Create empty directories
            Directory.CreateDirectory("Output");
            foreach (var typ in gameTypes)
            {
                Directory.CreateDirectory($"Paramdex ALT\\{typ}");
            }
        }
        /*
        private static string GetTime()
        {
            string time = DateTime.Now.ToString("MM.dd.yyyy HH-mm-ss");
            return time;
        }
        */

        public string GetPreferredRowName(PARAM.Row row_old, PARAM.Row row_new)
        {
            string rowname = "";
            if (cb_LogRowNames.Checked)
            {
                if (row_old.Name != "")
                    rowname = row_old.Name;
                else if (row_new.Name != "")
                    rowname = row_new.Name;
            }

            return rowname;
        }

        // Dumb solution to handle accessing combo_logNameExclusive index without running into thread issues.
        private bool logNameExclusive_Index_0 = true;
        private bool logNameExclusive_Index_1 = false;
        private bool logNameExclusive_Index_2 = false;

        public bool compareCells(List<string> changeList, PARAM.Row row_old, PARAM.Row row_new, string ID_str)
        {
            var changed = false;
            var combinedStr = ID_str;
            string nameChangeStr;
            string oneLineNameDiffstr = "";

            if (cb_LogRowNames.Checked == true)
            {
                if (logNameExclusive_Index_0)
                {
                    if (row_old.Name != row_new.Name)
                    {
                        //Name was changed
                        nameChangeStr = $"\"{row_old.Name}\" -> \"{row_new.Name}\"";
                        if (cb_fields_share_row.Checked)
                        {
                            combinedStr += "[" + nameChangeStr + "]";
                            if (cb_LogNamesOnlyIf_FieldChange.Enabled == false)
                                changed = true; //Mandate inclusion in changelog since name is different
                        }
                        else
                        {
                            //Prepare row name change for multi-line changelog
                            oneLineNameDiffstr = ID_str + " ROW NAME: " + nameChangeStr;
                        }
                    }
                    else if (cb_log_name_changes_only.Checked == false)
                    {
                        //Name unchanged, include the name in the string anyway
                        var rowname = GetPreferredRowName(row_old, row_new);
                        ID_str += "[" + rowname + "]";
                        combinedStr += "[" + rowname + "]";
                    }
                }
                else if (logNameExclusive_Index_1)
                {
                    // Only log new name
                    var rowname = row_new.Name;
                    ID_str += "[" + rowname + "]";
                    combinedStr += "[" + rowname + "]";
                }
                else if (logNameExclusive_Index_2)
                {
                    // Only log old name
                    var rowname = row_old.Name;
                    ID_str += "[" + rowname + "]";
                    combinedStr += "[" + rowname + "]";
                }
            }

            if (!cb_fields_share_row.Checked)
            {
                changeList.Add($"{ID_str} ROW MODIFIED:");
            }

            for (var iField = 0; iField < row_old.Cells.Count; iField++)
            {
                var log = false;
                var oldCell = row_old.Cells[iField];
                var newCell = row_new.Cells[iField];

                if (oldCell.Def != newCell.Def)
                {
                    // Fields don't match, this is a mixed-def check. Try to find correct field to compare (if it exists)
                    var newCell2 = row_new.Cells.FirstOrDefault(c => c.Def.InternalName == oldCell.Def.InternalName);
                    if (newCell2 == null)
                    {
                        var oldCell2 = row_old.Cells.FirstOrDefault(c => c.Def.InternalName == newCell.Def.InternalName);
                        if (oldCell2 == null)
                        {
                            // Couldn't find field.
                            continue;
                        }
                        else
                        {
                            oldCell = oldCell2;
                        }
                    }
                    else
                    {
                        newCell = newCell2;
                    }
                }

                var oldField = oldCell.Value;
                var newField = newCell.Value;

                var oldField_str = oldField.ToString();
                var newField_str = newField.ToString();

                string fieldName = row_old.Cells[iField].Def.InternalName;

                //check for field differences
                if (oldField.GetType() == typeof(byte[])) //check and handle if field is a byte array (padding)
                {
                    //is a byte array
                    var oldFieldArray = (byte[])oldField;
                    var newFieldArray = (byte[])newField;
                    for (var i=0; i < oldFieldArray.Length; i++)
                    {
                        var old_byte = oldFieldArray[i];
                        var new_byte = newFieldArray[i];

                        if (old_byte != new_byte)
                        {
                            //byte was changed
                            oldField_str = Convert.ToHexString(oldFieldArray);
                            newField_str = Convert.ToHexString(newFieldArray);
                            log = true;
                            changed = true;
                            break;
                        }
                    }
                }
                else if (oldField_str != newField_str) //check fields normally
                {
                    //field was changed
                    log = true;
                    changed = true;
                }

                if (log == true)
                {
                    if (cb_log_field_specifics.Checked)
                    {
                        //log details and continue loop
                        if (cb_fields_share_row.Checked)
                        {
                            combinedStr += " [] " + fieldName + ": " + oldField_str + " -> " + newField_str + "";
                        }
                        else
                        {
                            //changeList.Add(ID_str + " " + fieldName + ": " + oldField_str + " -> " + newField_str);
                            changeList.Add("\t" + fieldName + ": " + oldField_str + " -> " + newField_str);
                        }
                    }
                    else
                    {
                        //only log ID and end loop
                        changeList.Add(ID_str + " WAS MODIFIED");
                        return true;
                    }
                }
            }

            if (!cb_fields_share_row.Checked && !changed)
            {
                changeList.Remove($"{ID_str} ROW MODIFIED:");
            }

            if (changed)
            {
                //a field was changed
                if (cb_fields_share_row.Checked)
                {
                    //single row
                    changeList.Add(combinedStr);
                }
                else if (oneLineNameDiffstr != "")
                {
                    //multi row and name wa schanged, add it
                    changeList.Add(oneLineNameDiffstr);
                }
            }
            else if (cb_LogNamesOnlyIf_FieldChange.Checked == false && oneLineNameDiffstr != "")
            {
                //no field changes, but name was changed. log it.
                changeList.Add(oneLineNameDiffstr);
            }

            return changed;
        }

        private string ExclusiveLogName(PARAM.Row row)
        {
            if (cb_LogRowNames.Checked == true)
            {
                // Only log new name
                var rowname = row.Name;
                return "[" + rowname + "]";
            }
            return "";
        }

        private static string MakeIDString(string paramNameStr, PARAM.Row row, bool unused = false)
        {
            string str = paramNameStr + "[ID " + row.ID.ToString() + "]";
            /*
            if (addName == true && menu_log_row_name_behavior.SelectedIndex != ((int)RowNameBehaviorEnum.NoLog))
            {
                str += "[" + row.Name + "]";
            }
            */
            return str;
        }

        private bool CheckOodle()
        {
            if (File.Exists("oo2core_6_win64.dll") == false)
            {
                DialogResult result;
                do
                {
                    result = MessageBox.Show(
                       $"The selected files requires \"oo2core_6_win64.dll\", which can be found in your {gameType} directory." +
                       $"\n\nPlease copy and paste \"oo2core_6_win64.dll\" from your {gameType} directory to \"{Directory.GetCurrentDirectory()}\".",
                       $"Could not find oo2core_6_win64.dll", MessageBoxButtons.RetryCancel);

                    if (result == DialogResult.Cancel)
                        return false;
                }
                while (File.Exists("oo2core_6_win64.dll") == false);
            }

            return true;
        }

        private List<BinderFile>? GetBNDFiles(string path, bool is_old)
        {
            //Couple hamfisted things in here, but whatever. It works.

            List<BinderFile> list;
            string version;
            BND3 bnd3;
            BND4 bnd4;

            bool isRegulation = true;
            try
            {
                if (BND4.Is(path) || BND3.Is(path))
                    isRegulation = false; //file is a BND
            }
            catch (DllNotFoundException)
            {
                //oodle dll is required, but missing.
                if (CheckOodle() == false)
                    return null; //User cancelled oodle check, cancel comparison.

                if (BND4.Is(path) || BND3.Is(path))
                    isRegulation = false; //file is a BND
            }

            switch (gameType)
            {
                case "DES":
                case "DS1":
                case "DS1R":
                    bnd3 = BND3.Read(path);
                    list = bnd3.Files;
                    version = bnd3.Version;
                    break;
                case "DS2": //untested
                case "DS2S":
                case "BB": //untested
                case "SDT":
                    bnd4 = BND4.Read(path);
                    list = bnd4.Files;
                    version = bnd4.Version;
                    break;
                case "DS3":
                    if (isRegulation)
                        bnd4 = SFUtil.DecryptDS3Regulation(path);
                    else
                        bnd4 = BND4.Read(path);
                    list = bnd4.Files;
                    version = bnd4.Version;
                    break;
                case "ER":
                    if (isRegulation)
                        bnd4 = SFUtil.DecryptERRegulation(path);
                    else
                        bnd4 = BND4.Read(path);
                    list = bnd4.Files;
                    version = bnd4.Version;
                    break;
                default:
                    throw new Exception("Bad game type: " + gameType);
            }

            if (is_old)
                t_VersionOld.Text = version;
            else
                t_VersionNew.Text = version;

            return list;
        }

        private List<List<string>> CheckParamChanges(ConcurrentDictionary<string, PARAM> paramList_old, ConcurrentDictionary<string, PARAM> paramList_new)
        {
            ConcurrentBag<List<string>> superChangeList = new();

            Parallel.ForEach(Partitioner.Create(paramList_old), item =>
            {
                //
                //UpdateConsole($"Scanning Param: {item.Key}");
                //
                List<string> changeList = new();

                if (paramList_new.ContainsKey(item.Key) == false)
                {
                    //Couldn't find matching param in other list. Whatever caused this is logged elsewhere.
                    return;
                }
                
                PARAM param_old = paramList_old[item.Key];
                PARAM param_new = paramList_new[item.Key];

                // formatting
                string paramNameStr = "[" + item.Key + "]";
                string paramSpacer = "*** *** *** *** " + item.Key + " *** *** *** ***";

                changeList.Add(paramSpacer);
                int paramChanges = 0; //keep track of how many changes were made to this param (and remove the spacer if it's zero)

                //UpdateConsole("Reading Params (Duplicate rows)");

                //check for duplicate rows (old regulation)
                for (var iRow = 0; iRow < param_old.Rows.Count; iRow++)
                {
                    PARAM.Row row = param_old.Rows[iRow];
                    int ID = row.ID;
                    for (var i = 0; i < param_old.Rows.Count; i++)
                    {
                        PARAM.Row row2 = param_old.Rows[i];
                        if (ID == row2.ID && row2 != row)
                        {
                            //duplicate row found
                            if (cb_dupe.Checked)
                            {
                                if (cb_dupe_no_old.Checked == false)
                                {
                                    string ID_str = MakeIDString(paramNameStr, row2, false);//paramNameStr + "[ID " + row2.ID.ToString() + "]";
                                    changeList.Add(ID_str + " DUPLICATE ROW (OLD REGULATION)");
                                    paramChanges++;
                                }
                            }
                            param_old.Rows.Remove(row2); //remove the non-first dupe row (only first is loaded in-game)
                            iRow--; //check same ID again in case of multiple dupes
                            break;
                        }
                    }
                }

                //check for duplicate rows (new regulation)
                for (var iRow = 0; iRow < param_new.Rows.Count; iRow++)
                {
                    PARAM.Row row = param_new.Rows[iRow];
                    int ID = row.ID;
                    for (var i = 0; i < param_new.Rows.Count; i++)
                    {
                        PARAM.Row row2 = param_new.Rows[i];
                        if (ID == row2.ID && row2 != row)
                        {
                            //duplicate row found
                            if (cb_dupe.Checked)
                            {
                                string ID_str = MakeIDString(paramNameStr, row2, false);
                                if (changeList.Contains(ID_str + " DUPLICATE ROW (OLD REGULATION)"))
                                {
                                    //dupe is in both old and new param
                                    changeList.Remove(ID_str + " DUPLICATE ROW (OLD REGULATION)");
                                    paramChanges--;

                                    if (cb_dupe_no_both.Checked == false)
                                    {
                                        changeList.Add(ID_str + " DUPLICATE ROW (BOTH REGULATIONS)");
                                        paramChanges++;
                                    }
                                }
                                else
                                {
                                    changeList.Add(ID_str + " DUPLICATE ROW (NEW REGULATION)");
                                    paramChanges++;
                                }
                            }

                            param_new.Rows.Remove(row2); //remove the non-first dupe row (only first is loaded in-game)
                            iRow--; //check same ID again in case of multiple dupes
                            break;
                        }
                    }
                }
                //UpdateConsole("Reading Params (Added/Removed rows)");

                //check for added rows
                for (var i = 0; i < param_new.Rows.Count; i++)
                {
                    PARAM.Row row_new = param_new.Rows[i];

                    if (param_old[row_new.ID] == null)
                    {
                        string ID_new_str = MakeIDString(paramNameStr, row_new, true);
                        ID_new_str += ExclusiveLogName(row_new);
                        if (!cb_LogAddedRemovedRowCells.Checked)
                        {
                            changeList.Add(ID_new_str + " ROW ADDED");
                        }
                        else if (cb_log_field_specifics.Checked)
                        {
                            // Log all info for added row
                            string rowInfo = "";
                            string delimiter = ", ";
                            if (!cb_fields_share_row.Checked)
                            {
                                delimiter = "\r\n\t";
                                rowInfo = delimiter;
                            }
                            // Generate row data
                            foreach (var cell in row_new.Cells)
                            {
                                var value = cell.Value;
                                if (cell.Value.GetType() == typeof(byte[]))
                                    value = Convert.ToHexString((byte[])cell.Value);
                                rowInfo += $"{cell.Def.InternalName}: {value}{delimiter}";
                            }
                            rowInfo = rowInfo[..^delimiter.Length];
                            changeList.Add($"{ID_new_str} ROW ADDED: {rowInfo}");
                        }
                        //
                        paramChanges++;

                        param_new.Rows.Remove(row_new);
                        i--;
                    }
                }

                //check for removed rows
                for (var i = 0; i < param_old.Rows.Count; i++)
                {
                    PARAM.Row row_old = param_old.Rows[i];

                    if (param_new[row_old.ID] == null)
                    {
                        string ID_old_str = MakeIDString(paramNameStr, row_old, true);
                        ID_old_str += ExclusiveLogName(row_old);
                        if (!cb_LogAddedRemovedRowCells.Checked)
                        {
                            changeList.Add(ID_old_str + " ROW REMOVED");
                        }
                        else if (cb_log_field_specifics.Checked)
                        {
                            // Log all info for removed row
                            string rowInfo = "";
                            string delimiter = ", ";
                            if (!cb_fields_share_row.Checked)
                            {
                                delimiter = "\r\n\t";
                                rowInfo = delimiter;
                            }
                            // Generate row data
                            foreach (var cell in row_old.Cells)
                            {
                                var value = cell.Value;
                                if (cell.Value.GetType() == typeof(byte[]))
                                    value = Convert.ToHexString((byte[])cell.Value);
                                rowInfo += $"{cell.Def.InternalName}: {value}{delimiter}";
                            }
                            rowInfo = rowInfo[..^delimiter.Length];
                            changeList.Add($"{ID_old_str} ROW REMOVED: {rowInfo}");
                        }
                        paramChanges++;

                        param_old.Rows.Remove(row_old);
                        i--;
                    }
                }

                //UpdateConsole("Reading Params (Modified rows)");

                //check for modified rows (and moved rows)
                int rowCount = param_old.Rows.Count;
                if (param_old.Rows.Count != param_new.Rows.Count)
                {
                    //there's probably a duplicate row.
                    throw new Exception("row count is wrong! uh oh!");
                }

                for (var iRow = 0; iRow < rowCount; iRow++)
                {

                    PARAM.Row row_new = param_new.Rows[iRow];
                    PARAM.Row row_old = param_old.Rows[iRow];
                    string ID_old_str = MakeIDString(paramNameStr, row_old, false);
                    string ID_new_str = MakeIDString(paramNameStr, row_new, false);

                    if (row_new == null || param_new[row_old.ID] == null)
                    {
                        //old param probably has a duplicate row
                        throw new Exception("New row is null!");
                    }
                    else if (row_old == null)
                    {
                        //new param probably has a duplicate row
                        throw new Exception("Old row is null!");
                    }

                    // Go through row IDs and find new/missing
                    if (row_old.ID != row_new.ID)
                    {
                        //row was moved
                        row_new = param_new[row_old.ID]; //find the corresponding row at its new address

                        if (compareCells(changeList, row_old, row_new, ID_old_str))
                            paramChanges++;

                        param_old.Rows.Remove(row_old);
                        param_new.Rows.Remove(row_new);
                        iRow -= 2;
                        rowCount -= 2;
                        continue;
                    }

                    if (compareCells(changeList, row_old, row_new, ID_old_str))
                    {
                        paramChanges++;
                    }

                    if (param_old.Rows.Count < param_new.Rows.Count) //ham fist
                        rowCount = param_new.Rows.Count;
                    else
                        rowCount = param_old.Rows.Count;
                }

                if (paramChanges <= 0)
                    changeList.Remove(paramSpacer); //remove label for unchanged param type
                else
                    superChangeList.Add(changeList);
            });

            //sort super list
            var orderedSuperChangeList = superChangeList.OrderBy(list => list[0]).ToList();
            return orderedSuperChangeList;
        }

        private string? CompareFiles()
        {

            #region Load Params
            ConcurrentDictionary<string, PARAM> paramList_old = new();
            ConcurrentDictionary<string, PARAM> paramList_new = new();

            ConcurrentBag<string> paramTypeList_old = new(); // List of all param types. Is not modified by valid defs.
            ConcurrentBag<string> paramTypeList_new = new(); // List of all param types. Is not modified by valid defs.

            List<string> changeList = new();

            string regPath_old = openFileDialog_old.FileName;
            string regPath_new = openFileDialog_new.FileName;

            string outputFileName = $"{openFileDialog_old.SafeFileName} to {openFileDialog_new.SafeFileName}.txt";


            UpdateConsole("Loading ParamDefs");

            ConcurrentBag<PARAMDEF> paramdefs = new();
            foreach (string path in Directory.GetFiles("Paramdex\\" + gameType + "\\Defs", "*.xml"))
            {
                var paramdef = PARAMDEF.XmlDeserialize(path);
                paramdefs.Add(paramdef);
            }

            ConcurrentBag<PARAMDEF> paramdefs_alt = new();
            if (Directory.Exists("Paramdex ALT\\" + gameType + "\\Defs"))
            {
                foreach (string path in Directory.GetFiles("Paramdex ALT\\" + gameType + "\\Defs", "*.xml"))
                {
                    var paramdef = PARAMDEF.XmlDeserialize(path);
                    paramdefs_alt.Add(paramdef);
                }
            }

            UpdateConsole("Loading Params");

            if (regPath_old.EndsWith(".param"))
            {
                // Compare individual param files

                t_VersionOld.Text = "No Version";
                t_VersionNew.Text = "No Version";

                Util.ApplyParamDefs(paramdefs, paramdefs_alt, regPath_old, paramList_old, changeList, paramTypeList_old, "OLD");
                Util.ApplyParamDefs(paramdefs, paramdefs_alt, regPath_new, paramList_new, changeList, paramTypeList_new, "NEW");

            }
            else
            {
                // Compare paramBNDs

                List<BinderFile>? fileList_old = GetBNDFiles(regPath_old, true);
                if (fileList_old == null)
                {
                    UpdateConsole("Comparison Cancelled");
                    return null;
                }
                List<BinderFile>? fileList_new = GetBNDFiles(regPath_new, false);
                if (fileList_new == null)
                {
                    UpdateConsole("Comparison Cancelled");
                    return null;
                }

                UpdateConsole("Applying Defs");

                Util.ApplyParamDefs(paramdefs, paramdefs_alt, fileList_old, paramList_old, changeList, paramTypeList_old, "OLD");
                Util.ApplyParamDefs(paramdefs, paramdefs_alt, fileList_new, paramList_new, changeList, paramTypeList_new, "NEW");

                //Check for added/removed param types
                foreach(var paramType in paramTypeList_old.ToList())
                {
                    var otherFile = paramTypeList_new.FirstOrDefault(e => paramType == e);
                    if (otherFile == null)
                    {
                        // Can't find a match.
                        var defString = changeList.Find(e => e.Contains(paramType));
                        if (defString != null)
                            changeList.Remove(defString); // Remove entry for "Def could not be applied", if it exists.
                        changeList.Add($"PARAM TYPE REMOVED: {paramType}");
                        paramList_old.Remove(paramType, out _);
                    }
                }
                foreach (var paramType in paramTypeList_new.ToList())
                {
                    var otherFile = paramTypeList_old.FirstOrDefault(e => paramType == e);
                    if (otherFile == null)
                    {
                        // Can't find a match.
                        var defString = changeList.Find(e => e.Contains(paramType));
                        if (defString != null)
                            changeList.Remove(defString); // Remove entry for "Def could not be applied", if it exists.
                        changeList.Add($"PARAM TYPE ADDED: {paramType}");
                        paramList_new.Remove(paramType, out _);
                    }
                }
            }
            #endregion

            #region Read Params

            // Scan for params that can't be checked
            foreach (var item in paramList_new)
            {
                if (paramList_old.ContainsKey(item.Key) == false)
                {
                    //Couldn't find matching param in other list. Whatever caused this is logged elsewhere.
                    paramList_new.Remove(item.Key, out _);
                    continue;
                }
            }

            //Check for changes
            UpdateConsole("Checking param changes");
            
            var superChangeList = CheckParamChanges(paramList_old, paramList_new); //check param changes, return list of lists
            #endregion

            //compile changelists to single changelist for output
            foreach (List<string> list in superChangeList)
            {
                changeList.AddRange(list);
            }

            if (changeList.Count == 0)
                changeList.Add("No changes have been found.");

            changeList.Insert(0, $"{ProgramTitle}");
            changeList.Insert(1, $"Game: {gameType}");
            changeList.Insert(2, $"Version: {t_VersionOld.Text} to {t_VersionNew.Text}");

            Directory.CreateDirectory($"Output\\{gameType}");
            var outputPath = $"Output\\{gameType}\\{outputFileName}";

            File.WriteAllLines(outputPath, changeList);

            return outputFileName;
        }

        private void loadFile(FileDialog fileDialog)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                CheckEnableActivateButton();
            }
        }
        private void b_browse_old_Click(object sender, EventArgs e)
        {
            loadFile(openFileDialog_old);
            t_VersionOld.Text = "Loaded";
            UpdateConsole("Old param selected");
        }

        private void b_browse_new_Click(object sender, EventArgs e)
        {
            loadFile(openFileDialog_new);
            t_VersionNew.Text = "Loaded";
            UpdateConsole("New param selected");
        }

        public void UpdateConsole(string text)
        {
            t_console.Text = text;
            Application.DoEvents();
        }

        private void CheckEnableActivateButton()
        {
            if (openFileDialog_old.FileName != "" && openFileDialog_new.FileName != "" && gameType != "")
                b_activate.Enabled = true;
            return;
        }

        private void b_activate_Click(object sender, EventArgs e)
        {
            string? outputFileName = CompareFiles(); //do all the stuff

            GC.Collect(); //clear memory

            if (outputFileName != null)
            {
                UpdateConsole("Finished!");

                System.Media.SystemSounds.Exclamation.Play(); //make noise

                var result = MessageBox.Show("All done! Open the output file?", "Comparison Finished", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                    System.Diagnostics.Process.Start(@"explorer.exe", AppDomain.CurrentDomain.BaseDirectory + outputFileName); //open up the output file
            }


        }

        private void toggle_buttons_dupe()
        {
            if (cb_dupe.Checked)
            {
                cb_dupe_no_old.Enabled = true;
                cb_dupe_no_both.Enabled = true;
                toggle_buttons_dupe_no_old();
            }
            else
            {
                cb_dupe_no_old.Enabled = false;
                cb_dupe_no_both.Enabled = false;
            }
        }

        private void toggle_buttons_dupe_no_old()
        {
            if (cb_dupe_no_old.Checked)
            {
                cb_dupe_no_both.Enabled = false;
            }
            else
            {
                cb_dupe_no_both.Enabled = true;
            }
        }
 
        private void cb_dupe_CheckedChanged(object sender, EventArgs e)
        {
            toggle_buttons_dupe();
        }

        private void cb_dupe_no_old_CheckedChanged(object sender, EventArgs e)
        {
            toggle_buttons_dupe_no_old();
        }

        private void cb_log_field_specifics_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_log_field_specifics.Checked)
                cb_fields_share_row.Enabled = true;
            else
                cb_fields_share_row.Enabled = false;
        }

        private void cb_GameType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gameType = (string)menu_GameType.SelectedItem;
            CheckEnableActivateButton();
        }

        private void toggle_buttons_logNames()
        {
            if (cb_LogRowNames.Checked == true)
            {
                cb_log_name_changes_only.Enabled = true;
                cb_LogNamesOnlyIf_FieldChange.Enabled = true;
                combo_logNameExclusive.Enabled = true;
            }
            else
            {
                cb_log_name_changes_only.Enabled = false;
                cb_LogNamesOnlyIf_FieldChange.Enabled = false;
                combo_logNameExclusive.Enabled = false;
            }
        }

        private void openFileDialog_old_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateConsole("Reading Params");
        }

        private void cb_log_name_changes_only_CheckedChanged(object sender, EventArgs e)
        {
            toggle_buttons_logNames();
        }

        private void cb_LogRowNames_CheckedChanged(object sender, EventArgs e)
        {
            toggle_buttons_logNames();
        }

        private void combo_logNameExclusive_SelectedIndexChanged(object sender, EventArgs e)
        {
            toggle_buttons_logNames();
            logNameExclusive_Index_0 = false;
            logNameExclusive_Index_1 = false;
            logNameExclusive_Index_2 = false;
            if (combo_logNameExclusive.SelectedIndex == 0)
            {
                cb_log_name_changes_only.Enabled = true;
                cb_LogNamesOnlyIf_FieldChange.Enabled = true;
                logNameExclusive_Index_0 = true;
            }
            else if (combo_logNameExclusive.SelectedIndex == 1)
            {
                cb_log_name_changes_only.Enabled = false;
                cb_LogNamesOnlyIf_FieldChange.Enabled = false;
                logNameExclusive_Index_1 = true;
            }
            else if (combo_logNameExclusive.SelectedIndex == 2)
            {
                cb_log_name_changes_only.Enabled = false;
                cb_LogNamesOnlyIf_FieldChange.Enabled = false;
                logNameExclusive_Index_2 = true;
            }
        }

        private void t_VersionOld_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            openFileDialog_old.FileName = files[0];
            t_VersionOld.Text = "Loaded";
            CheckEnableActivateButton();
        }
        private void t_VersionNew_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            openFileDialog_new.FileName = files[0];
            t_VersionNew.Text = "Loaded";
            CheckEnableActivateButton();
        }
        private void t_VersionOld_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }
        private void t_VersionNew_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }
    }
}