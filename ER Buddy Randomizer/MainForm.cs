using SoulsFormats;

namespace BoreParamCompare
{
    public partial class MainForm : Form
    {

        //public string backupFile = Directory.GetCurrentDirectory() + "/regulation.bin.backup";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            b_activate.Enabled = false;

            toggle_buttons_dupe();

            if (cb_log_field_specifics.Checked)
                cb_fields_share_row.Enabled = true;
            else
                cb_fields_share_row.Enabled = false;

            Directory.CreateDirectory("Output");
        }

        private static PARAM.Row InsertParamRow(PARAM param, PARAM.Row row, int newID)
        {
            PARAM.Row newRow = new PARAM.Row(newID, "new row " + newID, param.AppliedParamdef);

            for (var i = 0; i < row.Cells.Count; i++)
            {
                newRow.Cells[i].Value = row.Cells[i].Value;
            }
            param.Rows.Insert(param.Rows.IndexOf(row) + 1, newRow); //messes up list order if you copy from one param to another

            return newRow;
        }


        public bool compareCells(List<string> changeList, PARAM.Row row_old, PARAM.Row row_new, string ID_str)
        {
            var changed = false;
            var combinedStr = ID_str+"";
            if (row_old.Cells.Count != row_new.Cells.Count)
            {
                throw new Exception("Field cell count mismatch! Was a new field introduced?");
            }
            else
            {
                for (var iField = 0; iField < row_old.Cells.Count; iField++)
                {
                    var field = row_old.Cells[iField];
                    string oldField = row_old.Cells[iField].Value.ToString();
                    string newField = row_new.Cells[iField].Value.ToString();

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

            if (cb_fields_share_row.Checked && changed)
                changeList.Add(combinedStr);


            return changed;
        }

        private string MakeIDString(string paramNameStr, PARAM.Row row, PARAM param)
        {
            string str = paramNameStr + "[ID " + row.ID.ToString() + "]";
            /*
            string namePath = "Paramdex\\ER\\" + param.ParamType + ".txt";
            if (File.Exists(namePath))
            {
                //todo. parse name list better (id and name are part of the same line, for a start)
                /*
                List<string> nameList = File.ReadAllLines(namePath).ToList();
                foreach (string ID in nameList)
                {
                    if (ID == row.ID.ToString())
                    {
                        //this row has a name definition, add it to the label.
                        str += "";
                        break;
                    }

                }
                //
            }
            */
            return str;
        }



        private void CompareFiles()
        {

            #region Load ParamBNDs from Regulation.bin
            Dictionary<string, PARAM> paramList_old = new();
            Dictionary<string, PARAM> paramList_new = new();

            string regPath_old = openFileDialog_old.FileName;
            string regPath_new = openFileDialog_new.FileName;

            string outputFileName = "Output\\"+openFileDialog_old.SafeFileName + " to " + openFileDialog_new.SafeFileName+".txt";

            UpdateConsole("Decrypting Regulation");

            BND4 paramBND_old = SFUtil.DecryptERRegulation(regPath_old); //load and decrypt param regulation
            BND4 paramBND_new = SFUtil.DecryptERRegulation(regPath_new); //load and decrypt param regulation

            UpdateConsole("Loading ParamDefs");

            var paramdefs = new List<PARAMDEF>();
            foreach (string path in Directory.GetFiles("Paramdex\\ER\\Defs", "*.xml"))
            {
                var paramdef = PARAMDEF.XmlDeserialize(path);
                paramdefs.Add(paramdef);
            }


            UpdateConsole("Handling Params");

            foreach (BinderFile file in paramBND_old.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                var param = PARAM.Read(file.Bytes);

                // Recommended method: checks the list for any match, or you can test them one-by-one
                if (param.ApplyParamdefCarefully(paramdefs))
                {
                    paramList_old[name] = param;
                }
            }

            foreach (BinderFile file in paramBND_new.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                var param = PARAM.Read(file.Bytes);

                // Recommended method: checks the list for any match, or you can test them one-by-one
                if (param.ApplyParamdefCarefully(paramdefs))
                {
                    paramList_new[name] = param;
                }
            }
            #endregion

            //
            UpdateConsole("Reading Params");
            //

            #region Read Params
            List<string> changeList = new(); //

            //scan for added params
            foreach (KeyValuePair<string, PARAM> item in paramList_new)
            {
                if (paramList_old.ContainsKey(item.Key) == false)
                {
                    //param was added
                    changeList.Insert(0,"PARAM ADDED: " + item.Key);
                    paramList_new.Remove(item.Key);
                    continue;
                }
            }
            
            //Check for changes
            foreach (KeyValuePair<string, PARAM> item in paramList_old)
            {
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
                bool logParamName = false;


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
                                    string ID_str = MakeIDString(paramNameStr, row2, param_old);//paramNameStr + "[ID " + row2.ID.ToString() + "]";
                                    changeList.Add(ID_str + " DUPLICATE ROW (OLD REGULATION)");
                                    logParamName = true;
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
                                string ID_str = MakeIDString(paramNameStr, row2, param_new);//paramNameStr+"[ID " + row2.ID.ToString() + "]";
                                if (changeList.Contains(ID_str + " DUPLICATE ROW (OLD REGULATION)"))
                                {
                                    //dupe is in both old and new param
                                    changeList.Remove(ID_str + " DUPLICATE ROW (OLD REGULATION)");
                                    if (cb_dupe_no_both.Checked == false)
                                    {
                                        changeList.Add(ID_str + " DUPLICATE ROW (BOTH REGULATIONS)");
                                        logParamName = true;
                                    }
                                }
                                else
                                {
                                    changeList.Add(ID_str + " DUPLICATE ROW (NEW REGULATION)");
                                    logParamName = true;
                                }
                            }

                            param_new.Rows.Remove(row2); //remove the non-first dupe row (only first is loaded in-game)
                            iRow--; //check same ID again in case of multiple dupes
                            break;
                        }
                    }
                }

                //check for added rows
                for (var i = 0; i < param_new.Rows.Count; i++)
                {
                    PARAM.Row row_new = param_new.Rows[i];

                    if (param_old[row_new.ID] == null)
                    {
                        string ID_new_str = MakeIDString(paramNameStr, row_new, param_new);//paramNameStr+"[ID " + row_new.ID.ToString() + "]";
                        changeList.Add(ID_new_str + " ROW ADDED");
                        logParamName = true;

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
                        string ID_old_str = MakeIDString(paramNameStr, row_old, param_new);//paramNameStr+"[ID " + row_old.ID.ToString() + "]";
                        changeList.Add(ID_old_str + " ROW REMOVED");
                        logParamName = true;

                        param_old.Rows.Remove(row_old);
                        i--;
                    }
                }

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
                    string ID_old_str = MakeIDString(paramNameStr, row_old, param_old);//paramNameStr+"[ID " + row_old.ID.ToString() + "]";
                    string ID_new_str = MakeIDString(paramNameStr, row_new, param_new);//paramNameStr+"[ID " + row_new.ID.ToString() + "]";

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
                        /*
                        //error with new row here. seems to step from dupe ID rows existing?
                        if (row_new == null)
                        {
                            throw new Exception("no");
                        }
                        */

                        if (compareCells(changeList, row_old, row_new, ID_old_str))
                            logParamName = true;

                        param_old.Rows.Remove(row_old);
                        param_new.Rows.Remove(row_new);
                        iRow -= 2;
                        rowCount -= 2;
                        continue;
                        //}
                    }

                    if (compareCells(changeList, row_old, row_new, ID_old_str))
                    {
                        logParamName = true;
                        //iRow--;
                    }

                    if (param_old.Rows.Count < param_new.Rows.Count)
                        rowCount = param_new.Rows.Count;
                    else
                        rowCount = param_old.Rows.Count;
                    }
                if (logParamName == false)
                    changeList.Remove(paramSpacer); //remove label for unchanged param type
            }
            #endregion

            //done
            /*
            if (cb_log_field_specifics.Checked)
            {
                outputFileName = "Changes FULL "+outputFileName'
            }
            else
            {
                outputFileName = "Changes BRIEF " + outputFileName;
            }
            */

            File.WriteAllLines(outputFileName, changeList);
            System.Diagnostics.Process.Start(@"explorer.exe", AppDomain.CurrentDomain.BaseDirectory+ outputFileName);

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
                if (openFileDialog_old.FileName != "" && openFileDialog_new.FileName != "")
                    b_activate.Enabled = true;
            }
        }
        private void b_browse_old_Click(object sender, EventArgs e)
        {
            loadFile(openFileDialog_old);
        }

        private void b_browse_new_Click(object sender, EventArgs e)
        {
            loadFile(openFileDialog_new);
        }


        public void UpdateConsole(string text)
        {
            t_console.Text = text;
            Application.DoEvents();
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

        /*
        private void b_restoreRegulation_Click(object sender, EventArgs e)
        {
            string regulationPath = openFileDialog_old.FileName;
            DialogResult result = MessageBox.Show("Delete current Regulation.bin and restore backup?", "Restore Backup", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                FileSystem.DeleteFile(regulationPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                File.Move(backupFile, regulationPath, false);
                UpdateConsole("Backup Restored");
                b_restoreRegulation.Enabled = false;
            }
        }
        */
    }
}