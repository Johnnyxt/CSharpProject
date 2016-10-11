using JW18001.ViewModels;

namespace JW18001.Models
{
    internal class ThreSetModel : NotificationObject
    {
        private string wave;
        private float ilLowerLimit;
        private float ilUpperLimit;
        private float pdlLowerLimit;
        private float pdlUpperLimit;
        private float rlLowerLimit;
        private float rlUpperLimit;

        public string Wave
        {
            get { return wave; }
            set
            {
                wave = value;
                RaisePropertyChanged("Wave");
            }
        }

        public float IlLowerLimit
        {
            get { return ilLowerLimit; }
            set
            {
                ilLowerLimit = value;
                RaisePropertyChanged("IlLowerLimit");
            }
        }

        public float IlUpperLimit
        {
            get { return ilUpperLimit; }
            set
            {
                ilUpperLimit = value;
                RaisePropertyChanged("IlUpperLimit");
            }
        }

        public float RlLowerLimit
        {
            get { return rlLowerLimit; }
            set
            {
                rlLowerLimit = value;
                RaisePropertyChanged("RlLowerLimit");
            }
        }

        public float RlUpperLimit
        {
            get { return rlUpperLimit; }
            set
            {
                rlUpperLimit = value;
                RaisePropertyChanged("RlUpperLimit");
            }
        }

        public float PdlLowerLimit
        {
            get { return pdlLowerLimit; }
            set
            {
                pdlLowerLimit = value;
                RaisePropertyChanged("PdlLowerLimit");
            }
        }

        public float PdlUpperLimit
        {
            get { return pdlUpperLimit; }
            set
            {
                pdlUpperLimit = value;
                RaisePropertyChanged("PdlUpperLimit");
            }
        }
    }
}