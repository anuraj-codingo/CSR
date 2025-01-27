﻿using CustomerServicePortal.Models;
using IBM.Data.DB2.iSeries;
using System;
using System.Configuration;
using System.Data;
using System.Web;

namespace CustomerServicePortal
{
    public static class Db2Connnect
    {
        public static DataTable GetDataTable(string commandText, CommandType commandType, iDB2Parameter[] parameters = null)
        {
            var dataset = new DataSet();
            try
            {
                LayoutModel layoutModel = new LayoutModel();
                layoutModel = (LayoutModel)HttpContext.Current.Session["LayoutDetails"];
                if (layoutModel.ClientDatabase !=null)
                {
                    string Conn = "DataSource = as400.abc.abchldg.com; userid = aspamo; password = a$pamo99; Default Collection =" + layoutModel.ClientDatabase + ";";
                    using (iDB2Connection connection = new iDB2Connection(Conn))
                    {
                        connection.Open();

                        using (iDB2Command command = new iDB2Command(commandText, commandType, connection))
                        {
                            command.CommandTimeout = 1000;
                            if (parameters != null)
                            {
                                foreach (var parameter in parameters)
                                {
                                    command.Parameters.Add(parameter);
                                }
                            }

                          
                            iDB2DataAdapter dataAdaper = new iDB2DataAdapter(command);
                            dataAdaper.Fill(dataset);
                            connection.Close();
                            return dataset.Tables[0];
                        }
                    }

                }
                return dataset.Tables[0];
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DataSet GetDataSet(string commandText, CommandType commandType, iDB2Parameter[] parameters = null)
        {
            try
            {
                using (iDB2Connection connection = new iDB2Connection(ConfigurationManager.ConnectionStrings["DB2"].ConnectionString))
                {
                    connection.Open();

                    using (iDB2Command command = new iDB2Command(commandText, commandType, connection))
                    {
                        command.CommandTimeout = 300;
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        var dataset = new DataSet();
                        iDB2DataAdapter dataAdaper = new iDB2DataAdapter(command);
                        dataAdaper.Fill(dataset);
                        connection.Close();
                        return dataset;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static iDB2Parameter CreateParameter(string name, object value, iDB2DbType dbType)
        {
            iDB2Parameter parm1 = new iDB2Parameter();
            parm1.iDB2DbType = dbType;
            parm1.ParameterName = name;
            parm1.iDB2Value = value;
            return parm1;
        }
    }
}