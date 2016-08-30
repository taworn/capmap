using System.IO;

namespace capmap {
    public class Map {

        private int width;
        private int height;
        private int[] blockMap;
        private int[] imageMap;

        /// <summary>
        /// Constructs a map.
        /// </summary>
        public Map(int w, int h) {
            width = w;
            height = h;
            blockMap = new int[width * height];
            imageMap = new int[width * height];
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
                if (w != 16 && h != 16)
                    throw new IOException("Width and Height must be 16 for now.");
                int dw = reader.ReadInt32();
                int dh = reader.ReadInt32();
                int pw = reader.ReadInt32();
                int ph = reader.ReadInt32();

                // reads block map and image map data
                int size = width * height;
                int[] blockMap = new int[size];
                int[] imageMap = new int[size];
                for (int i = 0; i < size; i++)
                    blockMap[i] = reader.ReadInt32();
                for (int i = 0; i < size; i++)
                    imageMap[i] = reader.ReadInt32();

                // copying data
                this.width = w;
                this.height = h;
                //this. = dw;
                //this. = dh;
                //this. = pw;
                //this. = ph;
                this.blockMap = blockMap;
                this.imageMap = imageMap;
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
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);

                // writes block map and image map data
                int size = width * height;
                for (int i = 0; i < size; i++)
                    writer.Write(blockMap[i]);
                for (int i = 0; i < size; i++)
                    writer.Write(imageMap[i]);
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

        public int Get(int index) {
            return blockMap[index];
        }

        public void Set(int index, int data) {
            blockMap[index] = data;
            imageMap[index] = data;
        }

    }
}
