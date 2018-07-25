using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icepick.CrashReporting
{
	class Settings : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void Notify(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public bool IsDisabled
		{
			get
			{
				return Api.IcepickRegistry.ReadDisableCrashReports();
			}
			set
			{
				Api.IcepickRegistry.WriteDisableCrashReports(value);
				Notify("IsDisabled");
			}
		}
	}
}
