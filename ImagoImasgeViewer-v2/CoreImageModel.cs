using System;
using System.Drawing;


namespace ImagoImasgeViewer_v2
{
    public class CoreImageModel 
    {
        public string imageLabel { get; set; }
        public string imagePath { get; set; }
        public Image imageShow { get; set; }
        public uint numberOfImages { get; set; }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public CoreImageModel()
        {
            this.imageLabel = "Not Set";
            this.imagePath = "Not Set";
            this.imageShow = null;
            this.numberOfImages = 0;
        }
    }
}
