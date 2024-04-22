using Godot;
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using File = Godot.File;
using Content.Beatmaps;
using System.Runtime.Remoting.Messaging;

namespace Compatibility.SSP
{
	public static class Map
	{
		public static int GetVersion(string path)
		{
			var file = new File();
			var err = file.Open(path, File.ModeFlags.Read);
			if (err != Error.Ok)
				return -1;
			var header = file.GetBuffer(4);
			if (header.GetStringFromASCII() != "SS+m")
				return -1;
			if (file.Get16() > 1) return 1;
			
			return -1;
		}

		public static bool Import(BeatmapToConvert toConvert) 
		{
			return Import(toConvert.Path);
		}

		public static bool Import(string path)
		{
			var file = new File();
			var err = file.Open(path, File.ModeFlags.Read);
			if (err != Error.Ok)
				return false;
			var header = file.GetBuffer(4);
			if (header.GetStringFromASCII() != "SS+m")
				return false;
			var version = file.Get16();
			GD.Print(path + " " + version.ToString());
			if (version != 2) {
				GD.PrintErr("Invalid map version, only sspmv2 is supported for now.");
				return false;
			}

			file.GetBuffer(24); //empty space

			var md5 = file.GetMd5(path);
			var importPath = Global.MapPath.PlusFile($"sspm_{md5}.flux");
			GD.Print("Converting map => " + importPath);
			using (var stream = new FileStream(importPath, FileMode.CreateNew))
			{
				using (var zip = new ZipArchive(stream, ZipArchiveMode.Create))
				{
					file.Get32(); // skip over map length
					var noteCount = file.Get32();
					GD.Print(noteCount.ToString());

					file.Seek(0x2d);
					if(file.Get8() != 1)
					{
						GD.PrintErr("NO AUDIO");
						return false;
					}

					if(file.Get8() == 1)
					{
						var imageEntry = zip.CreateEntry("cover.png");
						var imageStream = imageEntry.Open();
					
						file.Seek(0x50);
						var coverOffset = (long)file.Get64();
						var coverLength = (long)file.Get64();

						file.Seek(coverOffset);
						imageStream.Write(file.GetBuffer(coverLength), 0, (int)coverLength);
						imageStream.Flush();
						imageStream.Close();
					}

					file.Seek(0x30); // seek to where offsets were stored

					var customDataOffset = file.Get64();
					var customDataByteLength = file.Get64();

					var audioByteOffset = file.Get64();
					var audioByteLength = file.Get64();

					file.Seek(0x70); // seek to where markerByteOffset is stored
					var markerByteOffset = file.Get64();
					file.Seek(0x80); // seek to where metadata is stored
					file.GetBuffer(file.Get16()); // skip over id

					var metaEntry = zip.CreateEntry("meta.json");
					var metaJson = new JObject();
	
					metaJson.Add("_version", 1);
					metaJson.Add("_title", file.GetBuffer(file.Get16()).GetStringFromUTF8());

					file.GetBuffer(file.Get16()); // skip over song name

					var mappers = "";
					for(int mapperId = 0; mapperId < file.Get16(); mapperId++)
					{
						int mapperLen = file.Get16();
						if(mapperId != 0)
						{
							mappers += " & ";
							file.Seek((long)file.GetPosition()-2);
						}
						var mapper = file.GetBuffer(mapperLen).GetStringFromUTF8();
						mappers += mapper;
					}

					metaJson.Add("_mappers", new JArray(mappers));
					metaJson.Add("_difficulties", new JArray("extracted.json"));
					metaJson.Add("_music", "music.bin");
					var metaBytes = System.Text.Encoding.ASCII.GetBytes(metaJson.ToString());
					var metaStream = metaEntry.Open();
					metaStream.Write(metaBytes, 0, metaBytes.Length);
					metaStream.Flush();
					metaStream.Close();

					file.Seek((long)markerByteOffset);

					var notesEntry = zip.CreateEntry("extracted.json");

					var notesJson = new JObject();
					notesJson.Add("_version", 1);
					notesJson.Add("_name", "Sound Space Plus");

					var notes = new JArray();

					for(int i = 0; i < noteCount; i++)
					{
						var note = new JObject();

						note.Add("_time", (float)file.Get32() / 1000f);

						file.Get8(); // always 1, this is also stupid :D

						var hasQuantum = file.Get8() == 1;
						if(hasQuantum)
						{
							note.Add("_x", -(file.GetFloat() - 1));
							note.Add("_y", -(file.GetFloat() - 1));
						}
						else 
						{
							note.Add("_x", -(file.Get8() - 1));
							note.Add("_y", -(file.Get8() - 1));
						}
						
						notes.Add(note);
					}

					notesJson.Add("_notes", notes);
					var notesBytes = System.Text.Encoding.ASCII.GetBytes(notesJson.ToString());
					var notesStream = notesEntry.Open();
					notesStream.Write(notesBytes, 0, notesBytes.Length);
					notesStream.Flush();
					notesStream.Close();

					file.Seek((long)audioByteOffset);
					var musicEntry = zip.CreateEntry("music.bin");
					var musicBuffer = file.GetBuffer((long)audioByteLength);
					var musicStream = musicEntry.Open();
					musicStream.Write(musicBuffer, 0, (int)audioByteLength);
					musicStream.Flush();
					musicStream.Close();
				}
			}
			file.Close();
			return true;
		}
	}
}
