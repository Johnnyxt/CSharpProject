using JW18001.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JW18001.Models
{
    internal class TestItemModel : NotificationObject
    {
        private string channel;
        private string ilValue;
        private string pdlValue;
        private string rlValue;

        public string Channel
        {
            get { return channel; }
            set
            {
                channel = value;
                RaisePropertyChanged("Channel");
            }
        }

        public string IlValue
        {
            get { return ilValue; }
            set
            {
                ilValue = value;
                RaisePropertyChanged("IlValue");
            }
        }

        public string PdlValue
        {
            get { return pdlValue; }
            set
            {
                pdlValue = value;
                RaisePropertyChanged("PdlValue");
            }
        }

        public string RlValue
        {
            get { return rlValue; }
            set
            {
                rlValue = value;
                RaisePropertyChanged("RlValue");
            }
        }
    }
}