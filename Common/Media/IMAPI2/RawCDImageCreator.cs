using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Macro.Common.Media.IMAPI2
{
    [ComImport]
    [Guid("25983550-9D65-49CE-B335-40630D901227")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IRawCDImageCreator
    {
        // Creates the result stream
        [DispId(0x200)]
        IStream CreateResultImage();

        // Adds a track to the media (defaults to audio, always 2352 bytes/sector)
        [DispId(0x201)]
        int AddTrack(IMAPI_CD_SECTOR_TYPE dataType, [In, MarshalAs(UnmanagedType.Interface)] IStream data);

        // Adds a special pregap to the first track, and implies an audio CD
        [DispId(0x202)]
        void AddSpecialPregap([In, MarshalAs(UnmanagedType.Interface)] IStream data);

        // Adds an R-W subcode generation object to supply R-W subcode (i.e. CD-Text or CD-G).
        [DispId(0x203)]
        void AddSubcodeRWGenerator([In, MarshalAs(UnmanagedType.Interface)] IStream subcode);

        [DispId(0x100)]
        IMAPI_FORMAT2_RAW_CD_DATA_SECTOR_TYPE ResultingImageType { set; get; }

        // Equal to (final user LBA+1), defines minimum disc size image can be written to.
        [DispId(0x101)]
        int StartOfLeadout { get; }

        // 
        [DispId(0x102)]
        int StartOfLeadoutLimit { set; get; }

        // Disables gapless recording of consecutive audio tracks
        [DispId(0x103)]
        bool DisableGaplessAudio { set; get; }

        // The Media Catalog Number for the CD image
        [DispId(260)]
        string MediaCatalogNumber { set; get; }

        // The starting track number (only for pure audio CDs)
        [DispId(0x105)]
        int StartingTrackNumber { set; get; }

        [DispId(0x106)]
        IRawCDImageTrackInfo this[int trackIndex] { [DispId(0x106)] get; }

        [DispId(0x107)]
        int NumberOfExistingTracks { get; }

        [DispId(0x108)]
        int LastUsedUserSectorInImage { get; }

        [DispId(0x109)]
        object[] ExpectedTableOfContents { get; }
    }


    [ComImport]
    [Guid("25983551-9D65-49CE-B335-40630D901227")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IRawCDImageTrackInfo
    {
        // The LBA of the first user sector in this track
        [DispId(0x100)]
        int StartingLba { get; }

        // The number of user sectors in this track
        [DispId(0x101)]
        int SectorCount { get; }

        // The track number assigned for this track
        [DispId(0x102)]
        int TrackNumber { get; }

        // The type of data being recorded on the current sector.
        [DispId(0x103)]
        IMAPI_CD_SECTOR_TYPE SectorType { get; }

        // The International Standard Recording Code (ISRC) for this track.
        [DispId(260)]
        string ISRC { get; set; }

        // The digital audio copy setting for this track
        [DispId(0x105)]
        IMAPI_CD_TRACK_DIGITAL_COPY_SETTING DigitalAudioCopySetting { get; set; }

        // The audio provided already has preemphasis applied (rare).
        [DispId(0x106)]
        bool AudioHasPreemphasis { get; set; }

        // The list of current track-relative indexes within the CD track.
        [DispId(0x107)]
        object[] TrackIndexes { get; }

        // Add the specified LBA (relative to the start of the track) as an index.
        [DispId(0x200)]
        void AddTrackIndex(int lbaOffset);

        // Removes the specified LBA (relative to the start of the track) as an index.
        [DispId(0x201)]
        void ClearTrackIndex(int lbaOffset);
    }




    [ComImport]
    [Guid("25983550-9D65-49CE-B335-40630D901227")]
    [CoClass(typeof(MsftRawCDImageCreatorClass))]
    public interface MsftRawCDImageCreator : IRawCDImageCreator
    {
    }

    [ComImport]
    [Guid("25983561-9D65-49CE-B335-40630D901227")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ClassInterface(ClassInterfaceType.None)]
    public class MsftRawCDImageCreatorClass
    {
    }
}