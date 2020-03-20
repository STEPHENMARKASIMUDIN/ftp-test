using System;
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;
using log4net;
/// <summary>
/// Summary description for DBConnection
/// </summary>
public class DBConnection
{
    private MySqlConnection connection;
    private MySqlDataReader dr;
    private MySqlCommand cmd = new MySqlCommand();
    private MySqlTransaction tran = null;
    private Boolean pool = false;
    String path;
    private static readonly ILog kplog = LogManager.GetLogger(typeof(DBConnection));
    //Constructor
    public DBConnection(String Serv, String DB, String UID, String Password, String pooling, Int32 maxcon, Int32 mincon, Int32 tout)
    {
        Initialize(Serv, DB, UID, Password, pooling, maxcon, mincon, tout);
    }

    //Initialize values
    private void Initialize(String Serv, String DB, String UID, String Password, String pooling, Int32 maxcon, Int32 mincon, Int32 tout)
    {
        try
        {
            if (pooling.Equals("1"))
            {
                pool = true;
            }

            string myconstring = "server = " + Serv + "; database = " + DB + "; uid = " + UID + ";password= " + Password + "; pooling=" + pool + ";min pool size=" + mincon + ";max pool size=" + maxcon + "; Connection Lifetime=0 ;Command Timeout=28800; connection timeout=" + tout + ";Allow Zero Datetime=true";
            connection = new MySqlConnection(myconstring);
        }
        catch (Exception ex)
        {
            kplog.Fatal(ex.ToString());
            throw new Exception(ex.Message);
        }

    }

    public String Path
    {
        get { return path; }
        set { path = value; }
    }
    //open connection to database
    public void OpenConnection()
    {
        connection.Open();
    }
  
    public void CloseConnection()
    {
        connection.Close();

    }
    public void ReleaseDr()
    {
        dr.Close();
    }

    public MySqlConnection GetConnection()
    {
        return connection;
    }
    public void ExecuteCommand(string query)
    {
        cmd = connection.CreateCommand();
        cmd.CommandTimeout = 600;
        cmd.CommandText = query;
    }
    public void Execute()
    {
        cmd.ExecuteNonQuery();
    }
    public void ExecuteTranCommand(string query)
    {
        cmd.Connection = connection;
        cmd.Transaction = tran;
        cmd.CommandTimeout = 600;
        cmd.CommandText = query;
    }
    public void StartTransaction()
    {
        tran = connection.BeginTransaction();
        cmd = connection.CreateCommand();
    }
    public void StartDapperTran()
    {
        tran = connection.BeginTransaction();
    }
    protected internal MySqlTransaction Transaction()
    {
        return tran;
    }
    public void CommitTran()
    {
        tran.Commit();
    }
    public void RollbackTran()
    {
        tran.Rollback();
    }
    public void QueryParameters(Dictionary<string, object> data)
    {
        foreach (var param in data)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }
    }
    protected internal MySqlDataReader ExeDr()
    {
        dr = cmd.ExecuteReader();
        return dr;
    }
    public void Dispose(string type)
    {
        if (type.ToUpper().Equals("ADOREADER"))
        {
            dr.Dispose();
            cmd.Dispose();
            connection.Dispose();
        }
        else if (type.ToUpper().Equals("ADOTRANSACTION") || type.ToUpper().Equals("DAPPERTRANSACTION"))
        {
            cmd.Dispose();
            connection.Dispose();
        }
        else
        {
            connection.Dispose();
        }
    }
    public int Execute(string sql, object param = null, int? commandTimeout = null)
    {
        return connection.Execute(sql, param, tran, commandTimeout);
    }
    public IEnumerable<T> Query<T>(string sql, object param = null, int? commandTimeout = null) 
    {
        return connection.Query<T>(sql, param, null, false, commandTimeout);
    }
}