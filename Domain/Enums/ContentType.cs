using System.ComponentModel;

namespace Domain.Enums;

internal enum ContentType
{
    [Description("unknown")]
    Unknown,

    [Description("image/jpeg")]
    ImageJpeg,

    [Description("image/png")]
    ImagePng,

    [Description("image/gif")]
    ImageGif,

    [Description("image/bmp")]
    ImageBmp,

    [Description("video/mp4")]
    VideoMp4,

    [Description("application/octet-stream")]
    ApplicationOctetStream,

    [Description("image/svg+xml")]
    SvgXml,

    [Description("application/pdf")]
    Pdf,

    [Description("text/plain")]
    TextPlain,

    [Description("application/msword")]
    Word,

    [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    WordX,

    [Description("text/csv")]
    Csv
}