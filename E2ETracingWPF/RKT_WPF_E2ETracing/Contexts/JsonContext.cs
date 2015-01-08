using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace RKT_WPF_E2ETracing.Contexts
{
    public class JsonContext : INotifyPropertyChanged
    {
        public JsonContext(string airlineID, string flightNumber, DateTime flightDate, decimal price, string currency, string airport, string weather, string temp, string wind)
        {
            this.AirlineID = airlineID;
            this.FlightNumber = flightNumber;
            this.FlightDate = flightDate;
            this.Price = price;
            this.Currency = currency;
            this.DestinationAirport = airport;
            this.Weather = weather;
            this.Temperature = temp;
            this.Wind = wind;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string airlineID;
        public string AirlineID
        {
            get
            {
                return this.airlineID;
            }

            set
            {
                this.airlineID = value;
                this.NotifyPropertyChanged();
            }
        }

        private string flightNumber;
        public string FlightNumber
        {
            get
            {
                return this.flightNumber;
            }

            set
            {
                this.flightNumber = value;
                this.NotifyPropertyChanged();
            }
        }

        private DateTime flightDate;
        public DateTime FlightDate
        {
            get
            {
                return this.flightDate;
            }

            set
            {
                this.flightDate = value;
                this.NotifyPropertyChanged();
            }
        }

        private decimal price;
        public decimal Price
        {
            get
            {
                return this.price;
            }

            set
            {
                this.price = value;
                this.NotifyPropertyChanged();
            }
        }

        private string currency;
        public string Currency
        {
            get
            {
                return this.currency;
            }

            set
            {
                this.currency = value;
                this.NotifyPropertyChanged();
            }
        }

        private string destinationAirport;
        public string DestinationAirport
        {
            get
            {
                return this.destinationAirport;
            }

            set
            {
                this.destinationAirport = value;
                this.NotifyPropertyChanged();
            }
        }

        private string weather;
        public string Weather
        {
            get
            {
                return this.weather;
            }

            set
            {
                this.weather = value;
                this.NotifyPropertyChanged();
            }
        }

        private string temperature;
        public string Temperature
        {
            get
            {
                return this.temperature;
            }

            set
            {
                this.temperature = value;
                this.NotifyPropertyChanged();
            }
        }

        private string wind;
        public string Wind
        {
            get
            {
                return this.wind;
            }

            set
            {
                this.wind = value;
                this.NotifyPropertyChanged();
            }
        }

        private Visibility ringVisible = Visibility.Collapsed;
        public Visibility RingVisible
        {
            get
            {
                return this.ringVisible;
            }

            set
            {
                this.ringVisible = value;
                this.NotifyPropertyChanged();
            }
        }

        public void SetRingVisibility(Visibility visibility)
        {
            this.RingVisible = visibility;
        }
    }
}
