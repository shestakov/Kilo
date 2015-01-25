using System;
using System.IO;
using System.Text;

namespace KeyboardLayoutMonitor
{
	public class Settings
	{
		public string DefaultLayoutName { get; set; }
		public DwmApi.WDM_COLORIZATION_PARAMS DefaultLayoutColorScheme { get; set; }
		public DwmApi.WDM_COLORIZATION_PARAMS AlternativeLayoutColorScheme { get; set; }

		public static Settings CreateDefaultSettings()
		{
			var result = new Settings {DefaultLayoutName = "ENU"};

			var colorizationParams = new DwmApi.WDM_COLORIZATION_PARAMS
			{
				Color1 = 3640655872,
				Color2 = 3640655872,
				Opaque = 1,
				Intensity = 100,
				Unknown1 = 10,
				Unknown2 = 120,
				Unknown3 = 50
			};

			result.DefaultLayoutColorScheme = colorizationParams;

			colorizationParams = new DwmApi.WDM_COLORIZATION_PARAMS
			{
				Color1 = 3640680576,
				Color2 = 3640680576,
				Opaque = 1,
				Intensity = 100,
				Unknown1 = 10,
				Unknown2 = 120,
				Unknown3 = 50
			};

			result.AlternativeLayoutColorScheme = colorizationParams;

			return result;
		}

		public static byte[] Serialize(Settings settings)
		{
			var stream = new MemoryStream();
			var defaultLayoutNameBytes = Encoding.UTF8.GetBytes(settings.DefaultLayoutName);
			stream.Write(BitConverter.GetBytes(defaultLayoutNameBytes.Length), 0, sizeof (int));
			stream.Write(defaultLayoutNameBytes, 0, defaultLayoutNameBytes.Length);
			var buffer = SerializeColorizationParams(settings.DefaultLayoutColorScheme);
			stream.Write(buffer, 0, buffer.Length);
			buffer = SerializeColorizationParams(settings.AlternativeLayoutColorScheme);
			stream.Write(buffer, 0, buffer.Length);
			return stream.ToArray();
		}

		public static Settings Deserialize(byte[] serializedSettings)
		{
			var settings = new Settings();

			using (var reader = new BinaryReader(new MemoryStream(serializedSettings)))
			{
				var defaultLayoutNameLength = reader.ReadInt32();
				var buffer = reader.ReadBytes(defaultLayoutNameLength);
				settings.DefaultLayoutName = Encoding.UTF8.GetString(buffer);
				var colorizationParams = new DwmApi.WDM_COLORIZATION_PARAMS
				{
					Color1 = reader.ReadUInt32(),
					Color2 = reader.ReadUInt32(),
					Opaque = reader.ReadUInt32(),
					Intensity = reader.ReadUInt32(),
					Unknown1 = reader.ReadUInt32(),
					Unknown2 = reader.ReadUInt32(),
					Unknown3 = reader.ReadUInt32()
				};
				settings.DefaultLayoutColorScheme = colorizationParams;

				colorizationParams = new DwmApi.WDM_COLORIZATION_PARAMS
				{
					Color1 = reader.ReadUInt32(),
					Color2 = reader.ReadUInt32(),
					Opaque = reader.ReadUInt32(),
					Intensity = reader.ReadUInt32(),
					Unknown1 = reader.ReadUInt32(),
					Unknown2 = reader.ReadUInt32(),
					Unknown3 = reader.ReadUInt32()
				};
				settings.AlternativeLayoutColorScheme = colorizationParams;
			}

			return settings;
		}

		private static byte[] SerializeColorizationParams(DwmApi.WDM_COLORIZATION_PARAMS colorizationParams)
		{
			var stream = new MemoryStream();
			stream.Write(BitConverter.GetBytes(colorizationParams.Color1), 0, sizeof (uint));
			stream.Write(BitConverter.GetBytes(colorizationParams.Color2), 0, sizeof (uint));
			stream.Write(BitConverter.GetBytes(colorizationParams.Opaque), 0, sizeof (uint));
			stream.Write(BitConverter.GetBytes(colorizationParams.Intensity), 0, sizeof (uint));
			stream.Write(BitConverter.GetBytes(colorizationParams.Unknown1), 0, sizeof (uint));
			stream.Write(BitConverter.GetBytes(colorizationParams.Unknown2), 0, sizeof (uint));
			stream.Write(BitConverter.GetBytes(colorizationParams.Unknown3), 0, sizeof (uint));
			return stream.ToArray();
		}
	}
}