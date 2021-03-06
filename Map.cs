﻿using System.IO;

namespace capmap {
    public class Map {

        private int width;
        private int height;
        private int startDivoX;
        private int startDivoY;
        private int startPacmanX;
        private int startPacmanY;
        private byte[] mapData;
        private int[] imageData;
        private int[] blockData;

        /// <summary>
        /// Constructs a map.
        /// </summary>
        public Map(int w, int h) {
            width = w;
            height = h;
            startDivoX = -1;
            startDivoY = -1;
            startPacmanX = -1;
            startPacmanY = -1;
            mapData = new byte[width * height];
            imageData = new int[width * height];
            blockData = new int[width * height];

            for (var i = 0; i < width * height; i++)
                Set(i, 0);
        }

        /// <summary>
        /// Opens a map file.
        /// </summary>
        /// <param name="fileName">Filename to open.</param>
        public void Open(string fileName) {
            var stream = new FileStream(fileName, FileMode.Open);
            var reader = new BinaryReader(stream);
            try {
                // reads header, 8 bytes
                // header=CAPMAP##, # = ASCII 0
                byte[] check = { 0x43, 0x41, 0x50, 0x4D, 0x41, 0x50, 0x00, 0x00, };
                var header = reader.ReadBytes(8);
                for (int i = 0; i < header.Length; i++)
                    if (header[i] != check[i])
                        throw new IOException("Header is not acceptable.");

                // reads header information
                // 6 int = width, height, divo start width, divo start height, pacman start width, pacman start height
                int w = reader.ReadInt32();
                int h = reader.ReadInt32();
                int dx = reader.ReadInt32();
                int dy = reader.ReadInt32();
                int px = reader.ReadInt32();
                int py = reader.ReadInt32();
                int size = w * h;

                // mapData and imageData are for game, not used map editor, just copying
                byte[] mapData = new byte[size];
                for (int i = 0; i < size; i++)
                    mapData[i] = reader.ReadByte();
                int[] imageData = new int[size];
                for (int i = 0; i < size; i++)
                    imageData[i] = reader.ReadInt32();

                // reads block data
                int[] blockData = new int[size];
                for (int i = 0; i < size; i++)
                    blockData[i] = reader.ReadInt32();

                // copying data
                this.width = w;
                this.height = h;
                this.startDivoX = dx;
                this.startDivoY = dy;
                this.startPacmanX = px;
                this.startPacmanY = py;
                this.mapData = mapData;
                this.imageData = imageData;
                this.blockData = blockData;
            }
            finally {
                reader.Close();
                stream.Close();
            }
        }

        /// <summary>
        /// Saves a map file.
        /// </summary>
        /// <param name="fileName">Filename to save.</param>
        public void Save(string fileName) {
            var stream = new FileStream(fileName, FileMode.Create);
            var writer = new BinaryWriter(stream);
            try {
                // writes header, 8 bytes
                // header=CAPMAP##, # = ASCII 0
                byte[] check = { 0x43, 0x41, 0x50, 0x4D, 0x41, 0x50, 0x00, 0x00, };
                writer.Write(check);

                // writes header information
                // 6 int = width, height, divo start width, divo start height, pacman start width, pacman start height
                writer.Write(width);
                writer.Write(height);
                writer.Write(startDivoX);
                writer.Write(startDivoY);
                writer.Write(startPacmanX);
                writer.Write(startPacmanY);
                int size = width * height;

                // writes map data and image data for game
                for (int i = 0; i < size; i++)
                    writer.Write(mapData[i]);
                for (int i = 0; i < size; i++)
                    writer.Write(imageData[i]);

                // writes block data
                for (int i = 0; i < size; i++)
                    writer.Write(blockData[i]);
            }
            finally {
                writer.Close();
                stream.Close();
            }
        }

        public void ExportJavaScript(string fileName) {
            var stream = new FileStream(fileName, FileMode.Create);
            var writer = new StreamWriter(stream);
            try {
                var name = Path.GetFileNameWithoutExtension(fileName);
                name = name.Replace('.', '_');
                name += "MapResource";
                writer.WriteLine("// This file is generated.");
                writer.WriteLine("function " + name + "() {");
                writer.WriteLine("    return {");

                writer.WriteLine("        width: " + width + ",");
                writer.WriteLine("        height: " + height + ",");
                writer.WriteLine("        startDivoX: " + startDivoX + ",");
                writer.WriteLine("        startDivoY: " + startDivoY + ",");
                writer.WriteLine("        startPacmanX: " + startPacmanX + ",");
                writer.WriteLine("        startPacmanY: " + startPacmanY + ",");

                writer.WriteLine("        mapData: [");
                for (int j = 0; j < height; j++) {
                    var line = "            ";
                    for (int i = 0; i < width; i++) {
                        var s = mapData[j * width + i].ToString("X2");
                        line += "0x" + s + ", ";
                    }
                    writer.WriteLine(line);
                }
                writer.WriteLine("        ],");

                writer.WriteLine("        imageData: [");
                for (int j = 0; j < height; j++) {
                    var line = "            ";
                    for (int i = 0; i < width; i++) {
                        var s = imageData[j * width + i].ToString("X2");
                        line += "0x" + s + ", ";
                    }
                    writer.WriteLine(line);
                }
                writer.WriteLine("        ],");

                writer.WriteLine("    };");
                writer.WriteLine("}");
            }
            finally {
                writer.Close();
                stream.Close();
            }
        }

        public int GetWidth() {
            return width;
        }

        public int GetHeight() {
            return height;
        }

        public int GetDivoStartX() {
            return startDivoX;
        }

        public int GetDivoStartY() {
            return startDivoY;
        }

        public void SetDivoStart(int x, int y) {
            startDivoX = x;
            startDivoY = y;
        }

        public int GetPacmanStartX() {
            return startPacmanX;
        }

        public int GetPacmanStartY() {
            return startPacmanY;
        }

        public void SetPacmanStart(int x, int y) {
            startPacmanX = x;
            startPacmanY = y;
        }

        public int Get(int index) {
            return blockData[index];
        }

        public void Set(int index, int data) {
            blockData[index] = data;

            // translate block to map and image
            if (data == 0) {
                // blocking
                mapData[index] = 0x01;
                imageData[index] = 3;
            }
            else if (data == 1) {
                // movable
                mapData[index] = 0x00;
                imageData[index] = 0;
            }
            else if (data == 2) {
                // coke
                mapData[index] = 0x10;
                imageData[index] = 1;
            }
            else if (data == 3) {
                // bread
                mapData[index] = 0x10;
                imageData[index] = 2;
            }
            else if (data == 4 || data == 5) {
                // start point, just show movable
                mapData[index] = 0x00;
                imageData[index] = 0;
            }
            else {
                // treat like blocking
                mapData[index] = 0x01;
                imageData[index] = 3;
            }
        }

    }
}
