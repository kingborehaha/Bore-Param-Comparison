using SoulsFormats;
using System.Threading.Tasks;

namespace BoreParamCompare
{
    /* TODO
     * test remaining games
     * Consider fooling around with Async to speed things up
     */
    public partial class MainForm : Form
    {

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
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            toggle_buttons_dupe();
            toggle_buttons_logNames();

            if (cb_log_field_specifics.Checked)
                cb_fields_share_row.Enabled = true;
            else
                cb_fields_share_row.Enabled = false;

            menu_GameType.Items.Clear();
            menu_GameType.Items.AddRange(gameTypes.ToArray());

            //menu_log_row_name_behavior.SelectedIndex = (int)RowNameBehaviorEnum.NoLog;

            Directory.CreateDirectory("Output");
        }

        private static string GetTime()
        {
            string time = DateTime.Now.ToString("MM.dd.yyyy HH-mm-ss");
            return time;
        }

        public string GetPreferredRowName(PARAM.Row row_old, PARAM.Row row_new)
        {
            string rowname = "";
            if (cb_LogRowNames.Checked == true && row_old.Name != "")
                rowname = row_old.Name;

            return rowname;
        }

        public bool compareCells(List<string> changeList, PARAM.Row row_old, PARAM.Row row_new, string ID_str)
        {
            var changed = false;
            var combinedStr = ID_str;
            string nameChangeStr;
            string oneLineNameDiffstr = "";

            if (row_old.Cells.Count != row_new.Cells.Count)
            {
                throw new Exception("Field cell count mismatch! Was a new field introduced?");
            }
            else
            {
                //if (cb_log_names.Checked)
                if (cb_LogRowNames.Checked == true)
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

                for (var iField = 0; iField < row_old.Cells.Count; iField++)
                {
                    string? oldField = row_old.Cells[iField].Value.ToString();
                    string? newField = row_new.Cells[iField].Value.ToString();

                    if (oldField == null || newField == null)
                    {
                        throw new Exception("Field was null! Was a new field introduced?");
                    }

                    if (oldField != newField)
                    {
                        //field was changed
                        string fieldName = row_old.Cells[iField].Def.InternalName;
                        if (cb_log_field_specifics.Checked)
                        {
                            //log details and continue loop
                            if (cb_fields_share_row.Checked)
                            {
                                combinedStr += " [] " + fieldName + ": " + oldField + " -> " + newField + "";
                            }
                            else
                            {
                                changeList.Add(ID_str + " " + fieldName + ": " + oldField + " -> " + newField);
                            }
                        }
                        else
                        {
                            //only log ID and end loop
                            changeList.Add(ID_str + " WAS MODIFIED");
                            return true;
                        }

                        changed = true;
                    }
                }
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

        private string MakeIDString(string paramNameStr, PARAM.Row row, bool addName)
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

        private bool CheckOodle(string path)
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
                if (CheckOodle(path) == false)
                    return null;

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
                case "DS2S": //not thoroughly tested
                case "BB": //untested
                case "SDT": //not thoroughly tested
                    bnd4 = BND4.Read(path);
                    list = bnd4.Files;
                    version = bnd4.Version;
                    break;
                case "DS3": //not thoroughly tested
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
                    throw new Exception("Bad game type: "+gameType);
            }

            if (is_old)
                t_VersionOld.Text = version;
            else
                t_VersionNew.Text = version;

            return list;
        }

        private void CompareFiles()
        {

            #region Load Params
            Dictionary<string, PARAM> paramList_old = new();
            Dictionary<string, PARAM> paramList_new = new();
            List<string> changeList = new(); //

            string regPath_old = openFileDialog_old.FileName;
            string regPath_new = openFileDialog_new.FileName;

            string outputFileName = $"Output\\{openFileDialog_old.SafeFileName} to {openFileDialog_new.SafeFileName}.txt";


            UpdateConsole("Loading ParamDefs");

            List<PARAMDEF> paramdefs = new();
            foreach (string path in Directory.GetFiles("Paramdex\\" + gameType + "\\Defs", "*.xml"))
            {
                var paramdef = PARAMDEF.XmlDeserialize(path);
                paramdefs.Add(paramdef);
            }


            UpdateConsole("Loading Params");

            if (regPath_old.EndsWith(".param"))
            {
                //single .param
                PARAM param_old = PARAM.Read(regPath_old);
                PARAM param_new = PARAM.Read(regPath_new);

                t_VersionOld.Text = "Invalid";
                t_VersionNew.Text = "Invalid";

                var nameOld = Path.GetFileNameWithoutExtension(regPath_old);
                var nameNew = Path.GetFileNameWithoutExtension(regPath_new);

                if (param_old.ApplyParamdefCarefully(paramdefs))
                {
                    paramList_old[nameOld] = param_old;
                }
                else
                {
                    changeList.Add($": Could not apply ParamDef for (old) {param_old.ParamType}. If correct game was selected, param is incompatible with up-to-date ParamDef");
                    //throw new Exception("Could not apply paramDef! You probably selected the wrong game");
                }

                if (param_new.ApplyParamdefCarefully(paramdefs))
                {
                    paramList_new[nameNew] = param_new;
                }
                else
                {
                    changeList.Add($": Could not apply ParamDef for (new) {param_new.ParamType}. If correct game was selected, param is incompatible with up-to-date ParamDef");
                    //throw new Exception("Could not apply paramDef for new ! You probably selected the wrong game");
                }

            }
            else
            {
                //multiple params
                List<BinderFile>? fileList_old = GetBNDFiles(regPath_old, true);
                if (fileList_old == null)
                    return;
                List<BinderFile>? fileList_new = GetBNDFiles(regPath_new, false);
                if (fileList_new == null)
                    return;

                UpdateConsole("Applying Defs");

                foreach (BinderFile file in fileList_old)
                {
                    if (file.Name.Contains(".param") == false)
                        continue; //not a param.
                    string name = Path.GetFileNameWithoutExtension(file.Name);
                    var param = PARAM.Read(file.Bytes);

                    if (param.ApplyParamdefCarefully(paramdefs))
                    {
                        paramList_old[name] = param;
                    }
                    else
                    {
                        changeList.Add($"Could not apply ParamDef for {param.ParamType} in OLD file. If correct game was selected, Param is incompatible with current ParamDef");
                        //throw new Exception("Could not apply paramDef! You probably selected the wrong game");
                    }
                }

                foreach (BinderFile file in fileList_new)
                {
                    if (file.Name.Contains(".param") == false)
                        continue; //not a param.
                    string name = Path.GetFileNameWithoutExtension(file.Name);
                    var param = PARAM.Read(file.Bytes);

                    // Recommended method: checks the list for any match, or you can test them one-by-one
                    if (param.ApplyParamdefCarefully(paramdefs))
                    {
                        paramList_new[name] = param;
                    }
                    else
                    {
                        changeList.Add($"Could not apply ParamDef for {param.ParamType} in NEW file. If correct game was selected, Param is incompatible with current ParamDef");
                        //throw new Exception("Could not apply paramDef! You probably selected the wrong game");
                    }
                }
            }
            #endregion


            #region Read Params

            //scan for added params
            foreach (KeyValuePair<string, PARAM> item in paramList_new)
            {
                if (paramList_old.ContainsKey(item.Key) == false)
                {
                    //param was added
                    changeList.Insert(0,"PARAM TYPE ADDED OR MODIFIED: " + item.Key);
                    paramList_new.Remove(item.Key);
                    continue;
                }
            }

            //Check for changes
            foreach (KeyValuePair<string, PARAM> item in paramList_old)
            {

                //
                UpdateConsole($"Scanning Param: {item.Key}");
                //

                //scan for removed params
                if (paramList_new.ContainsKey(item.Key) == false)
                {
                    //param was removed
                    changeList.Insert(0,"PARAM REMOVED: " + item.Key);
                    paramList_old.Remove(item.Key);
                    continue;
                }

                PARAM param_old = paramList_old[item.Key];
                PARAM param_new = paramList_new[item.Key];

                // formatting
                string paramNameStr = "[" + item.Key + "]";
                string paramSpacer = "*** *** *** *** " +item.Key+ " *** *** *** ***";

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
                        changeList.Add(ID_new_str + " ROW ADDED");
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
                        changeList.Add(ID_old_str + " ROW REMOVED");
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

                    //go through row IDs and find new/missing rows
                    if (row_old.Cells.Count != row_new.Cells.Count)
                    {
                        throw new Exception("Field cell count mismatch! Was a new field introduced?");
                    }
                    else if (row_old.ID != row_new.ID)
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
            }
            #endregion

            changeList.Insert(0, "Game Type: " + gameType);

            File.WriteAllLines(outputFileName, changeList);
            System.Diagnostics.Process.Start(@"explorer.exe", AppDomain.CurrentDomain.BaseDirectory+ outputFileName); //open up the output file

            /*
            UpdateConsole("Exporting Params");

            //output regulation
            foreach (BinderFile file in paramBND.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                if (paramList.ContainsKey(name))
                    file.Bytes = paramList[name].Write();
            }
            SFUtil.EncryptERRegulation(regulationPath, paramBND); //encrypt and write param regulation
            */

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
            CompareFiles(); //do all the stuff

            GC.Collect(); //clear memory

            UpdateConsole("Finished!");
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
            //gameType = Enum.Parse<GameTypeEnum>(cb_GameType.SelectedText);
            CheckEnableActivateButton();
        }

        private void toggle_buttons_logNames()
        {
            if (cb_LogRowNames.Checked == true)
            {
                cb_log_name_changes_only.Enabled = true;
                cb_LogNamesOnlyIf_FieldChange.Enabled = true;
            }
            else
            {
                cb_log_name_changes_only.Enabled = false;
                cb_LogNamesOnlyIf_FieldChange.Enabled = false;
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
    }
}