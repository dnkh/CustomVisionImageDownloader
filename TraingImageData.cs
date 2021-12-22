namespace CustomVisionImageDownloader
{
    public class TraingImageData
    {
        public string id { get; set; }
        public DateTime created { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string resizedImageUri { get; set; }
        public string thumbnailUri { get; set; }
        public string originalImageUri { get; set; }
        
    }
}