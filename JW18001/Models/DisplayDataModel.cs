using JW18001.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JW18001.Models
{
    internal class DisplayDataModel : NotificationObject
    {
        private string channel;
        private string opmWave;
        private string db;
        private string dbm;

        public string Channel
        {
            get { return channel; }
            set
            {
                channel = value;
                RaisePropertyChanged("Channel");
            }
        }

        public string OpmWave
        {
            get { return opmWave; }
            set
            {
                opmWave = value;
                RaisePropertyChanged("OpmWave");
            }
        }

        public string Db
        {
            get { return db; }
            set
            {
                db = value;
                RaisePropertyChanged("Db");
            }
        }

        public string Dbm
        {
            get { return dbm; }
            set
            {
                dbm = value;
                RaisePropertyChanged("Dbm");
            }
        }
    }
}