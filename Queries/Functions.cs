﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace Queries
{
    class Functions
    {
        //**************

        #region ComboBoxSource
        /// <summary>
        /// set combobox source
        /// </summary>
        /// <param name="ComboBox">combobox name</param>
        /// <param name="Table">table name</param>
        /// <param name="ColumnIndex">index of column show in combobox</param>
        /// <param name="State">show in problem massage</param>
        public void ComboBoxSource(ComboBox ComboBox, DataTable Table, int ColumnIndex = 0, string State = "State")
        {
            try
            {
                ComboBox.BindingContext = new BindingContext();
                ComboBox.DataSource = Table;
                ComboBox.ValueMember = Table.Columns[ColumnIndex].ColumnName;
            }

            catch (Exception e)
            {
                if (State != "State")
                    MessageBox.Show(State + Environment.NewLine + e.Message);
            }
        }
        public void ComboBoxSource(DataGridViewComboBoxColumn dgvcmbColumn, DataTable Table, int ColumnIndex = 0, string State = "State")
        {
            try
            {
                dgvcmbColumn.DataSource = Table;
                dgvcmbColumn.DisplayMember = Table.Columns[ColumnIndex].ColumnName;
                dgvcmbColumn.ValueMember = Table.Columns[ColumnIndex].ColumnName;
            }

            catch (Exception e)
            {
                if (State != "State")
                    MessageBox.Show(State + Environment.NewLine + e.Message);
            }
        }

        #endregion        


        #region ListFind
        /// <summary>
        /// جستجو عبارت از لیست
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="match">عبارت جستجو</param>
        /// <returns>ایندکس</returns>
        public int ListFind(List<string> lst, string match)
        {
            int intRtn = -1;

            //  find math
            for (int i = 0; i < lst.Count; i++)
                if (lst[i].Contains(match)) intRtn = i;


            return intRtn;
        }

        #endregion


        #region SaveListToText
        /// <summary>
        /// ذخیره اطلاعات لاگین از لیست
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="Path">مسیر فایل</param>
        public void SaveListToText(List<string> lst, string Path)
        {
            File.WriteAllText(Path, lst[0]);
            for (int i = 1; i < lst.Count; i++)
            {
                File.WriteAllText(Path, File.ReadAllText(Path) + "\r\n" + lst[i]);
            }
        }
        #endregion


        #region CheckedListBoxSource


        public void CheckedListBoxSource(List<string> lstSource, CheckedListBox checkedListBox)
        {
            // clear list box
            checkedListBox.Items.Clear();

            // set list box source
            for (int i = 0; i < lstSource.Count; i++) checkedListBox.Items.Add(lstSource[i]);

        }

        #endregion


        #region GetQueryName
        /// <summary>
        ///  read querys name
        /// </summary>
        /// <param name="queryPath"></param>
        /// <param name="type"></param>
        /// <returns>list</returns>
        public List<string> GetQueryNameList(string strQueryPath, string strType)
        {
            List<string> lst = new List<string>();


            //  get all query file name
            string[] qryFileName = Directory.GetFiles(strQueryPath, "*.sql");


            //  file name with type
            foreach (string fileName in qryFileName)
            {
                //  type field
                if (strType != "جدول" && fileName.Contains("جدول") == false)
                { lst.Add(Path.GetFileNameWithoutExtension(fileName)); }

                //  type table
                if (strType == "جدول" && fileName.Contains("جدول"))
                { lst.Add(Path.GetFileNameWithoutExtension(fileName)); }
            }

            return lst;
        }

        public DataTable GetQueryNameeDatatable(string strQueryPath, string strType)
        {
            DataTable dt = new DataTable();
            string strFindTbName = "";

            dt.Columns.Add("FileQuery", typeof(string));
            dt.Columns.Add("TbName", typeof(string));

            //  get all query file name
            string[] qryFileName = Directory.GetFiles(strQueryPath, "*.sql");


            //  file name with type
            foreach (string fileName in qryFileName)
            {

                //  type field
                if (strType != "جدول" && fileName.Contains("جدول") == false)
                { dt.Rows.Add(Path.GetFileNameWithoutExtension(fileName)); }

                //  type table
                if (strType == "جدول" && fileName.Contains("جدول"))
                {
                    string strQry = File.ReadAllText(fileName);

                    //  tbl name index
                    int i = strQry.ToUpper().IndexOf("INTO [") + 6, b = strQry.ToUpper().IndexOf("] FROM") - i;

                    //  get table name
                    if (b > 0) strFindTbName = strQry.Substring(i, b);

                    //  add query file name
                    dt.Rows.Add(Path.GetFileNameWithoutExtension(fileName), strFindTbName);
                }
            }

            return dt;
        }

        public string GetQueryNameString(string strQuery, string strTbNmNew)
        {

            //  return query when substring
            string strQryRtn = "";
            int intStart = 0, intLen = 0;

            //  tbl name index
            if (strQryRtn == "" & strQuery.Contains("INTO ["))
            {
                intStart = strQuery.ToUpper().IndexOf("INTO [") + 6;
                intLen = strQuery.ToUpper().IndexOf("] FROM") - intStart;
                strQryRtn = strQuery.Substring(intStart, intLen);
            }

            if (strQryRtn == "" & strQuery.Contains("ALTER TABLE ["))
            {
                intStart = strQuery.ToUpper().IndexOf("ALTER TABLE [") + 13;
                intLen = strQuery.ToUpper().IndexOf("] ADD") - intStart;
                strQryRtn = strQuery.Substring(intStart, intLen);
            }

            if (strQryRtn == "" & strQuery.Contains("DROP TABLE ["))
            {
                intStart = strQuery.ToUpper().IndexOf("DROP TABLE [") + 12;
                intLen = strQuery.Length - (intStart + 1);

                strQryRtn = strQuery.Substring(intStart, intLen);
            }




            //  tb name new
            strQryRtn = strQuery.Substring(intStart, intLen);

            //  create query new
            strQryRtn = strQuery.Replace(strQryRtn, strTbNmNew);

            return strQryRtn;

        }

        #endregion


        #region TableNameOfQuery
        public string TableNameOfQuery(string strQuery)
        {
            string strName = "";
            int intStart = 0, intLen = 0;

            //  tbl name index
            if (strName == "" & strQuery.Contains("INTO ["))
            {
                intStart = strQuery.ToUpper().IndexOf("INTO [") + 6;
                intLen = strQuery.ToUpper().IndexOf("] FROM") - intStart;
                strName = strQuery.Substring(intStart, intLen);
            }

            if (strName == "" & strQuery.Contains("ALTER TABLE ["))
            {
                intStart = strQuery.ToUpper().IndexOf("ALTER TABLE [") + 13;
                intLen = strQuery.ToUpper().IndexOf("] ADD") - intStart;
                strName = strQuery.Substring(intStart, intLen);
            }
            if (strName == "" & strQuery.Contains("DROP TABLE ["))
            {
                intStart = strQuery.ToUpper().IndexOf("DROP TABLE [") + 12;
                intLen = strQuery.Length - (intStart + 1);

                strName = strQuery.Substring(intStart, intLen);
            }



            //  return tb name

            return strName = strQuery.Substring(intStart, intLen); ;
        }
        #endregion


        #region ChangeTableName

        //  change table name
        public DataTable ChangeTableName(DataGridView dgvQrys, DataTable dtSource)
        {

            DataTable dt = new DataTable();

            //  add columns
            dt.Columns.Add("QueryName", typeof(string));
            dt.Columns.Add("TbNameOld", typeof(string));
            dt.Columns.Add("TbNameNew", typeof(string));


            //  get row checked
            for (int i = 0; i < dgvQrys.Rows.Count; i++)
            {
                //  checked item
                if (Convert.ToBoolean(dgvQrys.Rows[i].Cells[0].Value) == true)
                {
                    //  add row
                    string ss = dtSource.Rows[i][1].ToString();           //  old table name 
                    dt.Rows.Add(
                                dgvQrys.Rows[i].Cells[1].Value.ToString(),//  query name
                                dtSource.Rows[i][1].ToString(),           //  old table name 
                                dgvQrys.Rows[i].Cells[2].Value.ToString() //  new table name
                              );

                }

            }


            return dt;
        }
        #endregion


        #region DataTableToList
        public List<string> DataTableToList(DataTable dt, int IndexColumn = 0)
        {
            List<string> lst = new List<string>();

            //  convert data to list
            for (int i = 0; i < dt.Rows.Count; i++)
            { lst.Add(dt.Rows[i][IndexColumn].ToString()); }

            return lst;
        }
        #endregion


        #region ServerUserPass
        /// <summary>
        /// serverUserPass
        /// </summary>
        /// <param name="loginPath">مسیر فایل اطلاعات ورود به سرور</param>
        /// <param name="strServer">سرور</param>
        /// <returns>لیست شامل نام سرور ، نام کاربری و رمز عبور سرور</returns>
        public List<string> ServerUserPass(string loginPath, string strServer)
        {
            //  return list 
            List<string> lst = new List<string>();
            lst.Add(strServer); // add server



            //  all servers login info
            List<string> lstLogin = new List<string>();
            lstLogin = ReadTxt(loginPath);


            //  get this server user & pass add to lst
            for (int i = 0; i < lstLogin.Count; i++)
            {
                //  find this server info
                if (lstLogin[i].Contains(strServer))
                {
                    //  finde index of separator
                    List<int> lstIndexOf = new List<int>();
                    lstIndexOf = IndexOfAll(lstLogin[i], ",");

                    //  add user & pass to lst
                    lst.Add(lstLogin[i].Substring(lstIndexOf[0], (lstIndexOf[1] - lstIndexOf[0]) - 1));
                    lst.Add(lstLogin[i].Substring(lstIndexOf[1], lstLogin[i].Length - lstIndexOf[1]));
                }
            }


            //  set user & pass when server is local
            if (strServer == ".") lst[1] = lst[2] = "";

            //  decrypt pass
            try { lst[2] = CryptorEngine.Decrypt(lst[2], true); }
            catch (Exception) { }


            return lst;
        }

        #endregion


        #region ListServerName
        /// <summary>
        /// get server name from file
        /// </summary>
        /// <param name="strPathLogin">login file path</param>
        /// <returns>List</returns>
        public List<string> ListServerName(string strPathLogin)
        {
            List<string> lstSvrName = new List<string>();

            //  load server names into cmbServer
            if (File.Exists(strPathLogin))
            {
                lstSvrName = ReadTxt(strPathLogin);

                lstSvrName.Insert(0, "., ,");

                //  get server from read line
                for (int i = 0; i < lstSvrName.Count; i++)
                { lstSvrName[i] = lstSvrName[i].Substring(0, lstSvrName[i].IndexOf(",")); }

            }

            return lstSvrName;
        }
        #endregion


        #region GetListBoxSelectedItems

        public List<string> GetListBoxSelectedItemsText(ListBox listBox)
        {
            List<string> lstSelectedItems = new List<string>();

            foreach (var item in listBox.SelectedItems)
                lstSelectedItems.Add(listBox.GetItemText(item));

            return lstSelectedItems;
        }


        #endregion


        #region ListBoxSelectAllWithRollBack

        List<int> lstSelectedIndex = new List<int>();
        private void ListBoxSelectAllWithRollBack(bool bolCheckAll, ListBox listBox)
        {

            //  select all
            if (bolCheckAll == true)
            {
                //  for roll back
                foreach (var item in listBox.SelectedItems)
                { lstSelectedIndex.Add(listBox.Items.IndexOf(item)); }

                //  select all
                for (int i = 0; i < listBox.Items.Count; i++)
                { listBox.SetSelected(i, true); }
            }



            // select roll back
            else
            {
                //  unselect all
                for (int i = 0; i < listBox.Items.Count; i++)
                    listBox.SetSelected(i, false);

                //  roll back
                for (int i = 0; i < lstSelectedIndex.Count; i++)
                    listBox.SetSelected(lstSelectedIndex[i], true);

                // clear roll back items
                lstSelectedIndex.Clear();
            }
        }

        #endregion


        #region GetSelectedItemsText
        public List<string> ListBoxGetSelectedItemsText(ListBox listBox)
        {
            int intSelectedIndex = 0;
            string strSelectedText = "";
            List<string> lstSelectedItemsText = new List<string>();


            foreach (var item in listBox.SelectedItems)
            {
                // index of select
                intSelectedIndex = listBox.Items.IndexOf(item);

                // text of select
                strSelectedText = listBox.GetItemText(listBox.Items[intSelectedIndex]);

                //  add selected text to list
                lstSelectedItemsText.Add(strSelectedText);
            }

            return lstSelectedItemsText;
        }
        #endregion



        #region ListToString

        public string ListToString(List<string> lst, string strSeperator, string strAddToRow = "", bool bolQuery = false, string strTable = "")
        {
            string strListRows = "";

            //  normal string
            for (int i = 0; i < lst.Count; i++)
            {
                //  just exist one row
                if (lst.Count == 1)
                { strListRows = strAddToRow + lst[0]; break; }



                //  multi row
                // first row
                if (i == 0)
                { strListRows = strAddToRow + lst[0] + strSeperator; }

                //  between first & last row
                if (i > 0 && i < lst.Count - 1)
                { strListRows = strListRows + strAddToRow + lst[i] + strSeperator; }

                //  last row
                if (i > 0 && i == lst.Count - 1)
                { strListRows = strListRows + strAddToRow + lst[i]; }
            }


            // query string
            if (bolQuery)
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst.Count == 1)
                    { strListRows = strAddToRow + ".[" + lst[0] + "]"; break; }

                    if (i == 0)
                    { strListRows = strAddToRow + ".[" + lst[0] + "]" + strSeperator; }

                    if (i > 0 && i < lst.Count - 1)
                    { strListRows = strListRows + strAddToRow + ".[" + lst[i] + "]" + strSeperator; }

                    if (i > 0 && i == lst.Count - 1)
                    { strListRows = strListRows + strAddToRow + ".[" + lst[i] + "]"; }
                }
            }


            //  qury string with alies
            if (bolQuery == true && strTable != "")
            {


                for (int i = 0; i < lst.Count; i++)
                {
                    //  set alies
                    strTable = "] [" + lst[i] + "_" + strTable + "]";

                    if (lst.Count == 1)
                    { strListRows = strAddToRow + ".[" + lst[0] + strTable; break; }

                    if (i == 0)
                    { strListRows = strAddToRow + ".[" + lst[0] + strTable + strSeperator; }

                    if (i > 0 && i < lst.Count - 1)
                    { strListRows = strListRows + strAddToRow + ".[" + lst[i] + strTable + strSeperator; }

                    if (i > 0 && i == lst.Count - 1)
                    { strListRows = strListRows + strAddToRow + ".[" + lst[i] + strTable; }
                }
            }


            return strListRows;
        }

        #endregion



        #region QueryGetTableName
        public string QueryGetTableName(string strQuery)
        {
            string strName = "";
            int intStart = 0, intLen = 0;

            //  tbl name index
            if (strName == "" & strQuery.Contains("INTO ["))
            {
                intStart = strQuery.ToUpper().IndexOf("INTO [") + 6;
                intLen = strQuery.ToUpper().IndexOf("] FROM") - intStart;
                strName = strQuery.Substring(intStart, intLen);
            }

            if (strName == "" & strQuery.Contains("ALTER TABLE ["))
            {
                intStart = strQuery.ToUpper().IndexOf("ALTER TABLE [") + 13;
                intLen = strQuery.ToUpper().IndexOf("] ") - intStart;
                strName = strQuery.Substring(intStart, intLen);
            }
            if (strName == "" & strQuery.Contains("DROP TABLE ["))
            {
                intStart = strQuery.ToUpper().IndexOf("DROP TABLE [") + 12;
                intLen = strQuery.Length - (intStart + 1);

                strName = strQuery.Substring(intStart, intLen);
            }



            //  return tb name

            return strName = strQuery.Substring(intStart, intLen); ;
        }
        #endregion


        #region ChangeTbName

        //  change table name
        public DataTable ChangeTbName(DataGridView dgvQrys, DataTable dtSource)
        {

            DataTable dt = new DataTable();

            //  add columns
            dt.Columns.Add("QueryName", typeof(string));
            dt.Columns.Add("TbNameOld", typeof(string));
            dt.Columns.Add("TbNameNew", typeof(string));


            //  get row checked
            for (int i = 0; i < dgvQrys.Rows.Count; i++)
            {
                //  checked item
                if (Convert.ToBoolean(dgvQrys.Rows[i].Cells[0].Value) == true)
                {
                    //  add row
                    string ss = dtSource.Rows[i][1].ToString();           //  old table name 
                    dt.Rows.Add(
                                dgvQrys.Rows[i].Cells[1].Value.ToString(),//  query name
                                dtSource.Rows[i][1].ToString(),           //  old table name 
                                dgvQrys.Rows[i].Cells[2].Value.ToString() //  new table name
                              );

                }

            }


            return dt;
        }
        #endregion



        #region GetQueryFileName

        public DataTable GetQueryFileName(string strQueryPath, string strType, string strFilter="")
        {
            DataTable dtReturn = new DataTable();

            dtReturn.Columns.Add("QueryFileName", typeof(string));
            dtReturn.Columns.Add("IntoTableName", typeof(string));


            //  get all query file name
            string[] qryFileName = Directory.GetFiles(strQueryPath, "*.sql");


            //  file name with type
            foreach (string fileName in qryFileName)
            {

                //  type field
                if (strType != "جدول" && fileName.Contains("جدول") == false)
                {
                    string strFileName = Path.GetFileNameWithoutExtension(fileName);

                    if (strFilter != "")
                    {
                        if (strFileName.Contains(strFilter))
                        { dtReturn.Rows.Add(strFileName); }
                    }

                    else { dtReturn.Rows.Add(strFileName); }
                }


                //  type table
                else if (strType == "جدول" && fileName.Contains("جدول"))
                {
                    //  get table name from query
                    string strTableName = SqlQueryGetTableName(File.ReadAllText(fileName));
                    string strFileName = Path.GetFileNameWithoutExtension(fileName);

                    //  add query file name & table name
                    if (strFilter != "")
                    {                        
                        if (strFileName.Contains(strFilter))
                        { dtReturn.Rows.Add(strFileName, strTableName); }
                    }

                    else { dtReturn.Rows.Add(strFileName, strTableName); }
                }
            }

            return dtReturn;
        }

        #endregion




        //****************************  sql functin *****************************
        //**********************************************************************


        #region SqlConnection
        public SqlConnection SqlConnect(string server = ".", string user = "", string pass = "", string dbName = "")
        {
            string strconn, strdbName;


            //  set data base name
            if (dbName == "") { strdbName = "master"; } else { strdbName = dbName; }


            // create connection string
            if (user != "" && pass != "")
            { strconn = "Data Source=" + server + ";Initial Catalog=" + strdbName + ";Persist Security Info=True;User ID=" + user + ";Password=" + pass; }

            else strconn = "Data Source=.;Initial Catalog=" + strdbName + ";Integrated Security=True;Connect Timeout=5";


            SqlConnection sqlConnection = new SqlConnection(strconn);
            return sqlConnection;
        }
        #endregion



        #region SqlExcutCommand

        public void SqlExcutCommand(SqlConnection sqlConnection, string Query)
        {
            SqlCommand cmd = new SqlCommand(Query, sqlConnection);
            try
            {
                // conection timeoute
                cmd.CommandTimeout = 3600;

                //  open conection
                if (cmd.Connection.State == ConnectionState.Closed)
                { cmd.Connection.Open(); }

                // execute query
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception) { }
        }

        public string SqlExcutCommand(SqlConnection sqlConnection, string Query, out string strSqlError, string strState = "")
        {
            //  replace new line & tab space
            Query = Query.Replace("\r\n", " ").Replace("\n", " ").Replace("\t", "  ");


            SqlCommand cmd = new SqlCommand(Query, sqlConnection);
            try
            {
                // conection timeoute
                cmd.CommandTimeout = 3600;

                //  open conection
                if (cmd.Connection.State == ConnectionState.Closed)
                { cmd.Connection.Open(); }

                // execute query
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                strSqlError = "";
                return strState + " => Done";
            }

            catch (Exception ex)
            {
                strSqlError = ex.ToString();
                return strState + " => Error";
            }
        }

        #endregion


        #region SqlExcutCommandWithGO
        public List<string> SqlExcutCommandWithGO(SqlConnection sqlConnection, string Query, out List<string> lstSqlError, string strState = "")
        {
            //  for substring
            int intStart, intLenght;

            List<string> lstQry = new List<string>();
            List<string> lstResult = new List<string>();
            List<string> lstSqlErrorIn = new List<string>();


            // index of all GO in query
            List<int> lstIndexGo = IndexOfAll(Query, "GO");


            //  create evry query
            for (int i = 0; i < lstIndexGo.Count; i++)
            {

                //  first GO
                if (i == 0)
                { intLenght = lstIndexGo[0] - 1; lstQry.Add(Query.Substring(0, intLenght)); }


                //  midel Go    // GO between first & last 
                if (lstIndexGo.Count - 1 > i)
                {
                    intStart = lstIndexGo[i] + 2; intLenght = (lstIndexGo[i + 1] - 1) - (lstIndexGo[i] + 2);
                    lstQry.Add(Query.Substring(intStart, intLenght));
                }


                //  last GO
                if (lstIndexGo.Count - 1 == i)
                {
                    intStart = lstIndexGo[i] + 2; intLenght = Query.Length - intStart;
                    lstQry.Add(Query.Substring(intStart, intLenght));
                }

            }


            // run evry query & set result
            for (int i = 0; i < lstQry.Count; i++)
            {
                string strSqlError;
                lstResult.Add(SqlExcutCommand(sqlConnection, lstQry[i], out strSqlError, strState + " [" + i.ToString() + "]"));
                lstSqlErrorIn.Add(strSqlError);
            }

            //  out sql error
            lstSqlError = lstSqlErrorIn;

            //  return
            return lstResult;
        }
        #endregion


        #region SqlGetDBName
        /// <summary>
        /// بر روی سیستم SQL لیستی شامل تمام پایگاه  داده های
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable SqlGetDBName(string server, string dbname = "master")
        {
            string qry = "SELECT name FROM sys.databases WHERE database_id>0 order by name";
            DataTable dt = SqlDataAdapter(qry, server, dbname);
            return dt;
        }

        public DataTable SqlGetDBName(SqlConnection sqlConnection)
        {
            string qry = "SELECT name FROM sys.databases WHERE database_id>0 order by name";
            DataTable dt = SqlDataAdapter(sqlConnection, qry);
            return dt;
        }

        #endregion


        #region SqlDataAdapter

        public DataTable SqlDataAdapter(string query, string server, string DBName = "master", string stat = "SqlDataAdapter")
        {
            DataTable dt = new DataTable();

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(query, SqlConnect(server));
                da.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                if (stat != "SqlDataAdapter")
                    MessageBox.Show(stat + Environment.NewLine + e.Message, "خطا");
                return dt;
            }
        }

        public DataTable SqlDataAdapter(SqlConnection sqlConnection, string strQuery, string stat = "SqlDataAdapter", string strTable = "")
        {
            DataTable dt = new DataTable();

            try
            {
                //  select table qury if query empty
                if (strQuery == "" && strTable != "")
                { strQuery = "SELECT TOP 100 * FROM [" + strTable + "]"; }

                SqlDataAdapter da = new SqlDataAdapter(strQuery, sqlConnection);
                da.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                if (stat != "SqlDataAdapter")
                    MessageBox.Show(stat + Environment.NewLine + e.Message, "خطا");
                return dt;
            }
        }


        #endregion


        #region SqlTableName

        public List<string> SqlTableName(SqlConnection sqlConnection)
        {
            string Query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
            return DataTableToList(SqlDataAdapter(sqlConnection, Query));

        }

        #endregion


        #region SqlColumnsName

        /// <summary>
        /// list of columns name
        /// </summary>
        /// <param name="sqlConnection">SqlConnection</param>
        /// <param name="strTableName">TableName</param>
        /// <returns>List > string</returns>
        public List<string> SqlColumns(SqlConnection sqlConnection, string strTableName)
        {
            string Query = "SELECT COLUMN_NAME as 'نام ستون' FROM INFORMATION_SCHEMA.COLUMNS " + "WHERE TABLE_NAME = N'" + strTableName + "'";
            return DataTableToList(SqlDataAdapter(sqlConnection, Query));
        }

        #endregion


        #region RecoredCount
        public int RecoredCount(string query, string server)
        {
            SqlCommand cmd = new SqlCommand(query, SqlConnect(server));
            int count;
            cmd.Connection.Open();
            cmd.CommandTimeout = 3600;
            count = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Connection.Close();
            return count;
        }
        #endregion


        #region SqlConnectionChangeDB


        public SqlConnection SqlConnectionChangeDB(SqlConnection sqlConnection, string newDB)
        {
            sqlConnection.Close();

            SqlConnection sqlConn = new SqlConnection(sqlConnection.ConnectionString.Replace(sqlConnection.Database, newDB));

            return sqlConn;
        }


        #endregion


        #region SqlCheckUniqColumn
        public string SqlCheckUniqColumn(SqlConnection sqlConnection, string strTable, string strColumn)
        {
            // create query
            string strQry = "SELECT[" + strColumn + "],COUNT(*)cnt FROM[" + strTable + "] GROUP BY[" + strColumn + "] HAVING COUNT(*) > 1";

            // check result
            if (SqlDataAdapter(sqlConnection, strQry).Rows.Count > 0)
            { return strTable + " ==>" + strColumn + " ==> its not uniq"; }


            return strTable + " ==>" + strColumn + " ==> its uniq";
        }

        #endregion


        #region SqlDelTable
        public string SqlDelTable(SqlConnection sqlConnection, string strTableName)
        {
            string strSqlError = "";

            string strQry = "DROP TABLE [" + strTableName + "]";

            string strResult = SqlExcutCommand(sqlConnection, strQry, out strSqlError, "SqlDelTable");

            return strTableName + " ==> " + strResult;
        }
        #endregion


        #region SqlGetUniqData
        public string SqlGetUniqData(SqlConnection sqlConnection, string strTable, string strColumn, out string strAllQuery)
        {
            string strQry, strAllQry, strSqlError;


            //  remove old table
            strQry = " IF OBJECT_ID('t01', 'U') IS NOT NULL BEGIN DROP TABLE t01 END";
            strAllQry = strQry;


            //  remove old table
            strQry = " IF OBJECT_ID('" + strTable + "_uniq', 'U') IS NOT NULL BEGIN DROP TABLE [" + strTable + "_uniq] END";
            strAllQry = strAllQry + strQry;

            //  add RowNumber & uniq datat
            strQry =
                        " SELECT c.* INTO t01 FROM"
                        + " (SELECT [" + strColumn + "], MAX(RowNumber)RowNumber FROM(SELECT ROW_NUMBER() OVER(ORDER BY [" + strColumn + "])RowNumber, [" + strColumn + "] FROM [" + strTable + "])a GROUP BY [" + strColumn + "])b"
                        + " JOIN(SELECT ROW_NUMBER() OVER(ORDER BY [" + strColumn + "])RowNumber, * FROM [" + strTable + "])c ON b.RowNumber = c.RowNumber";
            strAllQry = strAllQry + strQry;

            //  new table in to table orginal uniq
            strQry = " DROP TABLE[" + strTable + "] SELECT * INTO [" + strTable + "_uniq] FROM dbo.t01 DROP TABLE dbo.t01";
            strAllQry = strAllQry + strQry;



            //  remove RowNumber column
            strQry = " ALTER TABLE [" + strTable + "_uniq] DROP COLUMN RowNumber";
            SqlExcutCommand(sqlConnection, strQry, out strSqlError, "GetUniqData");

            // strAllQuery  out
            strAllQuery = strAllQry + strQry;


            return strTable + " ==> " + strColumn + " ==> " + SqlExcutCommand(sqlConnection, strAllQry, out strSqlError, "GetUniqData");

        }
        #endregion


        #region SqlDropColumn
        public string SqlDropColumn(SqlConnection sqlConnection, string strTable, string strColumn)
        {
            string strSqlError;

            string strQuery = "ALTER TABLE [" + strTable + "] DROP COLUMN [" + strColumn + "]";

            return SqlExcutCommand(sqlConnection, strQuery, out strSqlError, strColumn + " ==> DropColumn");
        }
        #endregion


        #region SqlJoin
        public DataTable SqlJoin(SqlConnection sqlConnection, string strFirstTable, string strSecendTable, string strJointColumn)
        {
            string strQuery = "SELECT * FROM [" + strFirstTable + "] a LEFT JOIN [" + strSecendTable + "] b ON b.[" + strJointColumn + "] = a.[" + strJointColumn + "]";

            return SqlDataAdapter(sqlConnection, strQuery, "SqlJoin");
        }

        public DataTable SqlJoin(SqlConnection sqlConnection, string strFirstTable, string strSecendTable, string strJointColumn, List<string> lstFirsTableColumn, List<string> lstSecendTableColumn, out string strQuery, bool bolCreat = false)
        {
            string strFirstColumn = ListToString(lstFirsTableColumn, ",", "a", true, strFirstTable);
            string strSecendColumn = ListToString(lstSecendTableColumn, ",", "b", true, strSecendTable);

            string Query = strQuery = "";

            if (bolCreat == true)
            { Query = strQuery = "SELECT " + strFirstColumn + "," + strSecendColumn + " INTO " + strFirstTable + "_" + strSecendTable + " FROM [" + strFirstTable + "] a LEFT JOIN [" + strSecendTable + "] b ON b.[" + strJointColumn + "] = a.[" + strJointColumn + "]"; }

            if (bolCreat == false)
            { Query = strQuery = "SELECT " + strFirstColumn + "," + strSecendColumn + " FROM [" + strFirstTable + "] a LEFT JOIN [" + strSecendTable + "] b ON b.[" + strJointColumn + "] = a.[" + strJointColumn + "]"; }

            return SqlDataAdapter(sqlConnection, Query, "SqlJoin");
        }


        #endregion


        #region SqlRename
        public void SqlRename(SqlConnection sqlConnection, string strType, string strOldName, string strNewName, string strTable = "")
        {
            string strQuery = "", strSqlError;

            // renam table
            if (strType.ToUpper() == "TABLE")
            { strQuery = "EXEC sp_rename '" + strOldName + "', '" + strNewName + "'"; }

            //  rename column
            if (strType.ToUpper() == "COLUMN" && strTable != "")
            { strQuery = "EXEC sp_rename '" + strTable + "." + strOldName + "', '" + strNewName + "' , 'COLUMN'"; }


            SqlExcutCommand(sqlConnection, strQuery, out strSqlError, "SqlRename => " + strOldName);
        }
        #endregion


        #region SqlQueryGetTableName

        public string SqlQueryGetTableName(string strQuery)
        {

            string strTableName = "";
            int intStart = 0, intLen = 0;


            // get table name when contains "INTO"
            if (strQuery.Contains("INTO ["))
            {
                intStart = strQuery.ToUpper().IndexOf("INTO [") + 6;
                string strSubQuery = strQuery.Substring(intStart, strQuery.Length - intStart);
                intLen = strSubQuery.ToUpper().IndexOf("] ");
                strTableName = strSubQuery.Substring(0, intLen);
            }

            // get when contains "ALTER"
            else if (strQuery.Contains("ALTER TABLE ["))
            {
                intStart = strQuery.ToUpper().IndexOf("ALTER TABLE [") + 13;
                string strSubQuery = strQuery.Substring(intStart, strQuery.Length - intStart);
                intLen = strSubQuery.ToUpper().IndexOf("] ");
                strTableName = strSubQuery.Substring(0, intLen);
            }

            // get when contains "DROP TABLE"
            else if (strQuery.Contains("DROP TABLE ["))
            {
                intStart = strQuery.ToUpper().IndexOf("DROP TABLE [") + 12;
                intLen = strQuery.Length - (intStart + 1);

                strTableName = strQuery.Substring(intStart, intLen);
            }


            return strTableName;

        }

        #endregion


        #region SqlQueryChangeTableName

        public string SqlQueryChangeTableName(string strQuery, string strNewTableName)
        {
            // old table name
            string strTableName = SqlQueryGetTableName(strQuery);

            //  change table name in query
            return strQuery.Replace(strTableName, strNewTableName);

        }

        #endregion




        //*******************************************************************


        #region SqlConnectionTest

        public Boolean SqlConnectionTest(SqlConnection sqlConnection)
        {
            Boolean bolSqlConnectionTest = false;


            //  test connection
            try
            {
                sqlConnection.Open();
                bolSqlConnectionTest = true;
                sqlConnection.Close();
            }
            catch (Exception) { }


            return bolSqlConnectionTest;
        }

        #endregion






        #region ReadTxt
        /// <summary>
        /// read text file
        /// </summary>
        /// <param name="Path">مسیر فایل متنی</param>
        /// <returns></returns>
        public List<string> ReadTxt(string Path)
        {

            //read text file
            FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);


            // read line & add to list
            List<string> lst = new List<string>();
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string strline;

                while ((strline = streamReader.ReadLine()) != null)
                { lst.Add(strline); }
            }


            //  return
            return lst;
        }

        #endregion



        #region IndexOfAll

        /// <summary>
        /// IndexOfAll
        /// </summary>
        /// <param name="txt">متن اصلی</param>
        /// <param name="search">عبارت جستجو</param>
        /// <returns></returns>
        public List<int> IndexOfAll(string txt, string search)
        {
            List<int> lst = new List<int>();

            for (int i = txt.IndexOf(search); i > -1; i = txt.IndexOf(search, i + 1))
            { lst.Add(i + 1); }

            return lst;
        }
        #endregion


        #region save list to text
        /// <summary>
        /// ذخیره اطلاعات لاگین از لیست
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="Path">مسیر فایل</param>
        public void saveList(List<string> lst, string Path)
        {
            File.WriteAllText(Path, lst[0]);

            for (int i = 1; i < lst.Count; i++)
            {File.WriteAllText(Path, File.ReadAllText(Path) + "\r\n" + lst[i]);}
        }
        #endregion



    }
}
