using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AirMonitor.Views.ViewModels
{
    class DetailsViewModel : INotifyPropertyChanged
    {
        private int mark = 50;
        public int Mark 
        {
            get 
            {
                return mark;
            }
            set 
            {
                if(Mark != value)
                {
                    Mark = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Mark"));
                    }
                }
            } 
        }

        private int cAQI = 50;
        public int CAQI
        {
            get
            {
                return mark;
            }
            set
            {
                if (cAQI != value)
                {
                    cAQI = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("CAQI"));
                    }
                }
            }
        }

        private int pressure = 1026;
        public int Pressure
        {
            get
            {
                return pressure;
            }
            set
            {
                if (pressure != value)
                {
                    cAQI = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Pressure"));
                    }
                }
            }
        }

        private float humility = 0.95f;
        public float Humility 
        {
            get
            {
                return humility;
            }
            set
            {
                if (humility != value)
                {
                    humility = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Humility"));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
