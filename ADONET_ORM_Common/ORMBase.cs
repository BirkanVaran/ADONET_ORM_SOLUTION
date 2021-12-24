using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADONET_ORM_Common
{
    public class ORMBase<ET, OT> : IORM<ET> where ET : class, new() where OT : class, new()
    {
        public Type ETType
        {
            get
            {
                return typeof(ET);
            }
        }
        public Table TheTable
        {
            get
            {
                var attributes = ETType.GetCustomAttributes(typeof(Table), false);
                if (attributes != null && attributes.Any())
                {
                    Table theTable = (Table)attributes[0];
                    return theTable;
                }
                return null;
            }
        }
        private static OT _current;
        public static OT Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new OT();
                }
                return _current;
            }
        }

        public ET SelectET(int etID)
        {
            string query = string.Empty;
            var attributes = ETType.GetCustomAttributes(typeof(Table), false);
            if (attributes != null && attributes.Any())
            {
                Table tbl = attributes[0] as Table;
                query = $"SELECT * FROM {tbl.TableName} WHERE {tbl.PrimaryColumn}={etID}";
            }
            DataTable dt = new DataTable();
            using (Tools.MySqlDBConnection)
            {
                SqlCommand cmd = new SqlCommand(query, Tools.MySqlDBConnection);
                Tools.OpenTheConnection();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);
            }

            return dt.ToET<ET>();
        }



        public bool Delete(ET Entity)
        {
            int theID = 0;
            PropertyInfo[] properties = ETType.GetProperties();
            foreach (var item in properties)
            {
                if (item.Name == TheTable.PrimaryColumn)
                {
                    theID = (int)item.GetValue(Entity);
                }
            }
            string query = $"DELETE FROM {TheTable.TableName} WHERE {TheTable.PrimaryColumn}={theID}";

            ////1. YOL
            //SqlCommand cmd = new SqlCommand(query, Tools.MySqlDBConnection);
            //Tools.MySqlDBConnection.Open();
            //int affectedRows = cmd.ExecuteNonQuery();
            //Tools.MySqlDBConnection.Close();

            int affectedRows = 0;
            using (Tools.MySqlDBConnection)
            {
                SqlCommand cmd = new SqlCommand(query, Tools.MySqlDBConnection);
                Tools.OpenTheConnection();
                affectedRows = cmd.ExecuteNonQuery();
            }


            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }



        public bool Insert(ET Entity)
        {
            string query = "";
            string tableName = "";
            string properties = "";
            string values = "";

            PropertyInfo[] propertyArray = ETType.GetProperties();
            SqlCommand cmd = new SqlCommand();

            // INSERT INTO sorgu cümlesi için tablodaki Columnları oluşturacağız.

            foreach (var item in propertyArray)
            {
                if (item.Name == TheTable.IdentityColumn)
                {
                    continue; // Örneğin KitapId gibi kolonları INSERT cümlesine eklenmez.
                }
                else
                {
                    properties += item.Name + ","; // KitapAdi, SayfaSayisi, Stok...
                }

            }
            properties = properties.TrimEnd(',');

            // INSERT INTO (Properties) VALUES ()
            // VALUES içine alınacak değerler:

            foreach (var item in propertyArray)
            {
                if (item.Name != TheTable.IdentityColumn)
                {
                    if (item.GetValue(Entity) == null)
                    {
                        values += "null,";
                    }
                    else if (item.PropertyType.Name.Contains("DateTime"))
                    {
                        DateTime theDateTime;
                        DateTime.TryParse(item.GetValue(Entity).ToString(), out theDateTime);
                        string theDateTimeString = "'" + theDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        values += theDateTimeString + ",";

                    }
                    else if (item.PropertyType.Name.Contains("Bool"))
                    {

                        bool deger = (bool)item.GetValue(Entity);
                        string degerString = deger ? "1" : "0";
                        values += degerString + ",";
                    }
                    else if (item.PropertyType.Name.Contains("String"))
                    {
                        values += $"'{item.GetValue(Entity).ToString().Replace("'","''")}',"; // Suç ve Ceza,
                    }
                    else
                    {
                        values += item.GetValue(Entity) + ",";
                    }
                }
            }
            values = values.TrimEnd(',');

            // INSERT INTO () VALUES ()

            tableName = TheTable.TableName;
            query += $"INSERT INTO {tableName} ({properties}) VALUES ({values})";
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.Connection = Tools.MySqlDBConnection;
            Tools.OpenTheConnection();
            int affectedRows = cmd.ExecuteNonQuery();
            Tools.MySqlDBConnection.Close();

            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<ET> Select()

        {
            Type type = typeof(ET); //
            string querySentence = "SELECT * FROM ";
            var attributes = type.GetCustomAttributes(typeof(Table), false);
            if (attributes != null && attributes.Any())
            {
                Table tbl = (Table)attributes[0];
                querySentence += tbl.TableName; // SELECT * FROM Kitaplar
            }

            SqlCommand command = new SqlCommand(querySentence, Tools.MySqlDBConnection);
            Tools.OpenTheConnection();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            // DataTable nesneleri ToList isimli metodu kullanabiliyorlar.
            // Yani ToList<> Generic yapılı metot DataTable nesnesi Extension metot oldu.
            // Extension ilave, ek, artırma anlamına gelir.
            // Yani normalde dt nesnesinin böyler bir metodu yok.
            // Ama siz yazılımcı olarak bir metot yazıp o metodu st
            return dt.ToList<ET>();


        }

        public bool Update(ET Entity)
        {
            string query = "";
            string sets = "";
            PropertyInfo[] propertyArray = ETType.GetProperties();
            foreach (var item in propertyArray)
            {
                if (item.Name == TheTable.IdentityColumn)
                {
                    continue;
                }
                if (item.GetValue(Entity)==null)
                {
                    sets += item.Name + "=null";
                }
                else if (item.PropertyType.Name.Contains("DateTime"))
                {
                    DateTime theDate;
                    DateTime.TryParse(item.GetValue(Entity).ToString(), out theDate);
                    string theDateString = $"'{theDate.ToString("yyyy-MM-dd HH:mm:ss")}'";
                    sets += item.Name + "=" + theDateString + ",";
                }
                else if (item.PropertyType.Name.Contains("Bool"))
                {
                    bool deger = (bool)item.GetValue(Entity);
                    string degerString = deger ? "1" : "0";

                    //Örn SilindiMi=0
                    sets += $"{item.Name}={degerString},";

                }
                else if (item.PropertyType.Name.Contains("String") || item.PropertyType.Name.Contains("Char"))
                {
                    // Örn KitapAdi='Suç ve Ceza',
                    sets += $"{item.Name}='{item.GetValue(Entity).ToString().Replace("'","''")}',";
                }
                else
                {
                    sets += $"{item.Name}={item.GetValue(Entity)},";

                }

            }

            sets = sets.TrimEnd(',');
            query = $"UPDATE {TheTable.TableName} SET {sets} ";

            // WHERE'e ihtiyacımız var.

            foreach (var item in propertyArray)
            {
                if (item.Name == TheTable.PrimaryColumn)
                {
                    if (item.PropertyType.Name.Contains("String") || item.PropertyType.Name.Contains("Char"))
                    {

                        query += $"WHERE {item.Name}='{item.GetValue(Entity)}'";
                    }
                    else
                    {
                        query += $"WHERE {item.Name}={item.GetValue(Entity)}";
                    }
                    break;
                }
            }

            int affectedRows = 0;
            using (Tools.MySqlDBConnection)
            {
                SqlCommand cmd = new SqlCommand(query, Tools.MySqlDBConnection);
                Tools.OpenTheConnection();
                affectedRows = cmd.ExecuteNonQuery();
            }
            return affectedRows > 0 ? true : false;
        }
    }
}
