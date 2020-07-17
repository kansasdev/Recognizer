using System.Runtime.Serialization;

namespace Recognizer.OCR
{
    public enum RecognitionUnitKind
    {
        [EnumMember(Value = "writingRegion")]
        WritingRegion,

        [EnumMember(Value = "paragraph")]
        Paragraph,

        [EnumMember(Value = "line")]
        Line,

        [EnumMember(Value = "inkWord")]
        InkWord,

        [EnumMember(Value = "inkDrawing")]
        InkDrawing,

        [EnumMember(Value = "listItem")]
        ListItem,

        [EnumMember(Value = "inkBullet")]
        InkBullet,

        [EnumMember(Value = "unknown")]
        Unknown
    }
}
