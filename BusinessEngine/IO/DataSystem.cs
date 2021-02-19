using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using BusinessEngine.Operating;
using BusinessEngine.Sales;
using System.Linq;
using Microsoft.VisualBasic;

namespace BusinessEngine.IO
{

    /* 
     program.ini

    [PROGRAM]
    SavePath=./database.sqlite

     */

    /// <summary>
    /// 테이블 이름 무조건 int 형식이여야함 (DataSystem을 거치기함을 위함)
    /// </summary>
    public enum Table
    {
        /// <summary>
        /// 판매사실 테이블
        /// 할인율,갯수,구매자([AC]의 PrimaryID에 먼저 추가후, 사용),판매일,입금예정일,판매상품([Product]의 PrimaryID)
        /// </summary>
        Sale,
        /// <summary>
        /// 채무 테이블
        /// 채권자([AC]의 PrimaryID),날짜,채무회수일,금액,상세이유
        /// </summary>
        Debt,
        /// <summary>
        /// 채권 테이블
        /// 채무사([AC]의 PrimaryID),날짜,회수일,금액,포기여부
        /// </summary>
        Bond,
        /// <summary>
        /// 거래처 테이블
        /// 이름,노트,(int)Warning
        /// Warning은 분석과 전략에 도움이됩니다.
        /// </summary>
        AC,
        /// <summary>
        /// 분개 사실 테이블
        /// 날짜,사용자 이름,사용처 이름,(int)JournalizingKind,설명,금액
        /// PrimaryID로 구분 불가능. 오직 이름으로 기재함
        /// </summary>
        Journalizing,
        /// <summary>
        /// 상품 테이블,
        /// 원재료는 [Cost] Primary ID 사용 예)'1,2,3' string으로 저장. ,로 구분
        /// 제조사는 [AC] Primary ID를 사용 예)'1'
        /// </summary>
        Product,
        /// <summary>
        /// 원재료 테이블 IProduct, Primary ID로 구분
        /// </summary>
        Cost,
    }
    public static class TableExtension
    {
        public static int ToInt(this Table t)
        {
            return (int)t;
        }
        public static Table? ToTable(this int intT)
        {
            try
            {
                return (Table)intT;
            }
            catch
            {
                Debug.WriteLine(intT);
                return null;
            }
        }
    }
    public class DataSystem
    {
        #region static, readonlys, enum
        /// <summary>
        /// program.ini의 저장위치입니다.
        /// </summary>
        public readonly string INISaveFile = "./program.ini";
        public readonly string DefaultDBSave = "./database.db";
        public readonly string TimeFormat = "yyyy-MM-dd HH:mm:ss";
        public bool Initialized = false;
        #endregion
        #region ini
        #region sections
        /// <summary>
        /// 프로그램 설정 섹션의 섹션 이름입니다.
        /// </summary>
        public static readonly string PROGRAM_SECTION = "PROGRAM";
        #endregion
        /// <summary>
        /// sql파일의 저장위치값의 키값입니다.
        /// </summary>
        #region keys
        public static readonly string SAVEPATH_KEY = "SavePath";
        #endregion
        #endregion
        string sqlPath;
        public DataSystem() {}

        public void Initialize()
        {
            if (!File.Exists(INISaveFile))
            {
                File.Create(INISaveFile);
                SQLiteConnection.CreateFile(DefaultDBSave);
                SetProgramINI(SAVEPATH_KEY, DefaultDBSave);
                sqlPath = DefaultDBSave;
            }
            else
            {
                sqlPath = GetProgramINI(SAVEPATH_KEY, DefaultDBSave);
                if (sqlPath == "") { SetProgramINI(SAVEPATH_KEY, DefaultDBSave); sqlPath = DefaultDBSave; }
                if (!File.Exists(sqlPath)) SQLiteConnection.CreateFile(sqlPath);
            }

            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                createTablesIfNotExists(sqlConnection);
            }

            Initialized = true;
        }

        public void SetProgramINI(string key, string value)
        {
            INI.Write(PROGRAM_SECTION, key, value, INISaveFile);
        }
        public string GetProgramINI(string key, string value)
        {
            return INI.Read(PROGRAM_SECTION, key, INISaveFile);
        }
        private string getInsertCommand(Table table, string[] queue)
        {
            string[] names = new string[queue.Length];
            string[] questionmks = new string[queue.Length];
            for (int i = 0; i < queue.Length; i++)
            {
                questionmks[i] = "?";
                names[i] = $"'{queue[i]}'";
            }

            return $"INSERT INTO '{table.ToInt()}' ({string.Join(',', names)}) VALUES ({string.Join(',',questionmks)})";
        }
        public void AddSale(DateTime depositDate, DateTime sellDate, AccountComany buyer, IProduct product, float discountRate, int qty)
        {
            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlConnection);
                cmd.CommandText = getInsertCommand(Table.Sale, new string[]
                {
                    "DepositDate",
                    "SellDate",
                    "BuyerID",
                    "ProductID",
                    "DiscountRate",
                    "Qty",
                });
                cmd.Parameters.Add(depositDate.ToString(TimeFormat));
                cmd.Parameters.Add(sellDate.ToString(TimeFormat));

                //buyer정보랑 일치하는것 primary value 뽑아서 씀
                //product정보랑 일치하는것 primary value ( = id)
                //cmd.Parameters.Add(buyer);
                //cmd.Parameters.Add(product);
                cmd.Parameters.Add(discountRate);
                cmd.Parameters.Add(qty);
            }
        }


        private KeyValuePair<string, string> tableElement(string name, string typenetc)
        {
            return new KeyValuePair<string, string>(name, typenetc);
        }
        private string getTableCreationCommand(bool onlyNotExist, bool columnId, string name,KeyValuePair<string,string>[] elements)
        {
            string ifnotexists = "IF NOT EXISTS", id = "'Id' INTEGER PRIMARY KEY AUTOINCREMENT";
            string table = $"CREATE TABLE {(onlyNotExist ? ifnotexists : "")} '{name}'({id}";

            for (int i = 0; i < elements.Length; i++)
                table += $", '{elements[i].Key}' {elements[i].Value}";

            table += ");";
            return table;
        }
        private void createTablesIfNotExists(SQLiteConnection conn)
        {
            using (var command = new SQLiteCommand(conn))
            {
                var a = getTableCreationCommand(true, true, Table.Sale.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("DepositDate","TEXT"),
                        tableElement("SellDate", "TEXT"),
                        tableElement("BuyerID", "INTEGER"),
                        tableElement("ProductID", "INTEGER"),
                        tableElement("DiscountRate", "INTEGER"),
                        tableElement("Qty", "INTEGER")
                });

                Debug.WriteLine(a);

                //Sale table
                command.CommandText = a;
                command.ExecuteNonQuery();
                //Debt table
                command.CommandText = getTableCreationCommand(true, true, Table.Debt.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("CreditorID", "INTEGER"),
                        tableElement("Date", "TEXT"),
                        tableElement("PayDate", "TEXT"),
                        tableElement("Amount", "INTEGER"),
                        tableElement("Why", "TEXT"),
                });
                command.ExecuteNonQuery();
                //Bond table
                command.CommandText = getTableCreationCommand(true, true, Table.Bond.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("DebtorID", "INTEGER"),
                        tableElement("Date", "TEXT"),
                        tableElement("PayDate", "TEXT"),
                        tableElement("Amount", "INTEGER"),
                        tableElement("ADN", "INTEGER"),
                });
                command.ExecuteNonQuery();
                //AC table
                command.CommandText = getTableCreationCommand(true, true, Table.AC.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("Name", "TEXT"),
                        tableElement("Note", "TEXT"),
                        tableElement("Warning", "INTEGER")
                });
                command.ExecuteNonQuery();
                //Journalizing table
                command.CommandText = getTableCreationCommand(true, true, Table.Journalizing.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("When", "TEXT"),
                        tableElement("FromACID", "INTEGER"),
                        tableElement("ToACID", "INTEGER"),
                        tableElement("WhatFor", "INTEGER"),
                        tableElement("Description", "TEXT"),
                        tableElement("Amount", "INTEGER"),
                });
                command.ExecuteNonQuery();
                //Product table
                command.CommandText = getTableCreationCommand(true, true, Table.Product.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("Name", "TEXT"),
                        tableElement("CostIDs", "TEXT"), // '1,2,3'
                        tableElement("Price", "INTEGER"),
                        tableElement("ManufacturerID", "INTEGER") //대부분 자신
                });
                //Cost table
                command.CommandText = getTableCreationCommand(true, true, Table.Cost.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("Name", "TEXT"),
                        tableElement("Price", "INTEGER"),
                        tableElement("ACID", "INTEGER")
                });
                command.ExecuteNonQuery();
            }
        }

    }
}
