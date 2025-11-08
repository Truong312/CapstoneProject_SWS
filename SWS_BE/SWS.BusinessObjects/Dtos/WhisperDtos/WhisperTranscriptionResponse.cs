namespace SWS.BusinessObjects.Dtos.WhisperDtos;

public class WhisperTranscriptionResponse
{
    public string Text { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public double Duration { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Compute { get; set; } = string.Empty;
    public TranscriptionParams? Params { get; set; }
}

public class TranscriptionParams
{
    public int BeamSize { get; set; }
    public double Temperature { get; set; }
    public string Task { get; set; } = string.Empty;
    public bool WordTimestamps { get; set; }
}

