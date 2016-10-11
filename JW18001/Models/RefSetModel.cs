using JW18001.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace JW18001.Models
{
    internal class RefSetModel : NotificationObject
    {
        private string channel;
        private string wave1Ref;
        private string wave2Ref;
        private string wave3Ref;
        private string wave4Ref;
        private string wave5Ref;
        private string wave6Ref;
        private string time;
        private string startRef;

        public string Channel
        {
            get { return channel; }
            set
            {
                channel = value;
                RaisePropertyChanged("Channel");
            }
        }

        public string Wave1Ref
        {
            get { return wave1Ref; }
            set
            {
                wave1Ref = value;
                RaisePropertyChanged("Wave1Ref");
            }
        }

        public string Wave2Ref
        {
            get { return wave2Ref; }
            set
            {
                wave2Ref = value;
                RaisePropertyChanged("Wave2Ref");
            }
        }

        public string Wave3Ref
        {
            get { return wave3Ref; }
            set
            {
                wave3Ref = value;
                RaisePropertyChanged("Wave3Ref");
            }
        }

        public string Wave4Ref
        {
            get { return wave4Ref; }
            set
            {
                wave4Ref = value;
                RaisePropertyChanged("Wave4Ref");
            }
        }

        public string Wave5Ref
        {
            get { return wave5Ref; }
            set
            {
                wave5Ref = value;
                RaisePropertyChanged("Wave5Ref");
            }
        }

        public string Wave6Ref
        {
            get { return wave6Ref; }
            set
            {
                wave6Ref = value;
                RaisePropertyChanged("Wave6Ref");
            }
        }

        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                RaisePropertyChanged("Time");
            }
        }

        public string StartRef
        {
            get { return startRef; }
            set
            {
                startRef = value;
                RaisePropertyChanged("StartRef");
            }
        }
    }
}