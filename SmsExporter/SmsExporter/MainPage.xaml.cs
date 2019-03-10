using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmsExporter
{
    public partial class MainPage : ContentPage
    {
        public MainPage(ISmsReader reader)
        {
            InitializeComponent();
            this.BindingContext = new MainPageModel(reader);
        }
    }
}
