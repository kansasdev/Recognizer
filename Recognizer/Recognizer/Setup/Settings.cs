using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recognizer.Setup
{
	public static class Settings
	{
		private static ISettings AppSettings => CrossSettings.Current;

		public static string KeyOcrSetting
		{
			get => AppSettings.GetValueOrDefault(nameof(KeyOcrSetting), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(KeyOcrSetting), value);
		}
		public static string KeySpeechSetting
		{
			get => AppSettings.GetValueOrDefault(nameof(KeySpeechSetting), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(KeySpeechSetting), value);
		}

		public static string LanguageSetting
		{
			get => AppSettings.GetValueOrDefault(nameof(LanguageSetting), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(LanguageSetting), value);
		}

		public static string RegionSetting
		{
			get => AppSettings.GetValueOrDefault(nameof(RegionSetting), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(RegionSetting), value);
		}

		public static string EndpointSetting
		{
			get => AppSettings.GetValueOrDefault(nameof(EndpointSetting), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(EndpointSetting), value);
		}

		public static bool NoSetupDefined { get; set; }

	}
}
