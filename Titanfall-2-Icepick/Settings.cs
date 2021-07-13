using System.ComponentModel;

namespace Icepick
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

		public bool CrashReportingDisabled
		{
			get
			{
				return Api.IcepickRegistry.ReadDisableCrashReports();
			}
			set
			{
				Api.IcepickRegistry.WriteDisableCrashReports(value);
				Notify("CrashReportingDisabled");
			}
		}

		public bool DeveloperModeEnabled
		{
			get
			{
				return Api.IcepickRegistry.ReadEnableDeveloperMode();
			}
			set
			{
				Api.IcepickRegistry.WriteEnableDeveloperMode(value);
				Notify("DeveloperModeEnabled");
			}
		}
	}
}
