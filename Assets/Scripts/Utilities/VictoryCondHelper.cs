public enum VictoryType {
    StickMan,
    Mouse,
    Draw,
}

public struct VictoryData {
    public VictoryType Type;
    public string Message;
    public int FinalScore;
}