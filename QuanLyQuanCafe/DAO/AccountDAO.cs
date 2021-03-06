﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }
            private set { instance = value; }
        }
        private AccountDAO() { }
        MD5 md = MD5.Create();
        public string MaHoaMK(string mk)
        {
            byte[] input = System.Text.Encoding.ASCII.GetBytes(mk);
            byte[] hash = md.ComputeHash(input);
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public bool Login(string userName, string password)
        {
            string query = "EXEC dbo.USP_LOGIN @userName , @password";
            DataTable result = DataProvider.Instance.ExcuteQuery(query, new object[] {userName, MaHoaMK(password)});
            return result.Rows.Count> 0;
        }
        public int LoaiChucVu(string userName)
        {
            string query = "SELECT Type FROM dbo.Account WHERE UserName = N'" + userName+"' ";
            int result = (int)DataProvider.Instance.ExcuteScalar(query);
            return result;
        }
        
        public bool InsertAccount(string userName, string displayName,string password, int type)
        {
            string query = string.Format("INSERT dbo.Account ( UserName , DisplayName ,matKhau, Type ) VALUES ( N'{0}' , N'{1}' , N'{2}', {3} ) ",userName,displayName, MaHoaMK(password), type);
            int result = DataProvider.Instance.ExcuteNonQuery(query);
            return result > 0;
        }
        
        public bool DeteleAccount(int id)
        {
            string query = string.Format("DELETE dbo.Account WHERE id = {0}", id);
            int result = DataProvider.Instance.ExcuteNonQuery(query);
            return result > 0;
        }
        public DataTable loadTaiKhoan()
        {
            string query = "SELECT id , DisplayName , UserName , Type FROM dbo.Account";
            DataTable result = DataProvider.Instance.ExcuteQuery(query);
            return result;

        }
        public int FindAccountByUserName(string name)
        {
            try
            {
                string result = DataProvider.Instance.ExcuteScalar("SELECT id FROM dbo.Account WHERE UserName = N'" + name + "'").ToString();
                return int.Parse(result);
            }
            catch (NullReferenceException)
            {
                return 0;
            }
        }
        public bool SuaTaiKhoan(int id,string DisplayName, string UserName,int loaiChucVu)
        {
            string query = string.Format("UPDATE dbo.Account SET DisplayName = N'{0}' , UserName = N'{1}' , Type = {2} WHERE id = {3}",DisplayName,UserName,loaiChucVu,id);
            int result = DataProvider.Instance.ExcuteNonQuery(query);
            return result >0;
        }
        public bool SuaMatKhau(int id, string password)
        {
            string query = string.Format("UPDATE dbo.Account SET matKhau = N'{0}' WHERE id = {1}", MaHoaMK(password), id);
            int result = DataProvider.Instance.ExcuteNonQuery(query);
            return result > 0;
        }
        public bool XoaTK(int id)
        {
            string query = string.Format("DELETE dbo.Account WHERE id = {0}", id);
            int result = DataProvider.Instance.ExcuteNonQuery(query);
            return result > 0;
        }
        public int nhanID(string tenTK)
        {
            string query = string.Format("SELECT id FROM dbo.Account WHERE UserName = N'"+tenTK+"'");
            int result = int.Parse(DataProvider.Instance.ExcuteScalar(query).ToString());
            return result;
        }
    }
}
