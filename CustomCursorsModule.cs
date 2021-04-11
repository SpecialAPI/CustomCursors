using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CursorAPI;
using Ionic.Zip;
using UnityEngine;

namespace CustomCursors
{
    public class CustomCursorsModule : ETGModule
    {
        public override void Init()
        {
        }

        public override void Start()
        {
            CursorMaker.Init();
            if(!Directory.Exists(CCDPath))
            {
                Directory.CreateDirectory(CCDPath);
            }
            HandleFolder(CCDPath);
            ETGModConsole.Log("Custom Cursors Mod initialized.");
        }

        public static void HandleFolder(string path)
        {
            foreach(string file in Directory.GetFiles(path))
            {
                if (file.EndsWith(".zip"))
                {
                    using(ZipFile zip = ZipFile.Read(file))
                    {
                        HandleZip(zip);
                    }
                }
                else if (file.EndsWith(".png"))
                {
                    Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    using (Stream stream = File.OpenRead(file))
                    {
                        if (stream != null)
                        {
                            byte[] bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, bytes.Length);
                            tex.LoadImage(bytes);
                        }
                    }
                    tex.name = file.Replace(".png", "");
                    tex.filterMode = FilterMode.Point;
                    CursorMaker.BuildCursor(tex);
                }
            }
            foreach(string folder in Directory.GetDirectories(path))
            {
                HandleFolder(folder);
            }
        }

        public static void HandleZip(ZipFile zip)
        {
            foreach(ZipEntry entry in zip.Entries)
            {
                if (!entry.IsDirectory)
                {
                    if (entry.FileName.EndsWith(".png"))
                    {
                        Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                        using (MemoryStream memoryStream2 = new MemoryStream())
                        {
                            entry.Extract(memoryStream2);
                            memoryStream2.Seek(0L, SeekOrigin.Begin);
                            tex.LoadImage(memoryStream2.GetBuffer());
                        }
                        tex.name = entry.FileName.Replace(".png", "");
                        tex.filterMode = FilterMode.Point;
                        CursorMaker.BuildCursor(tex);
                    }
                    else if (entry.FileName.EndsWith(".zip"))
                    {
                        using (MemoryStream memoryStream2 = new MemoryStream())
                        {
                            entry.Extract(memoryStream2);
                            memoryStream2.Seek(0L, SeekOrigin.Begin); 
                            using (ZipFile subzip = ZipFile.Read(memoryStream2))
                            {
                                HandleZip(subzip);
                            }
                        }
                    }
                }
            }
        }

        public override void Exit()
        {
        }

        public static string CCDPath = Path.Combine(ETGMod.GameFolder, "CustomCursorData");
    }
}
