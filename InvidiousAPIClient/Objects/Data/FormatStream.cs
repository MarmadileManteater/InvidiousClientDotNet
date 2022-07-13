using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Extensions;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class FormatStream
    {
        private readonly JObject _data;
        internal FormatStream(JObject stream)
        {
            _data = stream;
        }
        public virtual string Init
        {
            get
            {
                string? result = _data["init"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Index
        {
            get
            {
                string? result = _data["index"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Bitrate
        {
            get
            {
                string? result = _data["bitrate"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Url
        {
            get
            {
                string? result = _data["url"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Itag
        {
            get
            {
                string? result = _data["itag"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Type
        {
            get
            {
                string? result = _data["type"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual int Fps
        {
            get
            {
                int? result = _data["fps"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }

        public virtual string Container
        {
            get
            {
                string? result = _data["container"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }

        public virtual string Encoding
        {
            get
            {
                string? result = _data["encoding"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Resolution
        {
            get
            {
                string? result = _data["resolution"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string QualityLabel
        {
            get
            {
                string? result = _data["qualityLabel"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Size
        {
            get
            {
                string? result = _data["size"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Clen
        {
            get
            {
                string? result = _data["clen"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Lmt
        {
            get
            {
                string? result = _data["lmt"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string ProjectionType
        {
            get
            {
                string? result = _data["projectionType"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual Dictionary<string, string> ColorInfo
        {
            get
            {
                JObject? result = _data["colorInfo"]?.Value<JObject>();
                if (result != null)
                {
                    return result.ToDictionaryWithClass<string>();
                }
                return new Dictionary<string, string>();
            }
        }
    }
}
