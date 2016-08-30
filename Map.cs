using System;
using System.Collections.Generic;

namespace capmap {
    public class Map {

        private int width;
        private int height;
        private int[] blockMap;
        private int[] imageMap;

        /// <summary>
        /// Constructs a map.
        /// </summary>
        public Map() {
            width = 0;
            height = 0;
            blockMap = null;
            imageMap = null;
        }

        /// <summary>
        /// Opens a map file.
        /// </summary>
        /// <param name="fileName">Filename to open.</param>
        public void open(string fileName) {

        }

        /// <summary>
        /// Saves a map file.
        /// </summary>
        /// <param name="fileName">Filename to save.</param>
        public void save(string fileName) {

        }

        public bool isOpened() {
            return imageMap != null;
        }

    }
}
