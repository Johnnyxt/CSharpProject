using JW18001.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO.Packaging;
using System.Linq;
using System.Text;

namespace JW18001.Models
{
    internal class TestDataDetailModel : NotificationObject
    {
        private string userItem;
        private string userInfo;
        private string channel;
        private string wave1Il;
        private string wave1Pdl;
        private string wave1Rl;
        private string wave1TestTime;

        private string wave2Il;
        private string wave2Pdl;
        private string wave2Rl;
        private string wave2TestTime;

        private string wave3Il;
        private string wave3Pdl;
        private string wave3Rl;
        private string wave3TestTime;

        private string wave4Il;
        private string wave4Pdl;
        private string wave4Rl;
        private string wave4TestTime;

        public string UserItem
        {
            get { return userItem; }
            set
            {
                userItem = value;
                RaisePropertyChanged("UserItem");
            }
        }

        public string UserInfo
        {
            get { return userInfo; }
            set
            {
                userInfo = value;
                RaisePropertyChanged("UserInfo");
            }
        }

        public string Channel
        {
            get { return channel; }
            set
            {
                channel = value;
                RaisePropertyChanged("Channel");
            }
        }

        public string Wave1Il
        {
            get { return wave1Il; }
            set
            {
                wave1Il = value;
                RaisePropertyChanged("Wave1Il");
            }
        }

        public string Wave1Pdl
        {
            get { return wave1Pdl; }
            set
            {
                wave1Pdl = value;
                RaisePropertyChanged("Wave1Pdl");
            }
        }

        public string Wave1Rl
        {
            get { return wave1Rl; }
            set
            {
                wave1Rl = value;
                RaisePropertyChanged("Wave1Rl");
            }
        }

        public string Wave2Il
        {
            get { return wave2Il; }
            set
            {
                wave2Il = value;
                RaisePropertyChanged("Wave2Il");
            }
        }

        public string Wave2Pdl
        {
            get { return wave2Pdl; }
            set
            {
                wave2Pdl = value;
                RaisePropertyChanged("Wave2Pdl");
            }
        }

        public string Wave2Rl
        {
            get { return wave2Rl; }
            set
            {
                wave2Rl = value;
                RaisePropertyChanged("Wave2Rl");
            }
        }

        public string Wave3Il
        {
            get { return wave3Il; }
            set
            {
                wave3Il = value;
                RaisePropertyChanged("Wave3Il");
            }
        }

        public string Wave3Pdl
        {
            get { return wave3Pdl; }
            set
            {
                wave3Pdl = value;
                RaisePropertyChanged("Wave3Pdl");
            }
        }

        public string Wave3Rl
        {
            get { return wave3Rl; }
            set
            {
                wave3Rl = value;
                RaisePropertyChanged("Wave3Rl");
            }
        }

        public string Wave4Il
        {
            get { return wave4Il; }
            set
            {
                wave4Il = value;
                RaisePropertyChanged("Wave4Il");
            }
        }

        public string Wave4Pdl
        {
            get { return wave4Pdl; }
            set
            {
                wave4Pdl = value;
                RaisePropertyChanged("Wave4Pdl");
            }
        }

        public string Wave4Rl
        {
            get { return wave4Rl; }
            set
            {
                wave4Rl = value;
                RaisePropertyChanged("Wave4Rl");
            }
        }

        public string Wave1TestTime
        {
            get { return wave1TestTime; }
            set
            {
                wave1TestTime = value;
                RaisePropertyChanged("Wave1TestTime");
            }
        }

        public string Wave2TestTime
        {
            get { return wave2TestTime; }
            set
            {
                wave2TestTime = value;
                RaisePropertyChanged("Wave2TestTime");
            }
        }

        public string Wave3TestTime
        {
            get { return wave3TestTime; }
            set
            {
                wave3TestTime = value;
                RaisePropertyChanged("Wave3TestTime");
            }
        }

        public string Wave4TestTime
        {
            get { return wave4TestTime; }
            set
            {
                wave4TestTime = value;
                RaisePropertyChanged("Wave4TestTime");
            }
        }
    }
}