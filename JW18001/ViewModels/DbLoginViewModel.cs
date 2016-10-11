using JW18001.Commands;
using JW18001.Views;
using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace JW18001.ViewModels
{
    internal class DbLoginViewModel : NotificationObject
    {
        private string serverName;  //服务器名称
        private string dbName;     //数据库名称
        private string dbLoginName;//登录名
        private string dbLoginPsd;//登录密码
        private readonly XmlDocument xmlDoc = new XmlDocument();
        private const string XmlPath = @".//Config.xml";
        public static SqlHelper SqlHelper;
        public ICommand DbLoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        private bool isDbLoginFailed = true;

        public string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                RaisePropertyChanged("ServerName");
            }
        }

        public string DbName
        {
            get { return dbName; }
            set
            {
                dbName = value;
                RaisePropertyChanged("DbName");
            }
        }

        public string DbLoginName
        {
            get { return dbLoginName; }
            set
            {
                dbLoginName = value;
                RaisePropertyChanged("DbLoginName");
            }
        }

        public string DbLoginPsd
        {
            get { return dbLoginPsd; }
            set
            {
                dbLoginPsd = value;
                RaisePropertyChanged("DbLoginPsd");
            }
        }

        public bool IsDbLoginFailed
        {
            get { return isDbLoginFailed; }
            set
            {
                isDbLoginFailed = value;
                RaisePropertyChanged("IsDbLoginFailed");
            }
        }

        public DbLoginViewModel()
        {
            LoadXmlDoc();
            DbLoginCommand = new DelegateCommand(DbLogin);
            CloseCommand = new DelegateCommand(Close);
        }

        private static void Close(object obj)
        {
            Application.Current.Shutdown();
        }

        private void SaveXmlDoc()
        {
            string node = "Setting/ServerLogin";
            var selectSingleNode = xmlDoc.SelectSingleNode(node);
            if (selectSingleNode != null)
            {
                XmlNodeList nodeList = selectSingleNode.ChildNodes;
                if (nodeList[0] != null)
                {
                    nodeList[0].InnerText = ServerName;
                }
                if (nodeList[1] != null)
                {
                    nodeList[1].InnerText = DbName;
                }
                if (nodeList[2] != null)
                {
                    nodeList[2].InnerText = DbLoginName;
                }
                if (nodeList[3] != null)
                {
                    nodeList[3].InnerText = DbLoginPsd;
                }
            }
            xmlDoc.Save(XmlPath);
        }

        private void LoadXmlDoc()
        {
            try
            {
                xmlDoc.Load(XmlPath);
                string xpath = "Setting/ServerLogin/ServerName";
                var node = xmlDoc.SelectSingleNode(xpath);
                if (node != null)
                {
                    ServerName = node.InnerText.Trim();
                }
                xpath = "Setting/ServerLogin/DbName";
                node = xmlDoc.SelectSingleNode(xpath);
                if (node != null)
                {
                    DbName = node.InnerText.Trim();
                }
                xpath = "Setting/ServerLogin/LoginName";
                node = xmlDoc.SelectSingleNode(xpath);
                if (node != null)
                {
                    DbLoginName = node.InnerText.Trim();
                }
                xpath = "Setting/ServerLogin/LoginPsd";
                node = xmlDoc.SelectSingleNode(xpath);
                if (node != null)
                {
                    DbLoginPsd = node.InnerText.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DbLogin(object obj)
        {
            SqlHelper = new SqlHelper(ServerName, DbName, DbLoginName, DbLoginPsd);
            SaveXmlDoc();
            if (SqlHelper.IsConnect)
            {
                IsDbLoginFailed = false;
                BasicInfoView view = new BasicInfoView();
                view.ShowDialog();
            }
        }
    }
}