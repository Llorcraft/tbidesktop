using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbiDesktop.Models
{
    public class Document
    {
        public string folder { get; set; }
        public string file { get; set; }
        public double size { get; set; }
        public string extension
        {
            get
            {
                try
                {
                    return file.Substring(this.file.LastIndexOf(".") + 1, this.file.Length - 1).ToLower();
                }
                catch
                {
                    return "unknown";
                }
            }
        }
        public string mime
        {
            get
            {
                switch (this.extension)
                {
                    case "pdf": return "application/pdf";
                    case "doc": return "application/msword";
                    case "docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    case "xls": return "application/vnd.ms-excel";
                    case "xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    case "ppt": return "application/vnd.ms-powerpoint";
                    case "pps": return "application/vnd.ms-powerpoint";
                    case "pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    case "ppsx": return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                    case "jpg": return "image/jpeg";
                    case "bmp": return "image/bmp";
                    case "png": return "image/png";
                    case "txt": return "text/*";
                    default: return "application/octet-stream";
                }
            }
        }
    }
}
