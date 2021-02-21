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
using BusinessEngine.Accounting;

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
        public readonly string INISaveFile = "./settings.ini";
        public readonly string DefaultDBSave = "./database.db";
        public readonly string TimeFormat = "yyyy-MM-dd HH:mm:ss";
        public bool Initialized = false;
        public AccountCompany MyCompany { get; private set; }
        public readonly int MyCompanyID = -1;
        #endregion
        #region ini
        #region sections
        /// <summary>
        /// 프로그램 설정 섹션의 섹션 이름입니다.
        /// </summary>
        public static readonly string PROGRAM_SECTION = "PROGRAM";
        /// <summary>
        /// 회사 설정 섹션의 섹션 이름입니다.
        /// </summary>
        public static readonly string COMPANY_SECTION = "COMPANY";
        #endregion
        /// <summary>
        /// sql파일의 저장위치값의 키값입니다.
        /// </summary>
        #region keys
        public static readonly string SAVEPATH_KEY = "SavePath";
        public static readonly string COMPANY_NAME_KEY = "Name";
        #endregion
        #endregion
        string sqlPath;
        const string myCompanyNote = "TXlDb21wYW55";
        /// <summary>
        /// ini, database를 총괄합니다.
        /// </summary>
        public DataSystem() {
            MyCompany = new AccountCompany();
            MyCompany.Note = myCompanyNote;
        }

        /// <summary>
        /// DataSystem을 사용하기전 무조건 호출해야합니다.
        /// </summary>
        /// <param name="ownCompany"></param>
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
        /// <summary>
        /// 소유한 회사를 사용할 수 있도록 거래처 목록에 추가합니다. 
        /// </summary>
        /// <param name="company">바꿀 회사입니다.</param>
        /// <param name="pastName">회사 이름을 바꿀경우에, 과거 이름을 나타냅니다.</param>
        public void SetMyCompany(Company company)
        {
            INI.Write(COMPANY_SECTION, COMPANY_NAME_KEY, company.Name, INISaveFile);
            MyCompany.Name = company.Name;
        }
        public string GetMyCompanyName()
        {
            return INI.Read(COMPANY_SECTION, COMPANY_NAME_KEY, INISaveFile);
        }
        public void SetProgramINI(string key, string value)
        {
            INI.Write(PROGRAM_SECTION, key, value, INISaveFile);
        }
        public string GetProgramINI(string key, string value)
        {
            return INI.Read(PROGRAM_SECTION, key, INISaveFile);
        }

        public void AddDebt(AccountCompany creditor, DateTime date, DateTime payday, float amount, string why)
        {
            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlConnection);
                cmd.CommandText = getInsertCommand(Table.Debt, new string[]
                {
                    "CreditorID",
                    "Date",
                    "PayDate",
                    "Amount",
                    "Why",
                });
                var id = getAccountCompanyID(creditor, sqlConnection);
                if (id == null)
                {
                    throw new SQLiteException($"There is no Accounting Company (Creditor) like {creditor.Name},{creditor.Note}");
                }
                cmd.Parameters.AddWithValue("CreditorID", id);
                cmd.Parameters.AddWithValue("Date",date.ToString(TimeFormat));
                cmd.Parameters.AddWithValue("PayDate",payday.ToString(TimeFormat));
                cmd.Parameters.AddWithValue("Amount",amount);
                cmd.Parameters.AddWithValue("Why",why);
                cmd.ExecuteNonQuery();
            }
        }
        public void AddBond(AccountCompany debtor, DateTime date, DateTime payday, float amount, bool abdn = false)
        {
            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlConnection);
                cmd.CommandText = getInsertCommand(Table.Bond, new string[]
                {
                    "DebtorID",
                    "Date",
                    "PayDate",
                    "Amount",
                    "ABN",
                });
                var id = getAccountCompanyID(debtor, sqlConnection);
                if (id == null)
                {
                    throw new SQLiteException($"There is no Accounting Company (Creditor) like {debtor.Name},{debtor.Note}");
                }
                cmd.Parameters.AddWithValue("DebtorID", id);
                cmd.Parameters.AddWithValue("Date", date.ToString(TimeFormat));
                cmd.Parameters.AddWithValue("PayDate", payday.ToString(TimeFormat));
                cmd.Parameters.AddWithValue("Amount", amount);
                cmd.Parameters.AddWithValue("ABN", abdn ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
        }
        public void AddAccountingCompany(AccountCompany ac)
        {
            if (ac == null) return;
            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlConnection);
                cmd.CommandText = getInsertCommand(Table.AC, new string[]
                {
                    "Name",
                    "Note",
                    "Warning"
                });

                cmd.Parameters.AddWithValue("Name", ac.Name);
                cmd.Parameters.AddWithValue("Note", ac.Note);
                cmd.Parameters.AddWithValue("Warning", (int)ac.WarningPoint);
                cmd.ExecuteNonQuery();
            }
        }
        public void AddProduct(string name, float price, AccountCompany manufacturer, params IProduct[] costs)
        {
            if (manufacturer == null) return;

            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlConnection);
                cmd.CommandText = getInsertCommand(Table.Product, new string[]
                {
                    "Name",
                    "Price",
                    "CostIDs",
                    "ManufacturerID"
                });
                List<int> costIds = new List<int>();
                for (int i = 0; i < costs.Length; i++)
                {
                    var costId = getCostProductID(costs[i], sqlConnection);
                    if (costId != null)
                        costIds.Add((int)costId);
                    else
                        throw new SQLiteException($"Cost Product {costs[i].Name} doesn't exists in Cost table");
                }
                var id = getAccountCompanyID(manufacturer, sqlConnection);
                if (id == null)
                {
                    throw new SQLiteException($"There is no manufacturer company (AC) like {manufacturer.Name},{manufacturer.Note}");
                }

                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Price", price);
                cmd.Parameters.AddWithValue("CostIDs", string.Join(',', costIds));
                cmd.Parameters.AddWithValue("ManufacturerID",id);
                cmd.ExecuteNonQuery();
            }
        }
        public void AddCostProduct(string name, float price, AccountCompany manufacturer)
        {
            if (manufacturer == null) return;

            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlConnection);
                cmd.CommandText = getInsertCommand(Table.Cost, new string[]
                {
                    "Name",
                    "Price",
                    "ManufacturerID"
                });

                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Price", price);
                var id = getAccountCompanyID(manufacturer, sqlConnection);
                if (id == null)
                {
                    throw new SQLiteException($"There is no manufacturer company (AC) like {manufacturer.Name},{manufacturer.Note}");
                }
                cmd.Parameters.AddWithValue("ManufacturerID",id);
                cmd.ExecuteNonQuery();
            }
        }
        public void AddSale(DateTime depositDate, DateTime sellDate, AccountCompany buyer, IProduct product, float discountRate, int qty)
        {
            if (depositDate == null || sellDate == null || buyer == null || product == null || discountRate < 0) return;
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
                cmd.Parameters.AddWithValue("DepositDate", depositDate.ToString(TimeFormat));
                cmd.Parameters.AddWithValue("SellDate", sellDate.ToString(TimeFormat));
                var id = getAccountCompanyID(buyer, sqlConnection);
                if (id == null)
                {
                    throw new SQLiteException($"There is no Accounting Company like {buyer.Name},{buyer.Note}");
                }
                var pid = getProductID(product, sqlConnection);
                if (pid == null)
                {
                    throw new SQLiteException($"There is no Product like {product.Name}, price {product.Price}");
                }
                cmd.Parameters.AddWithValue("BuyerID", id);
                cmd.Parameters.AddWithValue("ProductID", pid);
                cmd.Parameters.AddWithValue("DiscountRate", discountRate);
                cmd.Parameters.AddWithValue("Qty", qty);

                cmd.ExecuteNonQuery();
            }
        }
        public void AddJournalizing(DateTime when, UsedFor whatfor, AccountCompany from, AccountCompany to, string description, float amount)
        {
            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlConnection);
                cmd.CommandText = getInsertCommand(Table.Journalizing, new string[]
                {
                    "When",
                    "FromACID",
                    "ToACID",
                    "WhatFor",
                    "Description",
                    "Amount",
                });
                cmd.Parameters.AddWithValue("When", when.ToString(TimeFormat));
                var fromid = getAccountCompanyID(from, sqlConnection);
                if (fromid == null)
                {
                    throw new SQLiteException($"(Journalizing From) There's is no Accounting Company like {from.Name},{from.Note}");
                }
                var toid = getAccountCompanyID(to, sqlConnection);
                if (toid == null)
                {
                    throw new SQLiteException($"(Journalizing To) There's No Accounting Company like {to.Name},{to.Note}");
                }
                cmd.Parameters.AddWithValue("FromACID", fromid);
                cmd.Parameters.AddWithValue("ToACID", toid);
                cmd.Parameters.AddWithValue("WhatFor", (int)whatfor);
                cmd.Parameters.AddWithValue("Description", description);
                cmd.Parameters.AddWithValue("Amount", amount);

                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 업데이트 하기전에 ID를 먼저 알아야합니다.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newData"></param>
        public void UpdateAccountingCompany(AccountCompany oldData,AccountCompany newData)
        {
            int id = 0;

            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                int? nid = getAccountCompanyID(oldData, sqlConnection);
                if (nid != null)
                    id = (int)nid;
                else if (nid == null || nid == MyCompanyID)
                    return;
                using (SQLiteCommand cmd = new SQLiteCommand(sqlConnection))
                {
                    cmd.CommandText = $"UPDATE '{Table.AC.ToInt()}' SET Name='{newData.Name}', Note='{newData.Note}', Warning={(int)newData.WarningPoint} WHERE Id={id}";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteAccountingCompany(AccountCompany ac)
        {
            int id = 0;
            using (var sqlConnection = new SQLiteConnection($@"Data Source={sqlPath}"))
            {
                sqlConnection.Open();
                int? nid = getAccountCompanyID(ac, sqlConnection);
                if (nid != null)
                    id = (int)nid;
                else if (nid == null || nid == MyCompanyID)
                    return;
                using (SQLiteCommand cmd = new SQLiteCommand(sqlConnection))
                {
                    cmd.CommandText = $"DELETE FROM '{Table.AC.ToInt()}' WHERE Id={id}";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /*
        public int? GetACID(AccountCompany ac)
        {
            int? id = null;
            if (ac.Note == myCompanyNote) return MyCompanyID;
            using (SQLiteConnection conn = new SQLiteConnection(@$"Data Source={sqlPath}"))
            {
                conn.Open();
                id = getAccountCompanyID(ac, conn);
            }
            return id;
        }
        */
        public List<AccountCompany> GetAccountingCompanyByName(string name)
        {
            var acs = new List<AccountCompany>();
            using (SQLiteConnection conn = new SQLiteConnection(@$"Data Source={sqlPath}"))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"SELECT * FROM '{Table.AC.ToInt()}' WHERE Name = '{name}';";
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var c = new AccountCompany(r["Name"].ToString());
                        c.Note = r["Note"].ToString();
                        c.WarningPoint = (Warning)(int.Parse(r["Warning"].ToString()));
                        acs.Add(c);
                    }
                }
            }
            return acs;
        }
        public List<AccountCompany> GetAccountingCompanies()
        {
            var acs = new List<AccountCompany>();
            using (SQLiteConnection conn = new SQLiteConnection(@$"Data Source={sqlPath}"))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"SELECT * FROM '{Table.AC.ToInt()}';";
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var c = new AccountCompany(r["Name"].ToString());
                        c.Note = r["Note"].ToString();
                        c.WarningPoint = (Warning)(int.Parse(r["Warning"].ToString()));
                        acs.Add(c);
                    }
                }
            }
            return acs;
        }
        public List<IProduct> GetCostProducts()
        {
            var ps = new List<IProduct>();
            using (SQLiteConnection conn = new SQLiteConnection(@$"Data Source={sqlPath}"))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"SELECT * FROM '{Table.Cost.ToInt()}';";
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var i = r["ManufacturerID"].ToString();
                        var m = getAccountingCompanyById(int.Parse(i), conn);

                        var c = new Product()
                        {
                            Name = r["Name"].ToString(),
                            Price = int.Parse(r["Price"].ToString()),
                            Manufacturer = m
                        };

                        ps.Add(c);
                    }
                }
            }
            return ps;
        }
        public List<IProduct> GetProducts()
        {
            var ps = new List<IProduct>();
            using (SQLiteConnection conn = new SQLiteConnection(@$"Data Source={sqlPath}"))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"SELECT * FROM '{Table.Product.ToInt()}';";
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var c = new Product()
                        {
                            Name = r["Name"].ToString(),
                            Price = int.Parse(r["Price"].ToString()),
                            Manufacturer = getAccountingCompanyById(int.Parse(r["ManufacturerID"].ToString()), conn)
                        };
                        var costs = r["CostIDs"].ToString().Split(',');
                        for (int i = 0;i < costs.Length;i++)
                        {
                            c.Costs.Add(getCostProductById(int.Parse(costs[i].ToString()), conn));
                        }
                        
                        ps.Add(c);
                    }
                }
            }
            return ps;
        }
        private IProduct getCostProductById(int id, SQLiteConnection conn)
        {
            var product = new Product();
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"SELECT * FROM '{Table.Cost.ToInt()}' WHERE Id = {id};";
                SQLiteDataReader r = cmd.ExecuteReader();
                if (r.HasRows && r.Read())
                {
                    product.Name = r["Name"].ToString();
                    product.Price = int.Parse(r["Price"].ToString());
                    product.Manufacturer = getAccountingCompanyById(int.Parse(r["ManufacturerID"].ToString()), conn);
                }
            }

            return product;
        }
        private int? getCostProductID(IProduct product, SQLiteConnection conn)
        {
            int? id = null;
            using (var cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"SELECT Id FROM '{Table.Cost.ToInt()}' WHERE Name='{product.Name}' AND Price={product.Price}";
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    id = int.Parse(reader["Id"].ToString());
                }
            }
            return id;
        }
        private int? getProductID(IProduct product, SQLiteConnection conn)
        {
            int? id = null;
            using (var cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"SELECT Id FROM '{Table.Product}' WHERE Name='{product.Name}' AND Price={product.Price}";
                var reader = cmd.ExecuteReader();
                if (reader.FieldCount > 0)
                    id = int.Parse(reader["Id"].ToString());
            }
            return id;
        }
        private AccountCompany getAccountingCompanyById(int id, SQLiteConnection conn)
        {
            AccountCompany ac = null;
            if (id == MyCompanyID)
                return MyCompany;
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"SELECT * FROM '{Table.AC.ToInt()}' WHERE Id={id};";
                SQLiteDataReader r = cmd.ExecuteReader();
                if (r.HasRows && r.Read())
                {
                    ac = new AccountCompany(r["Name"].ToString());
                    ac.Note = r["Note"].ToString();
                    ac.WarningPoint = (Warning)(int.Parse(r["Warning"].ToString()));
                }
            }

            return ac;
        }
        private int? getAccountCompanyID(AccountCompany ac, SQLiteConnection conn)
        {
            int? id = null;
            if (ac == null) return id;
            if (ac.Note == myCompanyNote)
                return MyCompanyID;
            using (var cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"SELECT Id FROM '{Table.AC.ToInt()}' WHERE Name='{ac.Name}' AND Note='{ac.Note}';";
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    id = int.Parse(reader["Id"].ToString());
                }

            }
            return id;
        }
        private int? getAccountCompanyIDByName(string name, SQLiteConnection conn)
        {
            int? id = null;
            using (var cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"SELECT Id FROM '{Table.AC.ToInt()}' WHERE Name='{name}';";
                var reader = cmd.ExecuteReader();
                if (reader.FieldCount > 0)
                {
                    reader.Read();
                    id = int.Parse(reader["Id"].ToString());
                }

            }
            return id;
        }
        private KeyValuePair<string, string> tableElement(string name, string typenetc)
        {
            return new KeyValuePair<string, string>(name, typenetc);
        }
        private string getTableCreationCommand(bool onlyNotExist, bool columnId, string name, KeyValuePair<string, string>[] elements)
        {
            string ifnotexists = "IF NOT EXISTS", id = "'Id' INTEGER PRIMARY KEY AUTOINCREMENT";
            string table = $"CREATE TABLE {(onlyNotExist ? ifnotexists : "")} '{name}'({id}";

            for (int i = 0; i < elements.Length; i++)
                table += $", '{elements[i].Key}' {elements[i].Value}";

            table += ");";
            return table;
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

            return $"INSERT INTO '{table.ToInt()}' ({string.Join(',', names)}) VALUES ({string.Join(',', questionmks)})";
        }
        private string getUpdateCommandByID(Table table, KeyValuePair<string, object>[] sets, int whereId)
        {
            string[] setting = new string[sets.Length];
            for (int i = 0; i < sets.Length; i++)
            {
                setting[i] = $"{sets[i].Key}='{sets[i].Value}'";
            }
            return $"UPDATE {table.ToInt()} SET {string.Join(',', setting)} WHERE Id={whereId}";
        }
        private void createTablesIfNotExists(SQLiteConnection conn)
        {
            using (var command = new SQLiteCommand(conn))
            {
                //Sale table
                command.CommandText = getTableCreationCommand(true, true, Table.Sale.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("DepositDate","TEXT"),
                        tableElement("SellDate", "TEXT"),
                        tableElement("BuyerID", "INTEGER"),
                        tableElement("ProductID", "INTEGER"),
                        tableElement("DiscountRate", "INTEGER"),
                        tableElement("Qty", "INTEGER")
                });
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
                        tableElement("ABN", "INTEGER"),
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
                        tableElement("ManufacturerID", "INTEGER")
                });
                command.ExecuteNonQuery();
                //Cost table
                command.CommandText = getTableCreationCommand(true, true, Table.Cost.ToInt().ToString(), new KeyValuePair<string, string>[]
                {
                        tableElement("Name", "TEXT"),
                        tableElement("Price", "INTEGER"),
                        tableElement("ManufacturerID", "INTEGER")
                });
                command.ExecuteNonQuery();
            }
        }

    }
}
