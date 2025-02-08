namespace MVBP.SnapPoints;

internal static class SnapPointNames
{
    public const string TOP = "$hud_snappoint_top";
    public const string BOTTOM = "$hud_snappoint_bottom";
    public const string CENTER = "$hud_snappoint_center";
    public const string CORNER = "$hud_snappoint_corner";
    public const string EDGE = "$hud_snappoint_edge";
    public const string MID = "$hud_snappoint_mid";
    public const string INNER = "$hud_snappoint_inner";
    public const string OUTER = "Outer";
    public const string SNAPPOINT = "Snappoint";
    public const string ORIGIN = "Origin";
    public const string EXTRA = "Extra";

    /// <summary>
    ///  The tag that identifies a transform as a snap point.
    /// </summary>
    public const string TAG = "snappoint";

    /// <summary>
    /// The name of SnapPoints that the Valheim devs did not name yet
    /// (generally because they are supposed to be unavailable to the player)
    /// </summary>
    public const string DEFAULT_NAME = "_snappoint";
}
